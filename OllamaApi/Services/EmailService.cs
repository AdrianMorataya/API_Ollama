using MailKit.Net.Smtp;
using MimeKit;

public class EmailService
{
    private readonly string _smtpServer = "smtp.gmail.com";
    private readonly int _smtpPort = 587;
    private readonly string _fromEmail = "adrianmorataya01@gmail.com";
    private readonly string _fromPassword = "wibb igwr shkd jgvj";

    public void SendEmail(string toEmail, string username, string code)
    {
        var email = new MimeMessage();

        email.From.Add(new MailboxAddress("Ollama Support", _fromEmail)); 
        email.To.Add(MailboxAddress.Parse(toEmail));
        email.Subject = "Código de verificación Ollama";

        var htmlBody = $@"
        <div style='font-family: Arial, sans-serif; text-align: center; padding: 20px;'>
            <h2 style='color: #4B6CB7;'>¡{username}!</h2>
            <p style='font-size: 16px; color: #333;'>Gracias por registrarte en <strong>Ollama AI</strong>.</p>
            <p style='font-size: 18px; margin: 20px 0;'>Tu código de verificación es:</p>
            <div style='font-size: 24px; font-weight: bold; color: #2c3e50; background: #f1f1f1; padding: 10px 20px; border-radius: 8px; display: inline-block;'>
                {code}
            </div>
            <p style='font-size: 14px; color: #777; margin-top: 30px;'>
                Si no fuiste tú quien se registró, ignora este correo.
            </p>
        </div>
        ";

        email.Body = new TextPart("html") { Text = htmlBody };

        using var smtp = new SmtpClient();
        smtp.Connect(_smtpServer, _smtpPort, false);
        smtp.Authenticate(_fromEmail, _fromPassword);
        smtp.Send(email);
        smtp.Disconnect(true);
    }
}
