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
builder.Services.AddCors(options => {
    options.AddPolicy("AllowVueApp", policy => {
        policy.WithOrigins("http://localhost:8080", "http://localhost:5173", corsOrigin ?? "")
              .AllowAnyHeader().AllowAnyMethod().AllowCredentials();
    });
});
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddSingleton<IConnectionMultiplexer>(sp => 
{
    var redisConnectionString = builder.Configuration.GetConnectionString("RedisConnectionString");
    if (string.IsNullOrEmpty(redisConnectionString))
    {
        // Isso evita que a aplicação quebre se a variável de ambiente ainda não estiver pronta
        // O serviço de background vai esperar.
        Console.WriteLine("String de conexão do Redis não encontrada, aguardando o serviço iniciar...");
        // Retorna um objeto nulo temporariamente, a lógica de migração vai segurar a inicialização
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

// --- LÓGICA DE MIGRAÇÃO AUTOMÁTICA NA INICIALIZAÇÃO ---
// Este bloco resolve o erro da API iniciar antes do banco de dados.
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        using var scope4db = app.Services.CreateScope();
        var db = scope4db.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Database.Migrate(); 
        Console.WriteLine("Aguardando o banco de dados ficar pronto e executando migrações...");
        Console.WriteLine("Migrações do banco de dados aplicadas com sucesso.");
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Ocorreu um erro fatal durante a migração do banco de dados.");
        throw; // Falha o deploy se a migração não funcionar
    }
}
// --- FIM DA LÓGICA ---

app.Run();