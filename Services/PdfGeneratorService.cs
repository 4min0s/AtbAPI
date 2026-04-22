using ClosedXML.Excel;
using iText.IO.Font.Constants;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using TodoApi.Services;
using Cell = iText.Layout.Element.Cell;
using Document = iText.Layout.Document;
using Paragraph = iText.Layout.Element.Paragraph;
using Table = iText.Layout.Element.Table;
using Path = System.IO.Path;
using TodoApi.Models;
using DocumentFormat.OpenXml;
using Microsoft.EntityFrameworkCore;



namespace TodoApi.Services
{
    public class PdfGeneratorService
    {
        private readonly string _xlsxPath;
        private readonly string _outputRootFolder;  // e.g. "wwwroot/tableaux"
        private readonly string _logoPath;

        public PdfGeneratorService(string xlsxPath, string outputRootFolder, string logoPath)
        {
            _xlsxPath = xlsxPath;
            _outputRootFolder = outputRootFolder;
            _logoPath = logoPath;
        }

        /// <summary>
        /// Generates the PDF and returns the full path of the created file.
        /// The file is saved under: outputRootFolder/referenceCredit/tableau.pdf
        /// </summary>
        public string GeneratePdf(Credit credit)
        {
            // ── Build output path ──────────────────────────────────
            var safeName = string.Concat(credit.Reference!.Split(Path.GetInvalidFileNameChars()));
            var folder = Path.Combine(_outputRootFolder, "demandescredits", safeName);
            Directory.CreateDirectory(folder);
            var outputPdfPath = Path.Combine(folder, "tab_amortissement.pdf");

            string periodiciteLabel = credit.Periodicite switch
            {
                1 => "Mensuel",
                2 => "Trimestriel",
                3 => "Semestriel",
                _ => ""
            };

            // ── Get echeances from the credit navigation property ──
            var rows = credit.Echeances
                .OrderBy(e => e.NumeroEcheance)
                .ToList();

            if (!rows.Any())
                throw new InvalidOperationException($"No echeances found for credit {credit.Reference}.");

            // ── Build PDF ──────────────────────────────────────────
            using var writer = new PdfWriter(outputPdfPath);
            using var pdf = new PdfDocument(writer);
            using var doc = new Document(pdf, PageSize.A4);

            doc.SetMargins(25, 30, 40, 30);

            var bold = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
            var regular = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);

            var headerBg = new DeviceRgb(50, 50, 50);
            var altRowBg = new DeviceRgb(245, 245, 245);
            var borderCol = new DeviceRgb(180, 180, 180);

            // ── Logo + date header ─────────────────────────────────
            var headerTable = new Table(UnitValue.CreatePercentArray(new float[] { 1, 1 }))
                .UseAllAvailableWidth().SetMarginBottom(4).SetBorder(Border.NO_BORDER);

            var logo = ImageDataFactory.Create(_logoPath);
            var logoImage = new Image(logo).SetWidth(50).SetHeight(50);

            headerTable.AddCell(new Cell().Add(logoImage)
                .SetBorder(Border.NO_BORDER)
                .SetVerticalAlignment(VerticalAlignment.MIDDLE));

            headerTable.AddCell(new Cell()
                .Add(new Paragraph($"Tunis le,   {DateTime.Now:dd/MM/yyyy}")
                    .SetFont(regular).SetFontSize(9)
                    .SetTextAlignment(TextAlignment.RIGHT))
                .SetBorder(Border.NO_BORDER)
                .SetVerticalAlignment(VerticalAlignment.MIDDLE));

            doc.Add(headerTable);

            // ── Title ──────────────────────────────────────────────
            doc.Add(new Paragraph("Tableau d'amortissement")
                .SetFont(bold).SetFontSize(14)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetMarginBottom(10));

            // ── Info grid ──────────────────────────────────────────
            var infoTable = new Table(UnitValue.CreatePercentArray(new float[] { 1, 1 }))
                .UseAllAvailableWidth().SetMarginBottom(12).SetBorder(Border.NO_BORDER);

