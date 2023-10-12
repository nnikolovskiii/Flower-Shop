using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity.UI.Services;
using MimeKit;

namespace Test1.Models
{
    public class GmailEmailSender : IEmailSender
    {
        private readonly EmailSenderOptions _emailConfig;

        public GmailEmailSender(EmailSenderOptions emailConfig)
        {
            _emailConfig = emailConfig;
        }
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(_emailConfig.SenderEmail, _emailConfig.SenderEmail));
            emailMessage.To.Add(new MailboxAddress("name", email));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = htmlMessage };
            Send(emailMessage);
            return Task.CompletedTask;
        }


        private void Send(MimeMessage mailMessage)
        {
            using (var client = new SmtpClient())
            {
                try
                {
                    client.Connect(_emailConfig.SmtpServer, _emailConfig.SmtpPort, true);
                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                    client.Authenticate(_emailConfig.SenderEmail, _emailConfig.SmtpPassword);
                    client.Send(mailMessage);
                }
                catch
                {
                    //log an error message or throw an exception or both.
                    throw;
                }
                finally
                {
                    client.Disconnect(true);
                    client.Dispose();
                }
            }
        }
    }
}
