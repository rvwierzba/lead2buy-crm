using System.Security.Claims;
using System.Text.Json;
using Lead2Buy.API.Data;
using Lead2Buy.API.Dtos.Chatbot;
using Lead2Buy.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis; 

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
        private readonly IConnectionMultiplexer _redis; // 2. Injetando o Redis

        public ChatbotController(
            IHttpClientFactory httpClientFactory, 
            AppDbContext context, 
            ILogger<ChatbotController> logger, 
            IConnectionMultiplexer redis) // 3. Recebendo no construtor
        {
            _httpClientFactory = httpClientFactory;
            _context = context;
            _logger = logger;
            _redis = redis; // 4. Atribuindo
        }
        
        // O método de conversa simples (sem anexo) continua o mesmo
        [HttpPost("converse")]
        public async Task<IActionResult> Converse(ChatRequestDto request)
        {
            // ... (Este método não precisa de alteração)
            var httpClient = _httpClientFactory.CreateClient("OllamaClient");
            var ollamaRequest = new OllamaRequestDto
            {
                Messages = new List<OllamaMessageDto> { new OllamaMessageDto { Content = request.Prompt } }
            };
            var ollamaUrl = "http://ollama:11434/api/chat";
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

        // --- MÉTODO COM ANEXO TOTALMENTE REFATORADO ---
        [HttpPost("converse-with-attachment")]
        public async Task<IActionResult> ConverseWithAttachment([FromForm] string prompt, [FromForm] IFormFile file)
        {
            _logger.LogInformation("Endpoint ConverseWithAttachment iniciado para criar um novo ChatJob.");

            if (file == null || file.Length == 0)
            {
                return BadRequest("Nenhum arquivo enviado.");
            }

            // 1. Salva o arquivo em uma pasta temporária (como antes)
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

            // 2. Cria o registro do Job no banco de dados (como antes)
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

            // 3. A GRANDE MUDANÇA: Publica o ID do Job na fila do Redis
            var publisher = _redis.GetSubscriber();
            await publisher.PublishAsync("ocr_jobs_channel", newJob.Id.ToString());
            _logger.LogInformation("Job ID {JobId} publicado na fila 'ocr_jobs_channel'.", newJob.Id);

            // 4. Retorna a resposta imediata para o usuário
            return Accepted(new { 
                message = "Sua solicitação foi recebida e está na fila para processamento. Você será notificado quando estiver pronta.", 
                jobId = newJob.Id 
            });
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