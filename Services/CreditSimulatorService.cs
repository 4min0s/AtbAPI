using ClosedXML.Excel;
namespace TodoApi.Services
{
    public class CreditSimulatorService
    {
        private readonly string _filePath;
        public CreditSimulatorService(string filePath) => _filePath = filePath;

        private (double years, double months) ConvertMonthsToYearsAndMonths(double totalMonths)
        {
            totalMonths--;
            double years = Math.Floor(totalMonths / 12);
            double months = totalMonths % 12;
            return (years, months);
        }

        public CreditResult Simulate(double montant, double tauxAnnuel,
            int periodicite, int dureegrace, double durationmounths)
        {
            var (durationYears, remainingMonths) = ConvertMonthsToYearsAndMonths(durationmounths);

            using (var wb = new XLWorkbook(_filePath))
            {
                var ws = wb.Worksheets.First();
                ws.Cell("F3").Value = durationYears;
                ws.Cell("F4").Value = remainingMonths;
                ws.Cell("D3").Value = montant;
                ws.Cell("D5").Value = tauxAnnuel / 100;
                ws.Cell("W2").Value = periodicite;
                ws.Cell("M7").Value = dureegrace;
                ws.Cell("F6").Value = DateTime.Now.Date;
                ws.Cell("F6").Style.NumberFormat.Format = "dd/MM/yyyy";
                wb.Save();
            }
            using (var wb = new XLWorkbook(_filePath))
            {
                var ws = wb.Worksheets.First();
                return new CreditResult
                {
                    NombreEcheances = ws.Cell("P4").GetDouble(),
                    TotalInteret = Math.Round(ws.Cell("P5").GetDouble(), 2),
                    TotalCapital = Math.Round(ws.Cell("P6").GetDouble(), 2),
                    CoutTotalCredit = Math.Round(ws.Cell("P8").GetDouble(), 2),
                    echellance = Math.Round(ws.Cell("D8").GetDouble(), 2),
                    dernierechellance = ws.Cell("P3").GetValue<DateTime>().ToString("dd/MM/yyyy")
                };
            }
        }
    }

    public class CreditResult
    {
        public double NombreEcheances { get; set; }
        public double TotalInteret { get; set; }
        public double TotalCapital { get; set; }
        public double CoutTotalCredit { get; set; }
        public double echellance { get; set; }
        public string dernierechellance { get; set; }
    }
}