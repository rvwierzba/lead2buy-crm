# Estágio 1: Build da API .NET
# Mudamos o nome do estágio para 'builder' para evitar conflitos no Render
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS builder
WORKDIR /src

# Copia os arquivos de projeto primeiro para otimizar o cache do Docker
COPY ["Lead2Buy.sln", "."]
COPY ["Lead2Buy.API/Lead2Buy.API.csproj", "Lead2Buy.API/"]

# Restaura as dependências
RUN dotnet restore "Lead2Buy.sln"

# Copia o resto do código fonte
COPY . .

# Publica a aplicação
RUN dotnet publish "Lead2Buy.API/Lead2Buy.API.csproj" -c Release -o /app/publish --no-restore


# Estágio 2: Imagem Final de Produção
FROM mcr.microsoft.com/dotnet/aspnet:8.0

# Instala o Ollama
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*
RUN curl -fsSL https://ollama.com/install.sh | sh

# Copia a API já compilada do estágio 'builder'
WORKDIR /app
COPY --from=builder /app/publish .

# Copia e dá permissão ao script de inicialização
COPY start.sh /start.sh
RUN chmod +x /start.sh

EXPOSE 10000
ENTRYPOINT ["/start.sh"]