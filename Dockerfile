# Estágio 1: Build da API .NET
# Usamos a imagem com o SDK completo para compilar a aplicação
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-api
WORKDIR /src

# Copia TODOS os arquivos do código fonte de uma vez.
# Isso simplifica o processo e evita problemas de cache de pacotes.
COPY . .

# Restaura as dependências para toda a solução a partir da raiz.
RUN dotnet restore "Lead2Buy.sln"

# Publica a aplicação. Removemos o '--no-restore' para garantir que o publish
# verifique se todos os pacotes estão presentes antes de compilar.
RUN dotnet publish "Lead2Buy.API/Lead2Buy.API.csproj" -c Release -o /app/publish


# Estágio 2: Imagem Final de Produção
# Começamos com a imagem oficial da Microsoft para garantir que a API rode
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