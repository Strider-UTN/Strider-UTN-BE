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

            message.Body = new TextPart("html")
            {
                Text = $@"
                    <html>
                    <body style='background-color: #f4f4f4; padding: 30px 0; font-family: Arial, sans-serif;'>
                        <div style='max-width: 600px; margin: 0 auto; padding: 24px; background-color: #ffffff; border-radius: 12px; box-shadow: 0 2px 8px rgba(0,0,0,0.05);'>
                            <h2 style='margin-top: 0; color: #222;'>Hola {username},</h2>
                            <p style='font-size: 16px; color: #555;'>
                                Gracias por registrarte en <strong>Strider</strong>. Para activar tu cuenta, hacé clic en el botón a continuación:
                            </p>
                            <div style='text-align: center; margin: 24px 0;'>
                                <a href='{activationUrl}' style='
                                    background-color: #1a73e8;
                                    color: #ffffff;
                                    padding: 12px 24px;
                                    border-radius: 8px;
                                    text-decoration: none;
                                    font-weight: bold;
                                    display: inline-block;
                                '>Activar Cuenta</a>
                            </div>
                            <p style='font-size: 14px; color: #777;'>
                                Este enlace estará disponible por 24 horas. Si no creaste esta cuenta, podés ignorar este mensaje.
                            </p>
                            <hr style='border: none; border-top: 1px solid #eee; margin: 32px 0;'>
                            <p style='font-size: 12px; color: #aaa; text-align: center;'>
                                © {DateTime.UtcNow.Year} Strider — Todos los derechos reservados.
                            </p>
                        </div>
                    </body>
                    </html>"
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

        public async Task SendPasswordResetEmailAsync(string email, string username, string resetToken)
        {
            var resetUrl = $"{_configuration["AppSettings:FrontendBaseUrl"]}/reset-password?token={resetToken}";

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Strider", _configuration["EmailSettings:SenderEmail"]));
            message.To.Add(new MailboxAddress(username, email));
            message.Subject = "Recuperá tu contraseña en Strider";

            message.Body = new TextPart("html")
            {
                Text = $@"
                    <html>
                    <body style='background-color: #f4f4f4; padding: 30px 0; font-family: Arial, sans-serif;'>
                        <div style='max-width: 600px; margin: 0 auto; padding: 24px; background-color: #ffffff; border-radius: 12px; box-shadow: 0 2px 8px rgba(0,0,0,0.05);'>
                            <h2 style='margin-top: 0; color: #222;'>Hola {username},</h2>
                            <p style='font-size: 16px; color: #555;'>
                                Recibimos una solicitud para restablecer tu contraseña en <strong>Strider</strong>.
                            </p>
                            <p style='font-size: 14px; color: #555;'>
                                Tu token de recuperación: <strong>{resetToken}</strong>
                            </p>
                            <div style='text-align: center; margin: 24px 0;'>
                                <a href='{resetUrl}' style='
                                    background-color: #1a73e8;
                                    color: #ffffff;
                                    padding: 12px 24px;
                                    border-radius: 8px;
                                    text-decoration: none;
                                    font-weight: bold;
                                    display: inline-block;
                                '>Restablecer contraseña</a>
                            </div>
                            <p style='font-size: 14px; color: #777;'>
                                Este enlace estará disponible por 24 horas. Si no solicitaste este cambio, podés ignorar este mensaje.
                            </p>
                            <hr style='border: none; border-top: 1px solid #eee; margin: 32px 0;'>
                            <p style='font-size: 12px; color: #aaa; text-align: center;'>
                                © {DateTime.UtcNow.Year} Strider — Todos los derechos reservados.
                            </p>
                        </div>
                    </body>
                    </html>"
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
