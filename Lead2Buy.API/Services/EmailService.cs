using System.Net;
using System.Net.Mail;

namespace Lead2Buy.API.Services
{
    public interface IEmailService
    {
        Task SendWelcomeEmailAsync(string toEmail, string toName);
    }

    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendWelcomeEmailAsync(string toEmail, string toName)
        {
            var smtpHost = "in-v3.mailjet.com";
            var smtpPort = 587;
            var smtpUser = _configuration["MAILJET_API_KEY"];
            var smtpPass = _configuration["MAILJET_SECRET_KEY"];
            var fromEmail = _configuration["MAILJET_FROM_EMAIL"];
            var fromName = "Equipe Lead2Buy";

            if (string.IsNullOrEmpty(smtpUser) || string.IsNullOrEmpty(smtpPass) || string.IsNullOrEmpty(fromEmail))
            {
                Console.WriteLine("ERRO: As variáveis de ambiente do Mailjet (API_KEY, SECRET_KEY, FROM_EMAIL) não estão configuradas. O e-mail não será enviado.");
                return;
            }

            var client = new SmtpClient(smtpHost, smtpPort)
            {
                Credentials = new NetworkCredential(smtpUser, smtpPass),
                EnableSsl = true
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(fromEmail, fromName),
                Subject = "Bem-vindo ao Lead2Buy CRM!",
                Body = $"Olá {toName},<br><br>Seja muito bem-vindo ao Lead2Buy CRM. Estamos felizes em ter você conosco.<br><br>Atenciosamente,<br>Equipe Lead2Buy",
                IsBodyHtml = true,
            };
            mailMessage.To.Add(toEmail);

            try
            {
                await client.SendMailAsync(mailMessage);
                Console.WriteLine($"E-mail de boas-vindas enviado para {toEmail}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Falha ao enviar e-mail para {toEmail}: {ex.Message}");
            }
        }
    }
}