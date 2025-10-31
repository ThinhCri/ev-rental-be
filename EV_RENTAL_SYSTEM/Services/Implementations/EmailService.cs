using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using MimeKit;
using EV_RENTAL_SYSTEM.Services.Interfaces;

namespace EV_RENTAL_SYSTEM.Services.Implementations
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration config, ILogger<EmailService> logger)
        {
            _config = config;
            _logger = logger;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            try
            {
                var senderEmail = _config["EmailSettings:SenderEmail"];
                var appPassword = _config["EmailSettings:AppPassword"];

                if (string.IsNullOrEmpty(senderEmail) || string.IsNullOrEmpty(appPassword))
                {
                    _logger.LogWarning("Email settings not configured properly");
                    return;
                }

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("EV Rental System", senderEmail));
                message.To.Add(new MailboxAddress("", to));
                message.Subject = subject;
                message.Body = new TextPart("html") { Text = body };

                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                    await client.AuthenticateAsync(senderEmail, appPassword);
                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                }

                _logger.LogInformation("Email sent successfully to {Email}", to);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending email to {Email}: {Error}", to, ex.Message);
                throw;
            }
        }
    }
}

