using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using StriderWebApi.Services.Interfaces;

namespace StriderWebApi.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendAccountActivationEmailAsync(string email, string username, string activationToken)
        {
            var activationUrl = $"{_configuration["AppSettings:FrontendBaseUrl"]}/activate?token={activationToken}";

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Strider", _configuration["EmailSettings:SenderEmail"]));
            message.To.Add(new MailboxAddress(username, email));
            message.Subject = "Activá tu cuenta en Strider";

            message.Body = new TextPart("plain")
            {
                Text = $@"Hola {username},

                Gracias por registrarte en Strider. Activá tu cuenta con este enlace:

                {activationUrl}

                Este link es válido por 24 horas. Si no creaste esta cuenta, podés ignorar este mensaje."
            };

            using var client = new SmtpClient();
            await client.ConnectAsync(
                _configuration["EmailSettings:SmtpHost"],
                int.Parse(_configuration["EmailSettings:SmtpPort"]),
                SecureSocketOptions.StartTls);

            await client.AuthenticateAsync(
                _configuration["EmailSettings:SmtpUser"],
                _configuration["EmailSettings:SmtpPass"]);

            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
}
