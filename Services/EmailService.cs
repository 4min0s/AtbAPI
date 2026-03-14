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
}