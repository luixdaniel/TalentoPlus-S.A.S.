using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace ApiTalento.Web.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendWelcomeEmailAsync(string toEmail, string employeeName)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(
                    _configuration["Email:FromName"],
                    _configuration["Email:FromAddress"]
                ));
                message.To.Add(new MailboxAddress(employeeName, toEmail));
                message.Subject = "¡Bienvenido a TalentoPlus S.A.S.!";

                var builder = new BodyBuilder
                {
                    HtmlBody = $@"
                        <html>
                        <body style='font-family: Arial, sans-serif;'>
                            <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                                <h2 style='color: #2c3e50;'>¡Bienvenido a TalentoPlus S.A.S.!</h2>
                                <p>Estimado/a <strong>{employeeName}</strong>,</p>
                                <p>Su registro en nuestro sistema ha sido <strong>exitoso</strong>.</p>
                                <p>A partir de este momento, ya puede autenticarse en nuestra plataforma utilizando sus credenciales.</p>
                                <div style='background-color: #f8f9fa; padding: 15px; border-left: 4px solid #007bff; margin: 20px 0;'>
                                    <p style='margin: 0;'><strong>Correo electrónico:</strong> {toEmail}</p>
                                </div>
                                <p>Si tiene alguna pregunta, no dude en contactarnos.</p>
                                <hr style='border: 1px solid #dee2e6; margin: 30px 0;'>
                                <p style='color: #6c757d; font-size: 12px;'>
                                    Este es un correo automático. Por favor no responda a este mensaje.<br>
                                    TalentoPlus S.A.S. - Sistema de Gestión de Recursos Humanos<br>
                                    © 2025 Todos los derechos reservados.
                                </p>
                            </div>
                        </body>
                        </html>
                    "
                };

                message.Body = builder.ToMessageBody();

                using var client = new SmtpClient();
                
                await client.ConnectAsync(
                    _configuration["Email:SmtpServer"],
                    int.Parse(_configuration["Email:SmtpPort"] ?? "587"),
                    SecureSocketOptions.StartTls
                );

                await client.AuthenticateAsync(
                    _configuration["Email:Username"],
                    _configuration["Email:Password"]
                );

                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                _logger.LogInformation($"Welcome email sent successfully to {toEmail}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending email to {toEmail}: {ex.Message}");
                throw;
            }
        }
    }
}

