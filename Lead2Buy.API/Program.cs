using Lead2Buy.API.Data;
using Lead2Buy.API.Hubs;
using Lead2Buy.API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
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
builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));

var redisConnectionString = builder.Configuration["RedisConnectionString"];
if (!string.IsNullOrEmpty(redisConnectionString))
{
    builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConnectionString));
}
else
{
    Console.WriteLine("AVISO CRÍTICO: String de conexão do Redis não foi encontrada. O serviço de OCR não será iniciado.");
}

builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddHostedService<OcrProcessingService>();
builder.Services.AddControllers().AddJsonOptions(options => {
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});
builder.Services.AddSignalR();
builder.Services.AddEndpointsApiExplorer();

// --- CORREÇÃO DEFINITIVA DO SWAGGER COM AUTENTICAÇÃO ---
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Lead2Buy API", Version = "v1" });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Por favor, insira 'Bearer' seguido de um espaço e o token JWT",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement {
    {
        new OpenApiSecurityScheme
        {
            Reference = new OpenApiReference
            {
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer"
            }
        },
        new string[] {}
    }});
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

var app = builder.Build();

// --- Pipeline HTTP ---
app.UseSwagger();
app.UseSwaggerUI();

if (app.Environment.IsDevelopment())
{
    // Configurações de desenvolvimento
}

app.UseCors("AllowVueApp");
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHub<NotificationHub>("/notificationHub");

// --- Lógica de Migração ---
#region Database Migration
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
#endregion

app.Run();

