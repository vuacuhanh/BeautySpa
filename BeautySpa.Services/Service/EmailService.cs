using BeautySpa.Contract.Services.Interface;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MimeKit.Text;

namespace BeautySpa.Services.Service
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            if (string.IsNullOrWhiteSpace(toEmail))
                throw new ArgumentException("Recipient email is required.", nameof(toEmail));

            var emailSettings = _configuration.GetSection("EmailSettings");

            var fromEmail = emailSettings.GetValue<string>("From")
                            ?? throw new InvalidOperationException("Email 'From' is not configured.");
            var smtpHost = emailSettings.GetValue<string>("Host")
                            ?? throw new InvalidOperationException("SMTP Host is not configured.");
            var smtpPort = emailSettings.GetValue<int?>("Port") ?? 587;
            var username = emailSettings.GetValue<string>("Username")
                            ?? throw new InvalidOperationException("SMTP Username is not configured.");
            var password = emailSettings.GetValue<string>("Password")
                            ?? throw new InvalidOperationException("SMTP Password is not configured.");

            var message = new MimeMessage();
            message.From.Add(MailboxAddress.Parse(fromEmail));
            message.To.Add(MailboxAddress.Parse(toEmail));
            message.Subject = subject;
            message.Body = new TextPart(TextFormat.Html) { Text = body };

            using var smtp = new SmtpClient
            {
                Timeout = 10000 
            };

            await smtp.ConnectAsync(smtpHost, smtpPort, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(username, password);
            await smtp.SendAsync(message);
            await smtp.DisconnectAsync(true);
        }
    }
}
