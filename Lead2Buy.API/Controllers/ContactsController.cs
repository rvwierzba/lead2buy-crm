using System.Globalization;
using System.IO;
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
using System.Collections.Generic;
using Lead2Buy.API.DTOs.Timeline;

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

        // GET /api/contacts?search=...&stageId=...
        [HttpGet]
        public async Task<IActionResult> GetContacts([FromQuery] string? search = null, [FromQuery] Guid? stageId = null)
        {
            var query = _context.Contacts
                .Include(c => c.FunnelStage)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.ToLowerInvariant();
                query = query.Where(c =>
                    c.Name.ToLower().Contains(term) ||
                    (!string.IsNullOrEmpty(c.PhoneNumber) && c.PhoneNumber.Contains(term)) ||
                    (!string.IsNullOrEmpty(c.Email) && c.Email.ToLower().Contains(term))
                );
            }

            if (stageId.HasValue)
            {
                query = query.Where(c => c.FunnelStageId == stageId.Value);
            }

            var contacts = await query
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

            return Ok(contacts);
        }

        // GET /api/contacts/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetContactById(Guid id)
        {
            var contact = await _context.Contacts
                .Include(c => c.Interactions)
                .Include(c => c.FunnelStage)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (contact == null) return NotFound("Contato não encontrado.");

            return Ok(contact);
        }

        // POST /api/contacts
        [HttpPost]
        public async Task<IActionResult> CreateContact(CreateContactDto request)
        {
            // etapa inicial: "Lead" (se existir)
            var leadStage = await _context.FunnelStages.FirstOrDefaultAsync(s => s.Name == "Lead");

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
                FunnelStageId = request.FunnelStageId ?? (leadStage?.Id ?? Guid.Empty),
                CreatedAt = DateTime.UtcNow
            };

            _context.Contacts.Add(newContact);
            await _context.SaveChangesAsync();

            await _hubContext.Clients.All.SendAsync("ReceiveNotification", $"Novo lead adicionado: {newContact.Name}");

            return CreatedAtAction(nameof(GetContactById), new { id = newContact.Id }, newContact);
        }

        // PUT /api/contacts/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateContact(Guid id, UpdateContactDto request)
        {
            var contact = await _context.Contacts.FindAsync(id);
            if (contact == null) return NotFound("Contato não encontrado.");

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

            if (request.FunnelStageId.HasValue)
                contact.FunnelStageId = request.FunnelStageId.Value;

            await _context.SaveChangesAsync();
            return Ok(contact);
        }

        // DELETE /api/contacts/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContact(Guid id)
        {
            var contact = await _context.Contacts.FindAsync(id);
            if (contact == null) return NotFound("Contato não encontrado.");

            _context.Contacts.Remove(contact);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // POST /api/contacts/{contactId}/interactions
        [HttpPost("{contactId}/interactions")]
        public async Task<IActionResult> AddInteraction(Guid contactId, Dtos.Interaction.CreateInteractionDto request)
        {
            var contact = await _context.Contacts.FindAsync(contactId);
            if (contact == null) return NotFound("Contato não encontrado.");

            var newInteraction = new Interaction
            {
                Type = request.Type,
                Notes = request.Notes,
                ContactId = contactId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Interactions.Add(newInteraction);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetContactById), new { id = newInteraction.Id }, newInteraction);
        }

        // GET /api/contacts/export
        [HttpGet("export")]
        public async Task<IActionResult> ExportContacts()
        {
            var contacts = await _context.Contacts
                .Include(c => c.FunnelStage)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

            // Projeção para export com nome da etapa
            var export = contacts.Select(c => new
            {
                c.Name,
                c.PhoneNumber,
                c.Email,
                c.Source,
                c.Gender,
                DateOfBirth = c.DateOfBirth?.ToString("yyyy-MM-dd"),
                c.Cep,
                c.Street,
                c.Number,
                c.Neighborhood,
                c.City,
                c.State,
                c.Observations,
                FunnelStage = c.FunnelStage?.Name ?? ""
            }).ToList();

            using var memoryStream = new MemoryStream();
            using var streamWriter = new StreamWriter(memoryStream, System.Text.Encoding.UTF8);
            using var csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture);

            await csvWriter.WriteRecordsAsync(export);
            await streamWriter.FlushAsync();

            return File(memoryStream.ToArray(), "text/csv", $"contatos_{DateTime.UtcNow:yyyyMMddHHmmss}.csv");
        }

        // GET /api/contacts/import-template
        [HttpGet("import-template")]
        public IActionResult DownloadImportTemplate()
        {
            var headers = new List<string>
            {
                "Name*", "PhoneNumber*", "Email", "Source", "Gender", "DateOfBirth",
                "Cep", "Street", "Number", "Neighborhood", "City", "State",
                "Observations", "FunnelStage"
            };

            var exampleRecord = new
            {
                Name = "Ex: Ana Silva",
                PhoneNumber = "Ex: 11987654321",
                Email = "ana.silva@email.com",
                Source = "Website",
                Gender = "Feminino",
                DateOfBirth = "1990-05-20",
                Cep = "01001-000",
                Street = "Praça da Sé",
                Number = "100",
                Neighborhood = "Sé",
                City = "São Paulo",
                State = "SP",
                Observations = "Primeiro contato realizado.",
                FunnelStage = "Lead"
            };

            using var memoryStream = new MemoryStream();
            using var streamWriter = new StreamWriter(memoryStream, System.Text.Encoding.UTF8);
            using var csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture);

            foreach (var header in headers)
                csvWriter.WriteField(header);
            csvWriter.NextRecord();

            csvWriter.WriteRecord(exampleRecord);
            csvWriter.NextRecord();

            streamWriter.Flush();
            return File(memoryStream.ToArray(), "text/csv", "modelo_importacao_contatos.csv");
        }

        // POST /api/contacts/import
        [HttpPost("import")]
        public async Task<IActionResult> ImportContacts(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Nenhum arquivo enviado.");

            var newContacts = new List<Contact>();
            var validationErrors = new List<string>();
            var rowNumber = 1;

            try
            {
                var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    PrepareHeaderForMatch = args => args.Header.Replace("*", "").Trim(),
                    MissingFieldFound = null,
                    HeaderValidated = null
                };

                using var reader = new StreamReader(file.OpenReadStream());
                using var csv = new CsvReader(reader, config);

                csv.Context.RegisterClassMap<ContactImportDtoMap>();
                var records = csv.GetRecords<ContactImportDto>().ToList();

                foreach (var record in records)
                {
                    rowNumber++;

                    // pula linha de exemplo
                    if (record.Name?.Trim() == "Ex: Ana Silva")
                        continue;

                    if (string.IsNullOrWhiteSpace(record.Name) || string.IsNullOrWhiteSpace(record.PhoneNumber))
                    {
                        validationErrors.Add($"Linha {rowNumber}: Nome e Telefone são obrigatórios.");
                        continue;
                    }

                    // Resolve etapa pelo nome (Status no CSV vira nome de FunnelStage)
                    FunnelStage? stage = null;
                    if (!string.IsNullOrWhiteSpace(record.Status))
                        stage = await _context.FunnelStages.FirstOrDefaultAsync(s => s.Name == record.Status);
                    if (stage == null)
                        stage = await _context.FunnelStages.FirstOrDefaultAsync(s => s.Name == "Lead");

                    var contact = new Contact
                    {
                        Name = record.Name,
                        PhoneNumber = record.PhoneNumber,
                        Email = record.Email,
                        Source = record.Source,
                        Gender = record.Gender,
                        DateOfBirth = record.DateOfBirth.HasValue
                            ? DateTime.SpecifyKind(record.DateOfBirth.Value, DateTimeKind.Utc)
                            : null,
                        Cep = record.Cep,
                        Street = record.Street,
                        Number = record.Number,
                        Neighborhood = record.Neighborhood,
                        City = record.City,
                        State = record.State,
                        Observations = record.Observations,
                        FunnelStageId = stage?.Id ?? Guid.Empty,
                        CreatedAt = DateTime.UtcNow
                    };

                    newContacts.Add(contact);
                }

                if (validationErrors.Any())
                {
                    return BadRequest(new { message = "Ocorreram erros de validação no arquivo.", errors = validationErrors });
                }

                await _context.Contacts.AddRangeAsync(newContacts);
                await _context.SaveChangesAsync();

                return Ok(new { message = $"{newContacts.Count} contatos importados com sucesso." });
            }
            catch (Exception)
            {
                return StatusCode(500, "Erro interno ao processar o arquivo. Verifique o formato do CSV.");
            }
        }

        // pequena ajuda para List<Contact> pré-alocar memória quando o arquivo é grande
        private static List<Contact> ContactCapacityHint(int capacity = 256) => new List<Contact>(capacity);

        

        [HttpGet("{contactId}/timeline")]
        public async Task<IActionResult> GetTimeline(Guid contactId)
        {
            var interactions = await _context.Interactions
                .Where(i => i.ContactId == contactId)
                .Select(i => new TimelineItemDto
                {
                    Kind = "interaction",
                    When = i.CreatedAt,   // corrigido
                    Title = i.Type,
                    Details = i.Notes
                })
                .ToListAsync();

            var tasks = await _context.CrmTasks
                .Where(t => t.ContactId == contactId)
                .Select(t => new TimelineItemDto
                {
                    Kind = "task",
                    When = t.DueDate,     
                    Title = t.Title,
                    Details = t.Description
                })
                .ToListAsync();

            var activities = await _context.UserActivities
                .Where(a => a.ContactId == contactId)
                .Select(a => new TimelineItemDto
                {
                    Kind = "activity",
                    When = a.OccurredAt,
                    Title = a.ActionType,
                    Details = a.Details
                })
                .ToListAsync();

            var timeline = interactions
                .Concat(tasks)
                .Concat(activities)
                .OrderByDescending(x => x.When)
                .ToList();

            return Ok(timeline);
        }


    }
}
