using System.Security.Claims;
using System.Text.Json;
using Lead2Buy.API.Data;
using Lead2Buy.API.Dtos.Chatbot;
using Lead2Buy.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lead2Buy.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ChatbotController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly AppDbContext _context;
        private readonly ILogger<ChatbotController> _logger;

        public ChatbotController(IHttpClientFactory httpClientFactory, AppDbContext context, ILogger<ChatbotController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _context = context;
            _logger = logger;
        }
        
        // --- ESTE MÉTODO É PARA CONVERSAS SIMPLES, SEM ARQUIVOS ---
        [HttpPost("converse")]
        public async Task<IActionResult> Converse(ChatRequestDto request)
        {
            var httpClient = _httpClientFactory.CreateClient("OllamaClient");
            var ollamaRequest = new OllamaRequestDto
            {
                Messages = new List<OllamaMessageDto> { new OllamaMessageDto { Content = request.Prompt } }
            };
            var ollamaUrl = "http://host.docker.internal:11434/api/chat";

            try
            {
                var response = await httpClient.PostAsJsonAsync(ollamaUrl, ollamaRequest);
                if (response.IsSuccessStatusCode)
                {
                    var ollamaResponse = await response.Content.ReadFromJsonAsync<JsonElement>();
                    var messageContent = ollamaResponse.GetProperty("message").GetProperty("content").GetString();
                    return Ok(new { response = messageContent });
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return StatusCode((int)response.StatusCode, $"Erro na comunicação com a IA: {errorContent}");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno: {ex.Message}");
            }
        }

        // --- ESTE MÉTODO É PARA CRIAR O JOB QUANDO HÁ UM ANEXO ---
        [HttpPost("converse-with-attachment")]
        public async Task<IActionResult> ConverseWithAttachment([FromForm] string prompt, [FromForm] IFormFile file)
        {
            _logger.LogInformation("Endpoint ConverseWithAttachment iniciado para criar um novo ChatJob.");

            if (file == null || file.Length == 0)
            {
                return BadRequest("Nenhum arquivo enviado.");
            }

            var uploadsFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "TempUploads");
            if (!Directory.Exists(uploadsFolderPath))
            {
                Directory.CreateDirectory(uploadsFolderPath);
            }
            var fileName = $"{Guid.NewGuid()}_{file.FileName}";
            var filePath = Path.Combine(uploadsFolderPath, fileName);

            await using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            _logger.LogInformation("Arquivo salvo em: {FilePath}", filePath);

            var userId = GetUserId();
            var newJob = new ChatJob
            {
                UserPrompt = prompt,
                FilePath = filePath,
                Status = JobStatus.Pending,
                UserId = userId
            };

            _context.ChatJobs.Add(newJob);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Novo ChatJob criado com ID: {JobId}", newJob.Id);

            return Accepted(new { message = "Sua solicitação foi recebida e está sendo processada.", jobId = newJob.Id });
        }
        
        private int GetUserId()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString))
            {
                throw new Exception("ID do usuário não encontrado no token.");
            }
            return int.Parse(userIdString);
        }
    }
}