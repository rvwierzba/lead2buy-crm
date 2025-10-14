using Lead2Buy.API.Data;
using Lead2Buy.API.DTOs.N8N;
using Lead2Buy.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lead2Buy.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IntegrationsController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IConfiguration _config;

        public IntegrationsController(AppDbContext db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }

        /// <summary>
        /// Recebe leads do N8N (Evolution) e cria um contato.
        /// </summary>
        [HttpPost("n8n/evolution/lead")]
        public async Task<IActionResult> ReceiveLead([FromBody] N8nLeadRequestDto req)
        {
            // validação da API Key
            var keyHeader = Request.Headers["X-Api-Key"].FirstOrDefault();
            var validKey = _config["N8n:ApiKey"];
            if (string.IsNullOrWhiteSpace(validKey) || keyHeader != validKey)
                return Unauthorized("Invalid API key");

            var contact = new Contact
            {
                Name = req.Name.Trim(),
                PhoneNumber = req.Phone.Trim()
                
            };

            _db.Contacts.Add(contact);

            // Registrar atividade de importação
            _db.UserActivities.Add(new UserActivity
            {
                UserId = Guid.Empty, // lead externo, sem usuário responsável ainda
                ContactId = contact.Id,
                OccurredAt = DateTime.UtcNow,
                ActionType = "LeadImported",
                Details = $"Imported from {req.Source} (Interest: {req.Interest})"
            });

            await _db.SaveChangesAsync();

            return Ok(new { contactId = contact.Id });
        }
    }
}
