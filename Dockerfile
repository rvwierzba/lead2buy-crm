# Estágio 1: Build da API .NET
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-api
WORKDIR /src
COPY . .
RUN dotnet restore "Lead2Buy.sln"
RUN dotnet publish "Lead2Buy.API/Lead2Buy.API.csproj" -c Release -o /app/publish

# Estágio 2: Imagem Final de Produção
# Começamos com a imagem oficial da Microsoft para garantir que a API rode
FROM mcr.microsoft.com/dotnet/aspnet:8.0

# Instala curl e depois o Ollama
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*
RUN curl -fsSL https://ollama.com/install.sh | sh

# Copia a API já compilada do estágio de build
WORKDIR /app
COPY --from=build-api /app/publish .

# Copia e dá permissão ao script que vai iniciar os dois serviços
COPY start.sh /start.sh
RUN chmod +x /start.sh

# Expõe a porta 10000 para a API .NET
EXPOSE 10000

# Define o script de inicialização como o comando principal do contêiner
ENTRYPOINT ["/start.sh"]