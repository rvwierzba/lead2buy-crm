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
    ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("RedisConnectionString")));
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
try
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    Console.WriteLine("Aguardando o banco de dados ficar pronto...");
    // Tenta conectar e aplicar migrações. Isso pausa a inicialização até o DB estar pronto.
    dbContext.Database.Migrate();
    Console.WriteLine("Migrações do banco de dados aplicadas com sucesso.");
}
catch (Exception ex)
{
    Console.WriteLine($"Ocorreu um erro fatal durante a migração do banco de dados: {ex.Message}");
    // Em um cenário real, poderíamos decidir parar a aplicação aqui.
    // Por enquanto, vamos logar e continuar.
}
// --- FIM DA LÓGICA DE MIGRAÇÃO ---

app.Run();