using MailKit.Net.Smtp;
using MimeKit;
using TodoApi.Models;

namespace TodoApi.Services;

public class EmailService
{
    private readonly IConfiguration _config;

    public EmailService(IConfiguration config)
    {
        _config = config;
    }

    public async Task SendOtpAsync(string toEmail, string otp)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("DigiBank", _config["Email:Username"]));
        message.To.Add(new MailboxAddress("", toEmail));
        message.Subject = "Your DigiBank OTP Code";
        message.Body = new TextPart("html")
        {
            Text = $@"
                <h2>DigiBank Verification</h2>
                <p>Your OTP code is:</p>
                <h1 style='color:#2196F3;letter-spacing:8px'>{otp}</h1>
                <p>This code expires in 5 minutes.</p>
            "
        };

        using var client = new SmtpClient();
        await client.ConnectAsync(_config["Email:Host"], int.Parse(_config["Email:Port"]!), false);
        await client.AuthenticateAsync(_config["Email:Username"], _config["Email:Password"]);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }
    public async Task SendDemandeEmailAsync(string toEmail, Client client, string pdfPath)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("ATBDigiBank", _config["Email:Username"]));
        message.To.Add(new MailboxAddress("", toEmail));
        message.Subject = "Votre demande d'ouverture de compte - DigiBank";

        var builder = new BodyBuilder();
        builder.HtmlBody = $@"
        <h2>Bonjour {client.Prenom} {client.Nom},</h2>
        <p>Votre demande d'ouverture de compte a bien été généré.</p>
        <p>Veuillez trouver en pièce jointe le formulaire de votre demande.</p>
        <p>Notre équipe traitera votre dossier dans un délai de 24 à 48 heures ouvrables.</p>
        <br/>
        <p>Cordialement,<br/>L'équipe ATB</p>
    ";

        // Attach the PDF
        builder.Attachments.Add(pdfPath);
        message.Body = builder.ToMessageBody();

        using var smtpClient = new SmtpClient();
        await smtpClient.ConnectAsync(_config["Email:Host"], int.Parse(_config["Email:Port"]!), false);
        await smtpClient.AuthenticateAsync(_config["Email:Username"], _config["Email:Password"]);
        await smtpClient.SendAsync(message);
        await smtpClient.DisconnectAsync(true);
    }


    public async Task SendDemandeCreditEmailAsync(string toEmail, Client client, string pdfPath, DemandeCredit demande)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("ATBDigiBank", _config["Email:Username"]));
        message.To.Add(new MailboxAddress("", toEmail));
        message.Subject = "Votre demande de crédit - ATB DigiBank";

        var typeLabel = demande.TypeCredit.ToLower() switch
        {
            "consommation" => "Crédit Consommation",
            "vehicule" => "Crédit Véhicule",
            "immobilier" => "Crédit Immobilier",
            _ => demande.TypeCredit
        };

        var builder = new BodyBuilder();
        builder.HtmlBody = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <style>
        body {{ font-family: Arial, sans-serif; background-color: #f4f6f9; margin: 0; padding: 0; }}
        .container {{ max-width: 600px; margin: 40px auto; background-color: #ffffff; border-radius: 10px; overflow: hidden; box-shadow: 0 4px 12px rgba(0,0,0,0.1); }}
        .header {{ background-color: #9D072A; padding: 30px; text-align: center; }}
        .header h1 {{ color: #ffffff; margin: 0; font-size: 24px; letter-spacing: 2px; }}
        .header p {{ color: #f0a0b0; margin: 5px 0 0; font-size: 13px; }}
        .body {{ padding: 35px 40px; }}
        .body h2 {{ color: #9D072A; font-size: 20px; margin-bottom: 5px; }}
        .body p {{ color: #555555; font-size: 14px; line-height: 1.7; }}
        .card {{ background-color: #fdf0f3; border-left: 4px solid #9D072A; border-radius: 0 6px 6px 0; padding: 18px 22px; margin: 25px 0; }}
        .card-row {{ display: flex; justify-content: space-between; margin-bottom: 10px; }}
        .card-row:last-child {{ margin-bottom: 0; }}
        .card-label {{ color: #888888; font-size: 12px; text-transform: uppercase; letter-spacing: 1px; }}
        .card-value {{ color: #222222; font-size: 14px; font-weight: bold; text-align: right; }}
        .badge {{ display: inline-block; background-color: #fde8ed; color: #9D072A; border-radius: 20px; padding: 5px 14px; font-size: 12px; font-weight: bold; margin-bottom: 20px; }}
        .status {{ background-color: #ffffff; border: 1px solid #dddddd; border-radius: 6px; padding: 12px 18px; font-size: 13px; color: #444444; margin: 20px 0; }}
        .footer {{ background-color: #fdf0f3; padding: 20px 40px; text-align: center; }}
        .footer p {{ color: #999999; font-size: 12px; margin: 4px 0; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>ATB DigiBank</h1>
            <p>Arab Tunisian Bank — Votre banque digitale</p>
        </div>
        <div class='body'>
            <h2>Bonjour {client.Prenom} {client.Nom},</h2>
            <p>Votre demande de crédit a bien été reçue et enregistrée dans notre système. Vous trouverez ci-joint le récapitulatif de votre dossier.</p>

            <span class='badge'>{typeLabel}</span>

            <div class='card'>
                <div class='card-row'>
                    <span class='card-label'>Référence</span>
                    <span class='card-value'>{demande.Reference}</span>
                </div>
                <div class='card-row'>
                    <span class='card-label'>Date de demande</span>
                    <span class='card-value'>{demande.DateEnvoi?.ToString("dd/MM/yyyy")}</span>
                </div>
                <div class='card-row'>
                    <span class='card-label'>Montant sollicité</span>
                    <span class='card-value'>{demande.Montant?.ToString("N2")} TND</span>
                </div>
                <div class='card-row'>
                    <span class='card-label'>Durée</span>
                    <span class='card-value'>{demande.DureeMois} mois</span>
                </div>
                <div class='card-row'>
                    <span class='card-label'>Objet</span>
                    <span class='card-value'>{demande.Objet}</span>
                </div>
            </div>

            <div class='status'>
                <strong>Statut :</strong> Votre dossier est en cours d'étude. Notre équipe vous contactera dans un délai de 24 à 48 heures ouvrables.
            </div>

            <p>Pour toute question, n'hésitez pas à contacter votre agence ou à vous connecter sur l'application ATB DigiBank.</p>
            <p>Cordialement,<br/><strong>L'équipe ATB DigiBank</strong></p>
        </div>
        <div class='footer'>
            <p>Cet email est généré automatiquement, merci de ne pas y répondre.</p>
            <p>© {DateTime.Now.Year} Arab Tunisian Bank. Tous droits réservés.</p>
        </div>
    </div>
</body>
</html>
";

        builder.Attachments.Add(pdfPath);
        message.Body = builder.ToMessageBody();

        using var smtpClient = new SmtpClient();
        await smtpClient.ConnectAsync(_config["Email:Host"], int.Parse(_config["Email:Port"]!), false);
        await smtpClient.AuthenticateAsync(_config["Email:Username"], _config["Email:Password"]);
        await smtpClient.SendAsync(message);
        await smtpClient.DisconnectAsync(true);
    }
}