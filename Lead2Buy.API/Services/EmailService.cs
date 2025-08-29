using Mailjet.Client;
using Mailjet.Client.TransactionalEmails;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace Lead2Buy.API.Services
{
    public interface IEmailService
    {
        Task SendWelcomeEmailAsync(string userEmail, string userName);
    }

    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendWelcomeEmailAsync(string userEmail, string userName)
        {
            var apiKey = _configuration["MailjetApiKey"];
            var apiSecret = _configuration["MailjetApiSecret"];

            if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(apiSecret))
            {
                // Apenas loga o erro no console e continua, não quebra a aplicação.
                System.Console.WriteLine("As chaves da API do Mailjet não estão configuradas. O e-mail não será enviado.");
                return;
            }

            var client = new MailjetClient(apiKey, apiSecret);

            var email = new TransactionalEmailBuilder()
                // USA O SEU E-MAIL VERIFICADO COMO REMETENTE
                .WithFrom(new SendContact("lead2buy@rvwtech.com.br", "Equipe Lead2Buy"))
                .WithTo(new SendContact(userEmail, userName))
                .WithSubject("Bem-vindo ao Lead2Buy!")
                .WithHtmlPart(GetWelcomeEmailHtml(userName))
                .Build();

            var response = await client.SendTransactionalEmailAsync(email);

            if (response.Messages != null && response.Messages.FirstOrDefault()?.Status == "success")
            {
                 System.Console.WriteLine($"E-mail de boas-vindas enviado com sucesso para {userEmail}");
            }
            else
            {
                 System.Console.WriteLine($"Falha ao enviar e-mail para {userEmail}. Resposta do Mailjet: {response.Messages?.FirstOrDefault()?.Errors?.FirstOrDefault()?.ErrorMessage}");
            }
        }

        private string GetWelcomeEmailHtml(string userName)
        {
            
            string logoUrl = "https://www.rvwtech.com.br/img/logo.png"; 

            string htmlBody = $@"
                <div style='font-family: Arial, sans-serif; color: #333; max-width: 600px; margin: auto; border: 1px solid #ddd; border-radius: 8px; overflow: hidden;'>
                    <div style='background-color: #4A47A3; padding: 20px; text-align: center;'>
                        <img src='{logoUrl}' alt='Lead2Buy Logo' style='max-width: 150px;'/>
                    </div>
                    <div style='padding: 30px 20px;'>
                        <h2 style='color: #4A47A3;'>Bem-vindo ao Lead2Buy, {userName}!</h2>
                        <p>Olá {userName},</p>
                        <p>Sua conta foi criada com sucesso. Estamos muito felizes em ter você a bordo!</p>
                        <p>Agora você pode começar a gerenciar seus contatos, acompanhar seu funil de vendas e converter mais leads em clientes.</p>
                        <p>Se tiver qualquer dúvida, nossa equipe está à disposição.</p>
                        <p>Atenciosamente,<br/>Equipe Lead2Buy</p>
                    </div>
                    <div style='background-color: #f8f9fa; padding: 15px; text-align: center; font-size: 12px; color: #6c757d;'>
                        Desenvolvido por <a href='http://www.rvwtech.com.br' style='color: #333; font-weight: bold; text-decoration: none;'>RVWtech</a>
                    </div>
                </div>
            ";
            return htmlBody;
        }
    }
}