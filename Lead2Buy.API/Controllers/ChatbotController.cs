using Lead2Buy.API.Dtos.Chatbot; 
using Lead2Buy.API.Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using StackExchange.Redis;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Lead2Buy.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ChatbotController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConnectionMultiplexer _redis;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly string _ollamaUrl;

        public ChatbotController(IHttpClientFactory httpClientFactory, IConnectionMultiplexer redis, IHubContext<NotificationHub> hubContext, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _redis = redis;
            _hubContext = hubContext;
            _ollamaUrl = configuration["OLLAMA_URL"] ?? "http://localhost:11434";
        }

        [HttpPost("converse")]
        public async Task<IActionResult> Converse([FromBody] ChatRequestDto request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Prompt))
            {
                return BadRequest("O prompt não pode ser vazio.");
            }

            // --- CORREÇÃO PRINCIPAL AQUI ---
            // O OllamaRequestDto espera uma lista de mensagens, não um prompt direto.
            var ollamaRequest = new OllamaRequestDto
            {
                Model = "phi4-mini:q4_0",
                Stream = false,
                Messages = new List<OllamaMessageDto>
                {
                    new OllamaMessageDto { Role = "user", Content = request.Prompt }
                }
            };
            // --- FIM DA CORREÇÃO ---

            var client = _httpClientFactory.CreateClient("OllamaClient");
            var response = await client.PostAsJsonAsync($"{_ollamaUrl}/api/chat", ollamaRequest); // Endpoint corrigido para /api/chat

            if (response.IsSuccessStatusCode)
            {
                var ollamaResponse = await response.Content.ReadFromJsonAsync<OllamaResponseDto>();
                return Ok(ollamaResponse);
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            return StatusCode((int)response.StatusCode, $"Erro ao se comunicar com o Ollama: {errorContent}");
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("converse-with-attachment")]
        public async Task<IActionResult> ConverseWithAttachment([FromForm] ChatRequestDto request, IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("Nenhum arquivo foi enviado.");
            }
            if (request == null || string.IsNullOrWhiteSpace(request.Prompt))
            {
                return BadRequest("O prompt não pode ser vazio.");
            }
            if (string.IsNullOrWhiteSpace(request.UserId))
            {
                return BadRequest("UserId é obrigatório.");
            }
            
            var uploadsFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
            if (!Directory.Exists(uploadsFolderPath))
            {
                Directory.CreateDirectory(uploadsFolderPath);
            }

            var filePath = Path.Combine(uploadsFolderPath, Guid.NewGuid().ToString() + Path.GetExtension(file.FileName));
            
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            
            var job = new
            {
                FilePath = filePath,
                Prompt = request.Prompt,
                UserId = request.UserId
            };

            var db = _redis.GetDatabase();
            await db.ListRightPushAsync("ocr-queue", JsonSerializer.Serialize(job));

            await _hubContext.Clients.User(request.UserId).SendAsync("JobStatusUpdate", "Seu arquivo foi recebido e está na fila para processamento.");

            return Ok(new { message = "Arquivo enviado e trabalho de OCR enfileirado com sucesso." });
        }
    }

    public class OllamaResponseDto
    {
        [JsonPropertyName("model")]
        public string? Model { get; set; } // Propriedade tornada nullable

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }

        // A resposta de /api/chat vem dentro de um objeto 'message'
        [JsonPropertyName("message")]
        public OllamaMessageDto? Message { get; set; }

        [JsonPropertyName("done")]
        public bool Done { get; set; }
    }
}