            void AddInfoRow(string label, string value, Table t) =>
                t.AddCell(new Cell()
                    .Add(new Paragraph($"- {label} :   {value}").SetFont(regular).SetFontSize(8.5f))
                    .SetBorder(Border.NO_BORDER).SetPaddingBottom(3));

            var client = credit.IdClientNavigation;
            var totalCapital = rows.Sum(e => e.CapitalRembourse ?? 0);

            var leftInfo = new Table(1).UseAllAvailableWidth().SetBorder(Border.NO_BORDER);
            AddInfoRow("Référence crédit", credit.Reference ?? "-", leftInfo);
            AddInfoRow("N° compte", credit.IdCompteNavigation?.Rib ?? "-", leftInfo);
            AddInfoRow("Nom et prénom", $"{client?.Nom ?? "-"} {client?.Prenom ?? "-"}", leftInfo);
            AddInfoRow("Nature du crédit", credit.NatureCredit ?? "-", leftInfo);
            AddInfoRow("Montant du crédit", $"{credit.Montant:N3}", leftInfo);

            var rightInfo = new Table(1).UseAllAvailableWidth().SetBorder(Border.NO_BORDER);
            AddInfoRow("TMM", $"{credit.Tmm:F2}%", rightInfo);
            AddInfoRow("Marge de la banque", $"{credit.MargeBanque:F0}%", rightInfo);
            AddInfoRow("Taux d'intérêt : TMM+marge", $"{credit.TauxInteret:F2}%", rightInfo);
            AddInfoRow("Durée", $"{credit.DureeMois}M", rightInfo);
            AddInfoRow("Délais de grâce", $"{credit.DureeGrace ?? 0}", rightInfo);
            AddInfoRow("Périodicité", periodiciteLabel, rightInfo);
            AddInfoRow("Date Prem. Ech.", credit.PremierEcheance?.ToString("dd/MM/yyyy") ?? "-", rightInfo);
            AddInfoRow("Nb Echéances", $"{credit.NbEcheance}", rightInfo);

            infoTable.AddCell(new Cell().Add(leftInfo).SetBorder(Border.NO_BORDER));
            infoTable.AddCell(new Cell().Add(rightInfo).SetBorder(Border.NO_BORDER));
            doc.Add(infoTable);

            // ── Amortization table ─────────────────────────────────
            float[] colWidths = { 1.2f, 1.6f, 1.8f, 1.6f, 1.8f, 1f, 1.6f };
            var table = new Table(UnitValue.CreatePercentArray(colWidths))
                .UseAllAvailableWidth()
                .SetBorder(new SolidBorder(borderCol, 0.5f));

            string[] headers = { "N°", "Date d'échéance", "Capital Restant",
                          "Intérêts", "Rem. Principal", "Frais", "Total Ech." };

            foreach (var h in headers)
                table.AddHeaderCell(new Cell()
                    .Add(new Paragraph(h).SetFont(bold).SetFontSize(7.5f)
                        .SetFontColor(ColorConstants.WHITE))
                    .SetBackgroundColor(headerBg)
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetPadding(5)
                    .SetBorder(new SolidBorder(ColorConstants.WHITE, 0.5f)));

            for (int i = 0; i < rows.Count; i++)
            {
                var r = rows[i];
                var bg = i % 2 == 1 ? altRowBg : ColorConstants.WHITE;

                Cell MakeCell(string text, bool center = false) =>
                    new Cell()
                        .Add(new Paragraph(text).SetFont(regular).SetFontSize(7.5f))
                        .SetBackgroundColor(bg)
                        .SetTextAlignment(center ? TextAlignment.CENTER : TextAlignment.RIGHT)
                        .SetPaddingRight(5).SetPaddingLeft(5)
                        .SetPaddingTop(3).SetPaddingBottom(3)
                        .SetBorder(new SolidBorder(borderCol, 0.3f));

                table.AddCell(MakeCell(r.NumeroEcheance.ToString(), true));
                table.AddCell(MakeCell(r.DatePaiement?.ToString("dd/MM/yyyy") ?? "-", true));
                table.AddCell(MakeCell($"{r.CapitalRestant:N3}"));
                table.AddCell(MakeCell($"{r.Interet:N3}"));
                table.AddCell(MakeCell($"{r.CapitalRembourse:N3}"));
                table.AddCell(MakeCell($"{r.Frais:N3}"));
                table.AddCell(MakeCell($"{r.EcheanceMontant:N3}"));
            }

