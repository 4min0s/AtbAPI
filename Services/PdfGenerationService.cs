using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using Npgsql.Internal;
using TodoApi.Models;
using iText.Bouncycastle.Crypto;


public class PdfGenerationService
{
    private readonly string _templatePath = "C:\\Users\\Amine\\Downloads\\Formulaire-ouverture-MAJ-PP-FR (1).pdf";

    public async Task<byte[]> GenerateClientFormAsync(Client client , DemandeCompte demande)
    {
        using var memoryStream = new MemoryStream();

        using var reader = new PdfReader(_templatePath);
        using var writer = new PdfWriter(memoryStream);
        using var pdfDoc = new PdfDocument(reader, writer);

        var font = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
        float fontSize = 9f;

        // ── PAGE 1 ──
        var page1 = pdfDoc.GetPage(1);
        var canvas1 = new PdfCanvas(page1);

        // Convert coordinates from mm to points (1mm = 2.835 points)
        // iText uses points from bottom-left, PDF-XChange uses mm from top-left
        // Page height A4 = 841.89 points = 297mm
        float pageHeight = 841.89f;
        float mmToPt = 2.835f;

        float ConvertX(float xMm) => xMm * mmToPt;
        float ConvertY(float yMm) => pageHeight - (yMm * mmToPt);

        canvas1.BeginText()
            .SetFontAndSize(font, fontSize)

            // Date
            .MoveText(ConvertX(85f), ConvertY(14f))
            .ShowText(DateTime.Now.ToString("dd/MM/yyyy"))

            // Prénom
            .MoveText(0, 0)
            .SetTextMatrix(ConvertX(21.5f), ConvertY(39.5f))
            .ShowText(client.Prenom ?? "")

            // Pays de naissance
            .SetTextMatrix(ConvertX(175f), ConvertY(39.5f))
            .ShowText(client.Pays ?? "")

            // Nom de famille
            .SetTextMatrix(ConvertX(78.5f), ConvertY(44.5f))
            .ShowText(client.Nom ?? "")

            // Numéro CIN
            .SetTextMatrix(ConvertX(116f), ConvertY(54f))
            .ShowText(client.Cin ?? "")

            // Date de délivrance
            .SetTextMatrix(ConvertX(173.2f), ConvertY(54f))
            .ShowText(client.DateDelivrance?.ToString("MM/yyyy") ?? "")

            // Date de naissance
            .SetTextMatrix(ConvertX(130f), ConvertY(34f))
            .ShowText(client.DateNaissance?.ToString("dd/MM/yyyy") ?? "")

            // Adresse
            .SetTextMatrix(ConvertX(19f), ConvertY(120.2f))
            .ShowText(client.Adresse ?? "")

            // Code postal
            .SetTextMatrix(ConvertX(73f), ConvertY(118.4f))
            .ShowText(client.CodePostal ?? "")

            // Ville
            .SetTextMatrix(ConvertX(104f), ConvertY(118.4f))
            .ShowText(client.Ville ?? "")

            // Gouvernorat
            .SetTextMatrix(ConvertX(83.5f), ConvertY(123f))
            .ShowText(client.Gouvernorat ?? "")

            // Pays
            .SetTextMatrix(ConvertX(128.6f), ConvertY(123f))
            .ShowText(client.Pays ?? "")

            // N° Tel
            .SetTextMatrix(ConvertX(18f), ConvertY(109f))
            .ShowText(client.NumTel ?? "")

            // Profession
            .SetTextMatrix(ConvertX(21.5f), ConvertY(152f))
            .ShowText(client.Profession ?? "")

            // Nom employeur
            .SetTextMatrix(ConvertX(31.5f), ConvertY(156.5f))
            .ShowText(client.NomEmployeur ?? "")

            // Montant et devise du revenu mensuel
            .SetTextMatrix(ConvertX(55f), ConvertY(165f))
            .ShowText($"{client.MontantRevMensuelNet} {client.Devise}")

            // Etat civil
            .SetTextMatrix(ConvertX(18.6f), ConvertY(136.7f))
            .ShowText(client.Civilite == true ? "Marié(e)" : "Célibataire")

            .EndText();

        // Sexe checkbox
        canvas1.BeginText()
            .SetFontAndSize(font, fontSize)
            .SetTextMatrix(ConvertX(16f), ConvertY(34f))
            .ShowText(client.Sexe == true ? "M" : "F")
            .EndText();

        // Civilité checkbox
        canvas1.BeginText()
            .SetFontAndSize(font, fontSize)
            .SetTextMatrix(ConvertX(63f), ConvertY(34f))
            .ShowText(client.Civilite == true ? "Marié(e)" : "Célibataire")
            .EndText();

        // Relation avec autres banques
        canvas1.BeginText()
            .SetFontAndSize(font, fontSize)
            .SetTextMatrix(
                client.RelationBanque == true ? ConvertX(63.5f) : ConvertX(85.4f),
                client.RelationBanque == true ? ConvertY(216.7f) : ConvertY(216.4f)
            )
            .ShowText("X")
            .EndText();

 

        canvas1.BeginText()
            .SetFontAndSize(font, fontSize)
            .SetTextMatrix(ConvertX(62.2f), ConvertY(18.2f))
            .ShowText("X")
            .EndText();

        canvas1.Release();

        // ── PAGE 2 ──
        var page2 = pdfDoc.GetPage(2);
        var canvas2 = new PdfCanvas(page2);

        // Type de compte checkbox
        canvas2.BeginText()
         .SetFontAndSize(font, fontSize)
         .SetTextMatrix(ConvertX(9.7f), ConvertY(19f))
         .ShowText(demande.TypeCompte == "Cheque" ? "X" : "")
         .EndText();

        canvas2.BeginText()
            .SetFontAndSize(font, fontSize)
            .SetTextMatrix(ConvertX(9.7f), ConvertY(23.7f))
            .ShowText(demande.TypeCompte == "Courant" ? "X" : "")
            .EndText();
        
        canvas2.BeginText()
            .SetFontAndSize(font, fontSize)
            .SetTextMatrix(ConvertX(9.7f), ConvertY(28.2f))
            .ShowText(demande.TypeCompte == "Epargne" ? "X" : "")
            .EndText();

        // Always X
        canvas2.BeginText()
            .SetFontAndSize(font, fontSize)
            .SetTextMatrix(ConvertX(95.2f), ConvertY(41f))
            .ShowText("X")
            .EndText();

        canvas2.Release();
        pdfDoc.Close();

        return memoryStream.ToArray();
    }





