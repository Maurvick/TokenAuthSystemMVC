using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity.UI.Services;
using MimeKit;

namespace TokenAuthSystemMVC.Services
{
    public class EmailSender : IEmailSender
    {
        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var emailMessage = new MimeMessage();
            
            string sender = "jeromy66@ethereal.email";
            string host = "smtp.ethereal.email";
            string password = "Q4uhhgNEB2CTbQNNYr";

            emailMessage.From.Add(MailboxAddress.Parse(sender));
            emailMessage.To.Add(MailboxAddress.Parse(email));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = htmlMessage
            };

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(host, 587, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(sender, password);
                await client.SendAsync(emailMessage);
                await client.DisconnectAsync(true);
            }
        }
    }
}
