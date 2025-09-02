# Estágio 1: Build da API .NET
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-api
WORKDIR /src
COPY ["Lead2Buy.sln", "."]
COPY ["Lead2Buy.API/Lead2Buy.API.csproj", "Lead2Buy.API/"]
RUN dotnet restore "Lead2Buy.sln"
COPY . .
WORKDIR "/src/Lead2Buy.API"
RUN dotnet publish "Lead2Buy.API.csproj" -c Release -o /app/publish

# Estágio 2: Imagem final com Ollama e a API Juntos
FROM ollama/ollama

# Instala o .NET Runtime 8.0 e outras ferramentas
RUN apt-get update && apt-get install -y dotnet-runtime-8.0 curl procps && rm -rf /var/lib/apt/lists/*

# Copia a API já compilada do estágio de build
WORKDIR /app
COPY --from=build-api /app/publish .

# Copia o script que vai iniciar os dois serviços
COPY start.sh /start.sh
RUN chmod +x /start.sh

# Expõe a porta da API
EXPOSE 10000

# O comando que inicia tudo
ENTRYPOINT ["/start.sh"]