    // Test without API
    public async Task TestGenerateAsync()
    {
        var client = new Client
        {
            Nom = "Ben Ali",
            Prenom = "Mohamed",
            DateNaissance = new DateOnly(1990, 5, 15),
            LieuNaissance = "Tunis",
            Cin = "12345678",
            DateDelivrance = new DateOnly(2015, 3, 20),
            Adresse = "Rue de la République",
            Ville = "Tunis",
            Gouvernorat = "Tunis",
            CodePostal = "1000",
            Pays = "Tunisie",
            NumTel = "22334455",
            Profession = "Ingénieur",
            NomEmployeur = "Société Nationale",
            MontantRevMensuelNet = 2500,
            Devise = "TND",
            RelationBanque = false,
            Sexe = true,
            Civilite = false,
        };
        var demande = new DemandeCompte
        {
            Id = 42,
            IdClient = client.Id,
            DateEnvoi = DateOnly.FromDateTime(DateTime.Now),
            TypeCompte = "Courant",
            Etat = 0,
        };


        var pdfBytes = await GenerateClientFormAsync(client , demande);
        var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "GeneratedForms");
        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        var desktopPath = Path.Combine(folderPath, $"{client.Id}_{demande.Id}.pdf");

        if(Path.Exists(desktopPath))
            File.Delete(desktopPath);
        
        await File.WriteAllBytesAsync(desktopPath, pdfBytes);
        
