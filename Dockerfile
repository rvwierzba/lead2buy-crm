# Estágio 1: Build da API .NET
# Usamos a imagem com o SDK completo para compilar a aplicação
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-api
WORKDIR /src

# Copia e restaura as dependências de toda a solução
COPY ["Lead2Buy.sln", "."]
COPY ["Lead2Buy.API/Lead2Buy.API.csproj", "Lead2Buy.API/"]
RUN dotnet restore "Lead2Buy.sln"

# Copia o resto do código e publica a aplicação
COPY . .
WORKDIR "/src/Lead2Buy.API"
RUN dotnet publish "Lead2Buy.API.csproj" -c Release -o /app/publish --no-restore


# Estágio 2: Imagem Final de Produção
# COMEÇAMOS COM A IMAGEM OFICIAL DA MICROSOFT PARA GARANTIR QUE A API RODE
FROM mcr.microsoft.com/dotnet/aspnet:8.0

# Instala o Ollama dentro da imagem da Microsoft
RUN curl -fsSL https://ollama.com/install.sh | sh

# Copia a API já compilada do estágio de build
WORKDIR /app
COPY --from=build-api /app/publish .

# Copia o script que vai iniciar os dois serviços
COPY start.sh /start.sh
RUN chmod +x /start.sh

# Expõe a porta 10000 para a API .NET
EXPOSE 10000

# Define o script de inicialização como o comando principal do contêiner
ENTRYPOINT ["/start.sh"]