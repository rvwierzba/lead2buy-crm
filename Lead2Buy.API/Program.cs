using Lead2Buy.API.Data;
using Lead2Buy.API.Hubs;
using Lead2Buy.API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// --- Serviços ---
builder.Services.AddHttpClient("OllamaClient", client => { client.Timeout = TimeSpan.FromSeconds(300); });
var corsOrigin = builder.Configuration["CORS_ORIGIN"];
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowVueApp", policy =>
    {
        policy.WithOrigins("http://localhost:8080", "http://localhost:5173", corsOrigin ?? "")
              .AllowAnyHeader().AllowAnyMethod().AllowCredentials();
    });
});

// AQUI ESTÁ A CORREÇÃO:
var connectionString = builder.Configuration["ConnectionStrings:DefaultConnection"];
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString)); // Passamos a variável que já contém a string de conexão.

var redisConnectionString = builder.Configuration["RedisConnectionString"]; // Leitura direta da variável de ambiente

if (!string.IsNullOrEmpty(redisConnectionString))
{
    // Registra a conexão do Redis como um singleton
    builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConnectionString));
}
else
{
    Console.WriteLine("AVISO CRÍTICO: String de conexão do Redis não foi encontrada. O serviço de OCR não será iniciado e a aplicação pode não funcionar corretamente.");
    // Propositalmente não registramos o serviço para evitar o crash,
    // mas deixamos um aviso claro de que algo está errado.
}
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddHostedService<OcrProcessingService>();
builder.Services.AddControllers().AddJsonOptions(options => {
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});
builder.Services.AddSignalR();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => { /* ... Configuração do Swagger ... */ });
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => { /* ... Configuração do JWT ... */ });

var app = builder.Build();

// --- Pipeline HTTP ---
if (app.Environment.IsDevelopment()) { app.UseSwagger(); app.UseSwaggerUI(); }
// app.UseHttpsRedirection(); // Removido temporariamente para o reverse proxy
app.UseCors("AllowVueApp");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHub<NotificationHub>("/notificationHub");

// --- LÓGICA DE MIGRAÇÃO ROBUSTA COM RETENTATIVAS ---
var logger = app.Services.GetRequiredService<ILogger<Program>>();
var maxRetries = 5;
var retryDelay = TimeSpan.FromSeconds(10);

for (int i = 0; i < maxRetries; i++)
{
    try
    {
        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            logger.LogInformation("Tentando aplicar migrações do banco de dados (Tentativa {Attempt}/{MaxAttempts})...", i + 1, maxRetries);
            dbContext.Database.Migrate();
            logger.LogInformation("Migrações do banco de dados aplicadas com sucesso.");
        }
        break; 
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Ocorreu um erro durante a migração do banco de dados na tentativa {Attempt}.", i + 1);
        if (i < maxRetries - 1)
        {
            logger.LogInformation("Aguardando {Delay} segundos antes da próxima tentativa.", retryDelay.Seconds);
            await Task.Delay(retryDelay); 
        }
        else
        {
            logger.LogCritical(ex, "Não foi possível conectar ao banco de dados após {MaxAttempts} tentativas. A aplicação será encerrada.", maxRetries);
            throw; 
        }
    }
}
// --- FIM DA LÓGICA ---

app.Run();