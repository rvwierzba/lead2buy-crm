using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Lead2Buy.API.Data;
using Lead2Buy.API.Dtos.Chatbot;
using Lead2Buy.API.Hubs;
using Lead2Buy.API.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

namespace Lead2Buy.API.Services
{
    public class OcrProcessingService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<OcrProcessingService> _logger;
        private readonly IConfiguration _configuration;

        public OcrProcessingService(IServiceProvider serviceProvider, ILogger<OcrProcessingService> logger, IConfiguration configuration)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Serviço de Processamento de OCR está se inscrevendo na fila Redis.");
            
            await using var scope = _serviceProvider.CreateAsyncScope();
            var redis = scope.ServiceProvider.GetRequiredService<IConnectionMultiplexer>();
            var subscriber = redis.GetSubscriber();

            // Inscrição no canal Redis com RedisChannel.Literal
            await subscriber.SubscribeAsync(RedisChannel.Literal("ocr_jobs_channel"), async (channel, jobIdValue) =>
            {
                if (int.TryParse(jobIdValue, out int jobId))
                {
                    _logger.LogInformation("Job {JobId} recebido da fila Redis. Iniciando processamento.", jobId);
                    await ProcessJob(jobId);
                }
            });

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(Timeout.Infinite, stoppingToken);
            }

            _logger.LogInformation("Serviço de Processamento de OCR está parando.");
        }

        private async Task ProcessJob(int jobId)
        {
            await using var scope = _serviceProvider.CreateAsyncScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var hubContext = scope.ServiceProvider.GetRequiredService<IHubContext<NotificationHub>>();
            var httpClientFactory = scope.ServiceProvider.GetRequiredService<IHttpClientFactory>();

            var job = await dbContext.ChatJobs.FindAsync(jobId);
            if (job == null || job.Status != JobStatus.Pending)
            {
                _logger.LogWarning("Job {JobId} não encontrado ou já processado.", jobId);
                return;
            }

            job.Status = JobStatus.Processing;
            await dbContext.SaveChangesAsync();

            string finalAiResponse;
            try
            {
                string extractedText = await ExtractTextWithOcrSpace(job, httpClientFactory);
                string summary = await GetAiSummary(extractedText, httpClientFactory);
                finalAiResponse = await GetAiAnalysis(summary, job.UserPrompt, httpClientFactory);
                job.Status = JobStatus.Completed;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Falha no processamento do Job de OCR/IA (ID: {JobId})", jobId);
                job.Status = JobStatus.Failed;
                finalAiResponse = $"Ocorreu um erro técnico ao processar sua solicitação: {ex.Message}";
            }
            
            job.AiResponse = finalAiResponse;
            job.CompletedAt = DateTime.UtcNow;
            await dbContext.SaveChangesAsync();
            _logger.LogInformation("Job (ID: {JobId}) finalizado com status: {Status}.", jobId, job.Status);

            await hubContext.Clients.All.SendAsync("ChatResponseReceived", new { jobId = job.Id, response = job.AiResponse });
        }

        // --- MÉTODOS AUXILIARES ---

        private async Task<string> ExtractTextWithOcrSpace(ChatJob job, IHttpClientFactory httpClientFactory)
        {
            if (string.IsNullOrEmpty(job.FilePath))
                throw new Exception("Caminho do arquivo não definido para o Job.");

            var fileBytes = await File.ReadAllBytesAsync(job.FilePath);
            var ocrApiKey = _configuration["OcrApiKey"];
            if (string.IsNullOrEmpty(ocrApiKey)) 
                throw new Exception("Chave da API do OCR.space não configurada.");

            using var httpClient = httpClientFactory.CreateClient();
            using var form = new MultipartFormDataContent();
            using var fileContent = new ByteArrayContent(fileBytes);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
            form.Add(new StringContent(ocrApiKey), "apikey");
            form.Add(new StringContent("por"), "language");
            form.Add(fileContent, "file", Path.GetFileName(job.FilePath));
            
            var ocrResponse = await httpClient.PostAsync("https://api.ocr.space/parse/image", form);
            if (!ocrResponse.IsSuccessStatusCode) 
                throw new Exception($"Erro na API do OCR.space: {await ocrResponse.Content.ReadAsStringAsync()}");

            var jsonResponse = await ocrResponse.Content.ReadFromJsonAsync<JsonElement>();
            return jsonResponse.GetProperty("ParsedResults")[0].GetProperty("ParsedText").GetString() ?? "";
        }

        private async Task<string> GetAiSummary(string textToSummarize, IHttpClientFactory httpClientFactory)
        {
            var prompt = $"Resuma o texto a seguir em seus pontos-chave, com no máximo 200 palavras: \n\n{textToSummarize.Substring(0, Math.Min(4000, textToSummarize.Length))}";
            return await CallOllamaApi(prompt, httpClientFactory);
        }

        private async Task<string> GetAiAnalysis(string summary, string userPrompt, IHttpClientFactory httpClientFactory)
        {
            var systemPrompt = "Você é o LeadBot, um assistente de IA integrado ao CRM Lead2Buy. Sua especialidade é analisar dados e fornecer insights. A palavra 'conversão' sempre se refere a vendas.";
            var finalPrompt = $"{systemPrompt}\n\nCom base no seguinte resumo de um documento: '{summary}'\n\nResponda à pergunta do usuário: '{userPrompt}'";
            return await CallOllamaApi(finalPrompt, httpClientFactory);
        }

        private async Task<string> CallOllamaApi(string prompt, IHttpClientFactory httpClientFactory)
        {
            var ollamaHttpClient = httpClientFactory.CreateClient("OllamaClient");
            var ollamaRequest = new OllamaRequestDto 
            { 
                Model = "phi4-mini:q4_0",
                Messages = new List<OllamaMessageDto> { new OllamaMessageDto { Content = prompt } } 
            };
            var ollamaUrl = "http://ollama:11434/api/chat";

            var aiResponse = await ollamaHttpClient.PostAsJsonAsync(ollamaUrl, ollamaRequest);
            if (!aiResponse.IsSuccessStatusCode)
            {
                var errorContent = await aiResponse.Content.ReadAsStringAsync();
                _logger.LogError("Erro da API Ollama: {Error}", errorContent);
                return "Ocorreu um erro ao se comunicar com o modelo de IA.";
            }

            var ollamaResponseJson = await aiResponse.Content.ReadFromJsonAsync<JsonElement>();
            return ollamaResponseJson.GetProperty("message").GetProperty("content").GetString() ?? "A IA não retornou uma resposta.";
        }
    }
}
