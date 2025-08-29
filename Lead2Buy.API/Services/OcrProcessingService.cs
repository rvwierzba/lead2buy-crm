using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Lead2Buy.API.Data;
using Lead2Buy.API.Dtos.Chatbot;
using Lead2Buy.API.Hubs;
using Lead2Buy.API.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

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
            _logger.LogInformation("Serviço de Processamento de OCR está iniciando.");

            while (!stoppingToken.IsCancellationRequested)
            {
                await using (var scope = _serviceProvider.CreateAsyncScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    var hubContext = scope.ServiceProvider.GetRequiredService<IHubContext<NotificationHub>>();
                    var httpClientFactory = scope.ServiceProvider.GetRequiredService<IHttpClientFactory>();

                    var pendingJob = await dbContext.ChatJobs.FirstOrDefaultAsync(j => j.Status == JobStatus.Pending, stoppingToken);

                    if (pendingJob != null)
                    {
                        _logger.LogInformation("Job de OCR encontrado (ID: {JobId}). Iniciando processamento.", pendingJob.Id);
                        pendingJob.Status = JobStatus.Processing;
                        await dbContext.SaveChangesAsync(stoppingToken);

                        string finalAiResponse = "Não foi possível processar a solicitação.";

                        try
                        {
                            // ETAPA 1: Extrair o texto do arquivo usando OCR.space (Rápido)
                            string extractedText = await ExtractTextWithOcrSpace(pendingJob, httpClientFactory, stoppingToken);

                            if (!string.IsNullOrEmpty(extractedText))
                            {
                                // ETAPA 2: Chamar a IA para RESUMIR o texto (Mais Rápido)
                                _logger.LogInformation("Enviando texto para sumarização pela IA.");
                                string summary = await GetAiSummary(extractedText, httpClientFactory, stoppingToken);

                                // ETAPA 3: Chamar a IA com o RESUMO para ANÁLISE (Muito Mais Rápido)
                                _logger.LogInformation("Enviando resumo e prompt do usuário para análise final da IA.");
                                finalAiResponse = await GetAiAnalysis(summary, pendingJob.UserPrompt, httpClientFactory, stoppingToken);
                            }
                            else
                            {
                                finalAiResponse = "Não foi possível extrair conteúdo do arquivo para análise.";
                            }
                            
                            pendingJob.Status = JobStatus.Completed;
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Falha no processamento do Job de OCR/IA (ID: {JobId})", pendingJob.Id);
                            pendingJob.Status = JobStatus.Failed;
                            finalAiResponse = $"Ocorreu um erro técnico ao processar sua solicitação: {ex.Message}";
                        }
                        
                        // Salva o resultado final no banco
                        pendingJob.AiResponse = finalAiResponse;
                        pendingJob.CompletedAt = DateTime.UtcNow;
                        await dbContext.SaveChangesAsync(stoppingToken);
                        _logger.LogInformation("Job (ID: {JobId}) finalizado com status: {Status}.", pendingJob.Id, pendingJob.Status);
                        
                        // Envia a notificação para o frontend
                        await hubContext.Clients.All.SendAsync("ChatResponseReceived", new { jobId = pendingJob.Id, response = pendingJob.AiResponse }, stoppingToken);
                    }
                }
                
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }

            _logger.LogInformation("Serviço de Processamento de OCR está parando.");
        }

        // --- MÉTODOS AUXILIARES ---

        private async Task<string> ExtractTextWithOcrSpace(ChatJob job, IHttpClientFactory httpClientFactory, CancellationToken stoppingToken)
        {
            var fileBytes = await File.ReadAllBytesAsync(job.FilePath, stoppingToken);
            var ocrApiKey = _configuration["OcrApiKey"];

            if (string.IsNullOrEmpty(ocrApiKey)) throw new Exception("Chave da API do OCR.space não configurada.");

            using var httpClient = httpClientFactory.CreateClient();
            using var form = new MultipartFormDataContent();
            using var fileContent = new ByteArrayContent(fileBytes);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");

            form.Add(new StringContent(ocrApiKey), "apikey");
            form.Add(new StringContent("por"), "language");
            form.Add(fileContent, "file", Path.GetFileName(job.FilePath));

            var ocrResponse = await httpClient.PostAsync("https://api.ocr.space/parse/image", form, stoppingToken);

            if (!ocrResponse.IsSuccessStatusCode)
            {
                var errorBody = await ocrResponse.Content.ReadAsStringAsync(stoppingToken);
                throw new Exception($"Erro ao chamar a API do OCR.space: {ocrResponse.ReasonPhrase} - {errorBody}");
            }

            var jsonResponse = await ocrResponse.Content.ReadFromJsonAsync<JsonElement>(cancellationToken: stoppingToken);
            return jsonResponse.GetProperty("ParsedResults")[0].GetProperty("ParsedText").GetString() ?? "";
        }

        private async Task<string> GetAiSummary(string textToSummarize, IHttpClientFactory httpClientFactory, CancellationToken stoppingToken)
        {
            var prompt = $"Resuma o texto a seguir em seus pontos-chave, com no máximo 200 palavras: \n\n{textToSummarize.Substring(0, Math.Min(4000, textToSummarize.Length))}";
            return await CallOllamaApi(prompt, httpClientFactory, stoppingToken);
        }

        private async Task<string> GetAiAnalysis(string summary, string userPrompt, IHttpClientFactory httpClientFactory, CancellationToken stoppingToken)
        {
            var systemPrompt = "Você é o LeadBot, um assistente de IA integrado ao CRM Lead2Buy. Sua especialidade é analisar dados e fornecer insights. A palavra 'conversão' sempre se refere a vendas.";
            var finalPrompt = $"{systemPrompt}\n\nCom base no seguinte resumo de um documento: '{summary}'\n\nResponda à pergunta do usuário: '{userPrompt}'";
            return await CallOllamaApi(finalPrompt, httpClientFactory, stoppingToken);
        }

        private async Task<string> CallOllamaApi(string prompt, IHttpClientFactory httpClientFactory, CancellationToken stoppingToken)
        {
            var ollamaHttpClient = httpClientFactory.CreateClient("OllamaClient");
            var ollamaRequest = new OllamaRequestDto 
            { 
                Model = "phi4-mini:q4_0", // Usando a versão otimizada
                Messages = new List<OllamaMessageDto> { new OllamaMessageDto { Content = prompt } } 
            };
            var ollamaUrl = "http://ollama:11434/api/chat"; // Usando a rede interna do Docker

            var aiResponse = await ollamaHttpClient.PostAsJsonAsync(ollamaUrl, ollamaRequest, stoppingToken);

            if (!aiResponse.IsSuccessStatusCode)
            {
                var errorContent = await aiResponse.Content.ReadAsStringAsync(stoppingToken);
                _logger.LogError("Erro da API Ollama: {Error}", errorContent);
                return "Ocorreu um erro ao se comunicar com o modelo de IA.";
            }

            var ollamaResponseJson = await aiResponse.Content.ReadFromJsonAsync<JsonElement>(cancellationToken: stoppingToken);
            return ollamaResponseJson.GetProperty("message").GetProperty("content").GetString() ?? "A IA não retornou uma resposta.";
        }
    }
}