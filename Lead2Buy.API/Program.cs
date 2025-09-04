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
var connectionString = builder.Configuration["ConnectionStrings:DefaultConnection"];
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString(connectionString)));
builder.Services.AddSingleton<IConnectionMultiplexer>(sp => 
{
    var redisConnectionString = builder.Configuration.GetConnectionString("RedisConnectionString");
    if (string.IsNullOrEmpty(redisConnectionString))
    {
        Console.WriteLine("String de conexão do Redis não encontrada, aguardando o serviço iniciar...");
        return null;
    }
    return ConnectionMultiplexer.Connect(redisConnectionString);
});
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
app.UseHttpsRedirection();
app.UseCors("AllowVueApp");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHub<NotificationHub>("/notificationHub");

// --- LÓGICA DE MIGRAÇÃO ROBUSTA COM RETENTATIVAS ---
// Este bloco resolve o erro da API iniciar antes do banco de dados estar pronto.
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
        break; // Sai do loop se a migração for bem-sucedida
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Ocorreu um erro durante a migração do banco de dados na tentativa {Attempt}.", i + 1);
        if (i < maxRetries - 1)
        {
            logger.LogInformation("Aguardando {Delay} segundos antes da próxima tentativa.", retryDelay.Seconds);
            await Task.Delay(retryDelay); // Espera assíncrona
        }
        else
        {
            logger.LogCritical(ex, "Não foi possível conectar ao banco de dados após {MaxAttempts} tentativas. A aplicação será encerrada.", maxRetries);
            throw; // Lança a exceção para falhar o início da aplicação
        }
    }
}
// --- FIM DA LÓGICA ---

app.Run();