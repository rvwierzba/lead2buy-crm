using Lead2Buy.API.Data;
using Lead2Buy.API.Hubs;
using Lead2Buy.API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// --- Adicionar serviços ao contêiner ---

builder.Services.AddHttpClient("OllamaClient", client =>
{
    client.Timeout = TimeSpan.FromSeconds(300);
});

var corsOrigin = builder.Configuration["CORS_ORIGIN"];
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowVueApp", policy =>
    {
        policy.WithOrigins("http://localhost:8080", "http://localhost:5173", corsOrigin ?? "")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// Registra o Redis (COMO JÁ ESTAVA NO SEU CÓDIGO CORRETO)
builder.Services.AddSingleton<IConnectionMultiplexer>(sp => 
{
    var redisConnectionString = builder.Configuration.GetConnectionString("RedisConnectionString");
    if (string.IsNullOrEmpty(redisConnectionString))
    {
        throw new InvalidOperationException("A string de conexão do Redis não foi encontrada.");
    }
    return ConnectionMultiplexer.Connect(redisConnectionString);
});

builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddHostedService<OcrProcessingService>();

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});

builder.Services.AddSignalR();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new() { Title = "Lead2Buy API", Version = "v1" });
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Por favor, insira 'Bearer ' seguido do seu token JWT",
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8
                .GetBytes(builder.Configuration.GetSection("Jwt:Key").Value!)),
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration.GetSection("Jwt:Issuer").Value,
            ValidateAudience = true,
            ValidAudience = builder.Configuration.GetSection("Jwt:Audience").Value
        };
    });

var app = builder.Build();

// --- Configurar o pipeline de requisições HTTP ---

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowVueApp");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHub<NotificationHub>("/notificationHub");

// --- LÓGICA DE MIGRAÇÃO AUTOMÁTICA NA INICIALIZAÇÃO (A PEÇA QUE FALTAVA) ---
// Este bloco resolve o erro da API iniciar antes do banco de dados.
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        // Aplica quaisquer migrações pendentes. Se o banco não existir, ele o cria.
        // Isso força a API a esperar o banco ficar pronto.
        context.Database.Migrate();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Ocorreu um erro durante a migração do banco de dados.");
        throw;
    }
}
// --- FIM DA LÓGICA ---

app.Run();