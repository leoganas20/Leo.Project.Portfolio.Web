using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace Leo.Project.Portfolio.Api.Controllers;

public class EmailService
{
    public async Task SendAsync(string clientEmail, string subject, string message)
    {
        if (string.IsNullOrWhiteSpace(clientEmail))
            throw new ArgumentNullException(nameof(clientEmail), "Client email address cannot be null or empty.");

        // Create the "From" address using the client's email
        var fromAddress = new MailboxAddress("LG Software", clientEmail);  // Dynamically set the sender's email
        var toAddress = new MailboxAddress("Creator", "leoganas.jr@gmail.com");  // Creator's email remains fixed

        var emailMessage = new MimeMessage();
        emailMessage.From.Add(fromAddress);  // Set client as sender
        emailMessage.To.Add(toAddress);  // Set creator as recipient
        emailMessage.Subject = subject;
        emailMessage.Body = new TextPart("plain") { Text = message };
        
        // Optionally add the client’s email in the "Reply-To" field
        emailMessage.ReplyTo.Add(fromAddress);

        // Sending email via SMTP
        using (var smtp = new SmtpClient())
        {
            await smtp.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync("leodev.software@gmail.com", "qjprakypiwgdgdtp"); // Use your SMTP credentials
            await smtp.SendAsync(emailMessage);
            await smtp.DisconnectAsync(true);
        }
    }

}