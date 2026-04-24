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
            .MoveText(ConvertX(85f), ConvertY(17f))
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
            .SetTextMatrix(ConvertX(18.6f), ConvertY(139f))
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


    public async Task<string> GenerateAndSavePdfAsyncV2(Client client, DemandeCompte demande)
    {
        var pdfBytes = await GenerateClientFormAsyncV2(demande);

        var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "Documents", "Demandes", demande.Reference);
        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        var filePath = Path.Combine(folderPath, "demande.pdf");
        await File.WriteAllBytesAsync(filePath, pdfBytes);
        return filePath;
    }

    public async Task GenerateDemandeCreditPdfAsync(DemandeCredit demande, Client client, Compte compte)
    {
        var templatePath = "D:\\codes\\projetExcel\\TodoApi\\Assets\\modelcredit.pdf";

        using var memoryStream = new MemoryStream();
        using var reader = new PdfReader(templatePath);
        using var writer = new PdfWriter(memoryStream);
        using var pdfDoc = new PdfDocument(reader, writer);

        var font = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
        float fontSize = 9f;

        float pageHeight = 841.89f;
        float mmToPt = 2.835f;

        float ConvertX(float xMm) => xMm * mmToPt;
        float ConvertY(float yMm) => pageHeight - (yMm * mmToPt);

        var page1 = pdfDoc.GetPage(1);
        var canvas = new PdfCanvas(page1);

        // ── Header ────────────────────────────────────────────────────────────
        canvas.BeginText().SetFontAndSize(font, fontSize)
            .SetTextMatrix(ConvertX(21.4f), ConvertY(44.7f))
            .ShowText(compte.IdAgenceNavigation?.Nom ?? "ATB")  // ← agence nom from compte                                // agence
            .SetTextMatrix(ConvertX(80.0f), ConvertY(45.3f))
            .ShowText(demande.DateEnvoi?.ToString("dd/MM/yyyy") ?? "")  // date envoi
            .SetTextMatrix(ConvertX(142.5f), ConvertY(45.3f))
            .ShowText(compte.Rib ?? "")                                 // N° compte RIB
            .SetTextMatrix(ConvertX(64.7f), ConvertY(57.8f))
            .ShowText($"{client.Prenom} {client.Nom}")                  // nom et prenom
            .SetTextMatrix(ConvertX(64.5f), ConvertY(69.5f))
            .ShowText(client.Cin ?? "")                                 // CIN
            .SetTextMatrix(ConvertX(56.1f), ConvertY(81.9f))
            .ShowText(demande.RevenuMensuelNet?.ToString("F2") ?? "").EndText();

        // Always X at 59.8, 63
        canvas.BeginText().SetFontAndSize(font, fontSize)
            .SetTextMatrix(ConvertX(59.8f), ConvertY(63f))
            .ShowText("X")
            .EndText();






        // ── Autres sources de revenu ───────────────────────────────────────────
        // NOTE: these come from provider fields not stored in DB
        // Pass them as nullable parameters — see method signature below
        // They are handled in the overload

        // ── Revenu mensuel net ─────────────────────────────────────────────────
        canvas.BeginText().SetFontAndSize(font, fontSize)
            .SetTextMatrix(ConvertX(56.1f), ConvertY(81.9f))
            .ShowText(demande.RevenuMensuelNet?.ToString("F2") ?? "")
            .EndText();

        // ── Autres sources de revenu ───────────────────────────────────────────
        if (!string.IsNullOrEmpty(demande.AutresSourcesDeRevenu))
        {
            var autreCoords = demande.AutresSourcesDeRevenu.ToLower() switch
            {
                "affaires personnelles" => (x: 10f, y: 88.5f),
                "dividendes" => (x: 49.5f, y: 89.1f),
                "revenu des placements bancaires" => (x: 91.7f, y: 89.1f),
                "loyers" => (x: 135.9f, y: 89.1f),
                null or "" => (x: 0f, y: 0f),
                _ => (x: 157.9f, y: 89.1f),
            };

            if (autreCoords.x != 0)
            {
                canvas.BeginText().SetFontAndSize(font, fontSize)
                    .SetTextMatrix(ConvertX(autreCoords.x), ConvertY(autreCoords.y))
                    .ShowText("X")
                    .EndText();
            }

            // Montant autres sources
            if (demande.MontantMensuelAutresRevenus != null)
            {
                canvas.BeginText().SetFontAndSize(font, fontSize)
                    .SetTextMatrix(ConvertX(68.2f), ConvertY(94.2f))
                    .ShowText(demande.MontantMensuelAutresRevenus.Value.ToString("F2"))
                    .EndText();
            }
        }


        // ── Situation professionnelle ──────────────────────────────────────────
        var situationCoords = demande.SituationProfessionnelle?.ToLower() switch
        {
            "titulaire" => (x: 60f, y: 73f),
            "contractuel" => (x: 83f, y: 73f),
            "retraité" => (x: 113f, y: 71f),
            "professionnels" => (x: 138f, y: 73f),
            _ => (x: 0f, y: 0f)
        };
        if (situationCoords.x != 0)
        {
            canvas.BeginText().SetFontAndSize(font, fontSize)
                .SetTextMatrix(ConvertX(situationCoords.x), ConvertY(situationCoords.y))
                .ShowText("X")
                .EndText();
        }


        // ── Données crédit ─────────────────────────────────────────────────────
        canvas.BeginText().SetFontAndSize(font, fontSize)
            .SetTextMatrix(ConvertX(60f), ConvertY(172.3f))
            .ShowText(demande.Montant?.ToString("F2") ?? "")            // montant
            .SetTextMatrix(ConvertX(61.6f), ConvertY(175.5f))
            .ShowText(demande.Objet ?? "")                              // objet
            .EndText();

        canvas.BeginText().SetFontAndSize(font, fontSize)
    .SetTextMatrix(ConvertX(58f), ConvertY(192.4f))
    .ShowText(demande.DureeMois?.ToString() ?? "")
    .EndText();

        // ── Periodicité ────────────────────────────────────────────────────────
        var perioCoords = demande.Periodicite switch
        {
            1 => (x: 59.7f, y: 186.7f),
            2 => (x: 86.0f, y: 186.3f),
            3 => (x: 116.0f, y: 186.1f),
            _ => (x: 0f, y: 0f)
        };
        if (perioCoords.x != 0)
        {
            canvas.BeginText().SetFontAndSize(font, fontSize)
                .SetTextMatrix(ConvertX(perioCoords.x), ConvertY(perioCoords.y))
                .ShowText("X")
                .EndText();
        }

        // ── Type de crédit ─────────────────────────────────────────────────────
        switch (demande.TypeCredit.ToLower())
        {
            case "consommation":
                canvas.BeginText().SetFontAndSize(font, fontSize)
                    .SetTextMatrix(ConvertX(102.9f), ConvertY(220f))
                    .ShowText("X")
                    .EndText();
                break;

            case "vehicule":
                var vehicule = demande.DemandeCreditVehicule;
                canvas.BeginText().SetFontAndSize(font, fontSize)
                    .SetTextMatrix(ConvertX(28.4f), ConvertY(205.6f))
                    .ShowText("X")                                      // vehicule checkbox
                    .EndText();

                canvas.BeginText().SetFontAndSize(font, fontSize)
                    .SetTextMatrix(ConvertX(62.2f), ConvertY(213.6f))
                    .ShowText(vehicule?.PuissanceFiscale?.ToString() ?? "") // puissance fiscale
                    .EndText();

                // voiture neuve oui/non
                canvas.BeginText().SetFontAndSize(font, fontSize)
                    .SetTextMatrix(
                        vehicule?.VehiculeNeuf == true ? ConvertX(48.7f) : ConvertX(70f),
                        ConvertY(219.7f))
                    .ShowText("X")
                    .EndText();

                // always X at 43.0 (no Y given in your specs — using 219.7 as closest, adjust if needed)
                canvas.BeginText().SetFontAndSize(font, fontSize)
                    .SetTextMatrix(ConvertX(43.0f), ConvertY(219.7f))
                    .ShowText("X")
                    .EndText();

                // date premiere mise en circulation — only if NOT new
                if (vehicule?.VehiculeNeuf == false && vehicule.DatePremiereMiseEnCirculation != null)
                {
                    canvas.BeginText().SetFontAndSize(font, fontSize)
                        .SetTextMatrix(ConvertX(56.9f), ConvertY(228.2f))
                        .ShowText(vehicule.DatePremiereMiseEnCirculation.Value.ToString("dd/MM/yyyy"))
                        .EndText();
                }
                break;

            case "immobilier":
                var immobilier = demande.DemandeCreditImmobilier;
                canvas.BeginText().SetFontAndSize(font, fontSize)
                    .SetTextMatrix(ConvertX(140.7f), ConvertY(205.2f))
                    .ShowText("X")                                      // immobilier checkbox
                    .EndText();

                // always X for immobilier at 156.6, 240.1
                canvas.BeginText().SetFontAndSize(font, fontSize)
                    .SetTextMatrix(ConvertX(156.6f), ConvertY(240.1f))
                    .ShowText("X")
                    .EndText();
                

                switch (immobilier?.SousType.ToLower())
                {
                    case "construction":
                        canvas.BeginText().SetFontAndSize(font, fontSize)
                            .SetTextMatrix(ConvertX(121.1f), ConvertY(212.9f))
                            .ShowText("X")
                            .EndText();

                        canvas.BeginText().SetFontAndSize(font, fontSize)
                            .SetTextMatrix(ConvertX(170.3f), ConvertY(235.7f))
                            .ShowText(immobilier.DemandeCreditImmobilierConstruction?.CoutTravaux?.ToString("F2") ?? "")
                            .EndText();
                        break;

                    case "renovation":
                        canvas.BeginText().SetFontAndSize(font, fontSize)
                            .SetTextMatrix(ConvertX(170.1f), ConvertY(212.6f))
                            .ShowText("X")
                            .EndText();

                        canvas.BeginText().SetFontAndSize(font, fontSize)
                            .SetTextMatrix(ConvertX(170.3f), ConvertY(235.7f))
                            .ShowText(immobilier.DemandeCreditImmobilierRenovation?.CoutTravaux?.ToString("F2") ?? "")
                            .EndText();
                        break;

                    case "acquisition":
                        // type acquisition: 1, 2, or 3
                        var acq = immobilier.DemandeCreditImmobilierAcquisition;
                        var acqY = acq?.TypeAcquisition switch
                        {
                            "1" => 217.4f,
                            "2" => 221.6f,
                            "3" => 228.2f,
                            _ => 0f
                        };
                        if (acqY != 0)
                        {
                            canvas.BeginText().SetFontAndSize(font, fontSize)
                                .SetTextMatrix(ConvertX(121.1f), ConvertY(acqY))
                                .ShowText("X")
                                .EndText();
                        }
                        // prix immobilier
                        canvas.BeginText().SetFontAndSize(font, fontSize)
                            .SetTextMatrix(ConvertX(170.3f), ConvertY(235.7f))
                            .ShowText(immobilier?.DemandeCreditImmobilierAcquisition?.PrixImmobilier?.ToString("F2") ?? "")
                            .EndText();
                        break;
                }
                break;
        }

        canvas.Release();
        pdfDoc.Close();

        // ── Save to demande folder ─────────────────────────────────────────────
        var folderPath = Path.Combine(
            Directory.GetCurrentDirectory(),
            "Documents", "DemandesCredit", demande.Reference ?? demande.Id.ToString()
        );
        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        var filePath = Path.Combine(folderPath, "demande_credit.pdf");
        await File.WriteAllBytesAsync(filePath, memoryStream.ToArray());
    }
}