using MailKit.Net.Smtp;
using MimeKit;
using System.Threading.Tasks;

namespace Daraz_CloneAgain.Services
{

    public class EmailService
    {
        private readonly string smtpServer = "smtp.gmail.com";  // Gmail SMTP
        private readonly int smtpPort = 587;                   // TLS Port
        private readonly string smtpUser = "yourgmail@gmail.com"; // tumhara Gmail
        private readonly string smtpPass = "your-app-password";  // Gmail App Password (not normal password)

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Daraz Clone", smtpUser));
            message.To.Add(new MailboxAddress("", toEmail));
            message.Subject = subject;

            message.Body = new TextPart("plain")
            {
                Text = body
            };

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(smtpServer, smtpPort, MailKit.Security.SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(smtpUser, smtpPass);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
        }
    }
}