        Console.WriteLine($"PDF saved to: {desktopPath}");
    }





    public async Task<string> GenerateAndSavePdfAsync(Client client, DemandeCompte demande)
    {
        var pdfBytes = await GenerateClientFormAsync(client, demande);
        var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "GeneratedForms");
        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);
        var filePath = Path.Combine(folderPath, $"{client.Id}_{demande.Id}.pdf");
        await File.WriteAllBytesAsync(filePath, pdfBytes);
        return filePath;
    }
    public async Task<byte[]> GenerateClientFormAsyncV2( DemandeCompte demande)
    {
        using var memoryStream = new MemoryStream();

        using var reader = new PdfReader(_templatePath);
        using var writer = new PdfWriter(memoryStream);
        using var pdfDoc = new PdfDocument(reader, writer);

        var font = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
        float fontSize = 9f;

        // ── PAGE 1 ──
        var page1 = pdfDoc.GetPage(1);
        var canvas1 = new PdfCanvas(page1);

        // Convert coordinates from mm to points (1mm = 2.835 points)
        // iText uses points from bottom-left, PDF-XChange uses mm from top-left
        // Page height A4 = 841.89 points = 297mm
        float pageHeight = 841.89f;
        float mmToPt = 2.835f;

        float ConvertX(float xMm) => xMm * mmToPt;
        float ConvertY(float yMm) => pageHeight - (yMm * mmToPt);

        canvas1.BeginText()
            .SetFontAndSize(font, fontSize)

            // Date
            .MoveText(ConvertX(85f), ConvertY(14f))
            .ShowText(DateTime.Now.ToString("dd/MM/yyyy"))

            // Prénom
            .MoveText(0, 0)
            .SetTextMatrix(ConvertX(21.5f), ConvertY(39.5f))
            .ShowText(demande.Prenom ?? "")

            // Pays de naissance
            .SetTextMatrix(ConvertX(175f), ConvertY(39.5f))
            .ShowText(demande.Pays ?? "")

            // Nom de famille
            .SetTextMatrix(ConvertX(78.5f), ConvertY(44.5f))
            .ShowText(demande.Nom ?? "")

            // Numéro CIN
            .SetTextMatrix(ConvertX(116f), ConvertY(54f))
            .ShowText(demande.Cin ?? "")

            // Date de délivrance
            .SetTextMatrix(ConvertX(173.2f), ConvertY(54f))
            .ShowText(demande.DateDelivrance?.ToString("MM/yyyy") ?? "")

            // Date de naissance
            .SetTextMatrix(ConvertX(130f), ConvertY(34f))
            .ShowText(demande.DateNaissance?.ToString("dd/MM/yyyy") ?? "")

            // Adresse
            .SetTextMatrix(ConvertX(19f), ConvertY(120.2f))
            .ShowText(demande.Adresse ?? "")

            // Code postal
            .SetTextMatrix(ConvertX(73f), ConvertY(118.4f))
            .ShowText(demande.CodePostal ?? "")

            // Ville
            .SetTextMatrix(ConvertX(104f), ConvertY(118.4f))
            .ShowText(demande.Ville ?? "")

            // Gouvernorat
            .SetTextMatrix(ConvertX(83.5f), ConvertY(123f))
            .ShowText(demande.Gouvernorat ?? "")

            // Pays
            .SetTextMatrix(ConvertX(128.6f), ConvertY(123f))
            .ShowText(demande.Pays ?? "")

            // N° Tel
            //.SetTextMatrix(ConvertX(18f), ConvertY(109f))
            //.ShowText(demande.NumTel ?? "")

            // Profession
            .SetTextMatrix(ConvertX(21.5f), ConvertY(152f))
            .ShowText(demande.Profession ?? "")

            // Nom employeur
            .SetTextMatrix(ConvertX(31.5f), ConvertY(156.5f))
            .ShowText(demande.NomEmployeur ?? "")

            // Montant et devise du revenu mensuel
            .SetTextMatrix(ConvertX(55f), ConvertY(165f))
            .ShowText($"{demande.RevenuMensuel} {demande.Devise}")

            // Etat civil
            //.SetTextMatrix(ConvertX(18.6f), ConvertY(136.7f))
            //.ShowText(demande. == true ? "Marié(e)" : "Célibataire")

            .EndText();

        // Sexe checkbox
        canvas1.BeginText()
            .SetFontAndSize(font, fontSize)
            .SetTextMatrix(ConvertX(16f), ConvertY(34f))
            .ShowText(demande.Sexe == true ? "M" : "F")
            .EndText();

        // Civilité checkbox
        //canvas1.BeginText()
        //    .SetFontAndSize(font, fontSize)
        //    .SetTextMatrix(ConvertX(63f), ConvertY(34f))
        //    .ShowText(demande.Civilite == true ? "Marié(e)" : "Célibataire")
        //    .EndText();

        // Relation avec autres banques
        canvas1.BeginText()
            .SetFontAndSize(font, fontSize)
            .SetTextMatrix(
                demande.RelationBanque == true ? ConvertX(63.5f) : ConvertX(85.4f),
                demande.RelationBanque == true ? ConvertY(216.7f) : ConvertY(216.4f)
            )
            .ShowText("X")
            .EndText();



        canvas1.BeginText()
            .SetFontAndSize(font, fontSize)
            .SetTextMatrix(ConvertX(62.2f), ConvertY(18.2f))
            .ShowText("X")
            .EndText();

        canvas1.Release();

        // ── PAGE 2 ──
        var page2 = pdfDoc.GetPage(2);
        var canvas2 = new PdfCanvas(page2);

        // Type de compte checkbox
        canvas2.BeginText()
         .SetFontAndSize(font, fontSize)
         .SetTextMatrix(ConvertX(9.7f), ConvertY(19f))
         .ShowText(demande.TypeCompte == "Cheque" ? "X" : "")
         .EndText();

        canvas2.BeginText()
            .SetFontAndSize(font, fontSize)
            .SetTextMatrix(ConvertX(9.7f), ConvertY(23.7f))
            .ShowText(demande.TypeCompte == "Courant" ? "X" : "")
            .EndText();

        canvas2.BeginText()
            .SetFontAndSize(font, fontSize)
            .SetTextMatrix(ConvertX(9.7f), ConvertY(28.2f))
            .ShowText(demande.TypeCompte == "Epargne" ? "X" : "")
            .EndText();

        // Always X
        canvas2.BeginText()
            .SetFontAndSize(font, fontSize)
            .SetTextMatrix(ConvertX(95.2f), ConvertY(41f))
            .ShowText("X")
            .EndText();

        canvas2.Release();
        pdfDoc.Close();

        return memoryStream.ToArray();
    }


    public async Task<string> GenerateAndSavePdfAsyncV2(Client client , DemandeCompte demande)
    {
        var pdfBytes = await GenerateClientFormAsyncV2( demande);
        var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "GeneratedForms");
        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);
        var filePath = Path.Combine(folderPath, $"{client.Id}_{demande.Id}.pdf");
        await File.WriteAllBytesAsync(filePath, pdfBytes);
        return filePath;
    }
}