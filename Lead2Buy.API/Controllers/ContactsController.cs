using System.Globalization;
using System.IO;
using System.Security.Claims;
using CsvHelper;
using CsvHelper.Configuration;
using Lead2Buy.API.Data;
using Lead2Buy.API.Dtos.Contact;
using Lead2Buy.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;
using Lead2Buy.API.Hubs;

namespace Lead2Buy.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ContactsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IHubContext<NotificationHub> _hubContext;

        public ContactsController(AppDbContext context, IHubContext<NotificationHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetContacts([FromQuery] string? search = null, [FromQuery] string? status = null)
        {
            var query = _context.Contacts.AsQueryable();
            if (!string.IsNullOrEmpty(search))
            {
                var searchTerm = search.ToLower();
                query = query.Where(c => 
                    c.Name.ToLower().Contains(searchTerm) || 
                    (c.PhoneNumber != null && c.PhoneNumber.Contains(searchTerm))
                );
            }
            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(c => c.Status == status);
            }
            var contacts = await query.ToListAsync();
            return Ok(contacts);
        }
        
        [HttpGet("{id}")]
        public async Task<IActionResult> GetContactById(int id)
        {
            var contact = await _context.Contacts
                .Include(c => c.Interactions)
                .FirstOrDefaultAsync(c => c.Id == id);
            if (contact == null)
            {
                return NotFound("Contato não encontrado.");
            }
            return Ok(contact);
        }

        [HttpPost]
        public async Task<IActionResult> CreateContact(CreateContactDto request)
        {
            var newContact = new Contact
            {
                Name = request.Name,
                PhoneNumber = request.PhoneNumber,
                Email = request.Email,
                Source = request.Source,
                Gender = request.Gender,
                DateOfBirth = request.DateOfBirth.HasValue ? DateTime.SpecifyKind(request.DateOfBirth.Value, DateTimeKind.Utc) : null,
                Cep = request.Cep,
                Street = request.Street,
                Number = request.Number,
                Neighborhood = request.Neighborhood,
                City = request.City,
                State = request.State,
                Observations = request.Observations,
                Status = "Lead",
                CreatedAt = DateTime.UtcNow
            };
            _context.Contacts.Add(newContact);
            await _context.SaveChangesAsync();
             // Envia uma notificação para todos os clientes conectados
            await _hubContext.Clients.All.SendAsync("ReceiveNotification", $"Novo lead adicionado: {newContact.Name}");
            return CreatedAtAction(nameof(GetContacts), new { id = newContact.Id }, newContact);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateContact(int id, UpdateContactDto request)
        {
            var contact = await _context.Contacts.FindAsync(id);
            if (contact == null)
            {
                return NotFound("Contato não encontrado.");
            }
            contact.Name = request.Name;
            contact.PhoneNumber = request.PhoneNumber;
            contact.Email = request.Email;
            contact.Source = request.Source;
            contact.Gender = request.Gender;
            contact.DateOfBirth = request.DateOfBirth.HasValue ? DateTime.SpecifyKind(request.DateOfBirth.Value, DateTimeKind.Utc) : null;
            contact.Cep = request.Cep;
            contact.Street = request.Street;
            contact.Number = request.Number;
            contact.Neighborhood = request.Neighborhood;
            contact.City = request.City;
            contact.State = request.State;
            contact.Observations = request.Observations;
            contact.Status = request.Status;
            await _context.SaveChangesAsync();
            return Ok(contact);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContact(int id)
        {
            var contact = await _context.Contacts.FindAsync(id);
            if (contact == null)
            {
                return NotFound("Contato não encontrado.");
            }
            _context.Contacts.Remove(contact);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPost("{contactId}/interactions")]
        public async Task<IActionResult> AddInteraction(int contactId, Dtos.Interaction.CreateInteractionDto request)
        {
            var contact = await _context.Contacts.FindAsync(contactId);
            if (contact == null)
            {
                return NotFound("Contato não encontrado.");
            }
            var newInteraction = new Interaction
            {
                Type = request.Type,
                Notes = request.Notes,
                ContactId = contactId,
                CreatedAt = DateTime.UtcNow
            };
            _context.Interactions.Add(newInteraction);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetContacts), new { id = newInteraction.Id }, newInteraction);
        }

        [HttpGet("export")]
        public async Task<IActionResult> ExportContacts()
        {
            var contacts = await _context.Contacts.ToListAsync();
            using (var memoryStream = new MemoryStream())
            using (var streamWriter = new StreamWriter(memoryStream, System.Text.Encoding.UTF8))
            using (var csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture))
            {
                await csvWriter.WriteRecordsAsync(contacts);
                await streamWriter.FlushAsync();
                return File(memoryStream.ToArray(), "text/csv", $"contatos_{DateTime.UtcNow:yyyyMMddHHmmss}.csv");
            }
        }

        [HttpGet("import-template")]
        public IActionResult DownloadImportTemplate()
        {
            var headers = new List<string>
            {
                "Name*", "PhoneNumber*", "Email", "Source", "Gender", "DateOfBirth",
                "Cep", "Street", "Number", "Neighborhood", "City", "State",
                "Observations", "Status"
            };
            var exampleRecord = new 
            {
                Name = "Ex: Ana Silva", PhoneNumber = "Ex: 11987654321", Email = "ana.silva@email.com",
                Source = "Website", Gender = "Feminino", DateOfBirth = "20/05/1990",
                Cep = "01001-000", Street = "Praça da Sé", Number = "100",
                Neighborhood = "Sé", City = "São Paulo", State = "SP",
                Observations = "Primeiro contato realizado.", Status = "Lead"
            };
            using (var memoryStream = new MemoryStream())
            using (var streamWriter = new StreamWriter(memoryStream, System.Text.Encoding.UTF8))
            using (var csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture))
            {
                foreach (var header in headers)
                {
                    csvWriter.WriteField(header);
                }
                csvWriter.NextRecord();
                csvWriter.WriteRecord(exampleRecord);
                csvWriter.NextRecord();
                streamWriter.Flush();
                return File(memoryStream.ToArray(), "text/csv", "modelo_importacao_contatos.csv");
            }
        }

        [HttpPost("import")]
        public async Task<IActionResult> ImportContacts(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("Nenhum arquivo enviado.");
            }

            var newContacts = new List<Contact>();
            var validationErrors = new List<string>();
            int rowNumber = 1;

            try
            {
                var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    PrepareHeaderForMatch = args => args.Header.Replace("*", "").Trim()
                };
                using (var reader = new StreamReader(file.OpenReadStream()))
                using (var csv = new CsvReader(reader, config))
                {
                    csv.Context.RegisterClassMap<ContactImportDtoMap>();
                    var records = csv.GetRecords<ContactImportDto>().ToList();

                    foreach (var record in records)
                    {
                        rowNumber++;
                        if (record.Name == "Ex: Ana Silva")
                        {
                            continue;
                        }
                        if (string.IsNullOrWhiteSpace(record.Name) || string.IsNullOrWhiteSpace(record.PhoneNumber))
                        {
                            validationErrors.Add($"Linha {rowNumber}: Nome e Telefone são obrigatórios.");
                            continue;
                        }
                        newContacts.Add(new Contact
                        {
                            Name = record.Name,
                            PhoneNumber = record.PhoneNumber,
                            Email = record.Email,
                            Source = record.Source,
                            Gender = record.Gender,
                            DateOfBirth = record.DateOfBirth.HasValue ? DateTime.SpecifyKind(record.DateOfBirth.Value, DateTimeKind.Utc) : null,
                            Cep = record.Cep,
                            Street = record.Street,
                            Number = record.Number,
                            Neighborhood = record.Neighborhood,
                            City = record.City,
                            State = record.State,
                            Observations = record.Observations,
                            Status = string.IsNullOrWhiteSpace(record.Status) ? "Lead" : record.Status,
                            CreatedAt = DateTime.UtcNow
                        });
                    }
                }
                if (validationErrors.Any())
                {
                    return BadRequest(new { message = "Ocorreram erros de validação no arquivo.", errors = validationErrors });
                }
                await _context.Contacts.AddRangeAsync(newContacts);
                await _context.SaveChangesAsync();
                return Ok(new { message = $"{newContacts.Count} contatos importados com sucesso." });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return StatusCode(500, $"Erro interno ao processar o arquivo. Verifique o formato do CSV.");
            }
        }
    }
}