            doc.Add(table);

            doc.Add(new Paragraph("\n\nSignature")
                .SetFont(regular).SetFontSize(9)
                .SetTextAlignment(TextAlignment.RIGHT)
                .SetMarginTop(20));

            return outputPdfPath;
        }

        static DateTime SafeGetDate(IXLCell cell)
        {
            if (cell.DataType == XLDataType.DateTime) return cell.GetDateTime();
            if (DateTime.TryParse(cell.GetString(), out var d)) return d;
            if (double.TryParse(cell.GetString(), out var serial)) return DateTime.FromOADate(serial);
            return DateTime.MinValue;
        }

        static double SafeGetDouble(IXLCell cell)
        {
            if (cell.DataType == XLDataType.Number) return cell.GetDouble();
            return double.TryParse(cell.GetString(),
                System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out var v) ? v : 0;
        }







        public async Task<List<Echeance>> GenerateAndSaveEcheancesAsync(Credit credit, DigiBankContext db)
        {
            // ── Run simulation ──
            var simulator = new CreditSimulatorService(_xlsxPath);
            simulator.Simulate(
                montant: (double)credit.Montant,
                tauxAnnuel: (double)credit.TauxInteret,
                periodicite: credit.Periodicite ?? 1,
                dureegrace: credit.DureeGrace ?? 0,
                durationmounths: (double)credit.DureeMois
            );






            var rows = new List<AmortizationRow>();
            int nbEcheance = 0;
            decimal montant_total = 0;

            using (var wb = new XLWorkbook(_xlsxPath))
            {
                var ws = wb.Worksheet("TAM");

                // Read NbEcheance from P4
                nbEcheance = (int)SafeGetDouble(ws.Cell("P4"));
                montant_total = (decimal)SafeGetDouble(ws.Cell("P8"));
                credit.DateDerniereEcheance = DateOnly.FromDateTime(SafeGetDate(ws.Cell("P3")));

                for (int row = 15; row <= 500; row++)
                {
                    var echNum = SafeGetDouble(ws.Cell(row, 8));
                    if (echNum == 0) break;

                    rows.Add(new AmortizationRow
                    {
                        Numero = (int)echNum,
                        Date = SafeGetDate(ws.Cell(row, 11)),
                        CapitalRestant = SafeGetDouble(ws.Cell(row, 12)),
                        Interets = SafeGetDouble(ws.Cell(row, 13)),
                        CapitalRembourse = SafeGetDouble(ws.Cell(row, 14)),
                        Frais = SafeGetDouble(ws.Cell(row, 15)),
                        TotalPaye = SafeGetDouble(ws.Cell(row, 16))
                    });
                }


            }

            // ── Update NbEcheance on the credit record ──
            
            credit.NbEcheance = nbEcheance;
            credit.MontantTotalARembourser = montant_total;
            db.Credits.Update(credit);

            // ── Map to Echeance and save ──
            var echeances = rows.Select(r => new Echeance
            {
                IdCredit = credit.Id,
                NumeroEcheance = r.Numero,
                DatePaiement = DateOnly.FromDateTime(r.Date),
                CapitalRestant = (decimal)r.CapitalRestant,
                Interet = (decimal)r.Interets,
                CapitalRembourse = (decimal)r.CapitalRembourse,
                Frais = (decimal)r.Frais,
                EcheanceMontant = (decimal)r.TotalPaye,
                Etat = 0,
            }).ToList();

            await db.Echeances.AddRangeAsync(echeances);
            await db.SaveChangesAsync(); // saves both the echeances and the updated NbEcheance

            return echeances;
        }











    }























    public class AmortizationRow
    {
        public int Numero { get; set; }
        public DateTime Date { get; set; }
        public double CapitalRestant { get; set; }
        public double Interets { get; set; }
        public double CapitalRembourse { get; set; }
        public double Frais { get; set; }
        public double TotalPaye { get; set; }
    }
}