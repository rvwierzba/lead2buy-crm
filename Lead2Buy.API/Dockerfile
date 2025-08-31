# Estágio 1: Build - Focado em restaurar dependências e compilar
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia o arquivo da solução (.sln) e os arquivos de projeto (.csproj)
# Esta é a maneira correta de garantir que todas as dependências sejam reconhecidas
COPY ["Lead2Buy.sln", "."]
COPY ["Lead2Buy.API/Lead2Buy.API.csproj", "Lead2Buy.API/"]
# Adicione aqui a cópia de QUALQUER OUTRO PROJETO .csproj que sua solução tenha

# Executa o restore no nível da solução para baixar TODOS os pacotes (incluindo IronOcr)
RUN dotnet restore "Lead2Buy.sln"

# Agora copia todo o resto do código fonte
COPY . .

# Define o diretório de trabalho para o projeto da API
WORKDIR "/src/Lead2Buy.API"

# Publica a aplicação, otimizada para Release
RUN dotnet publish "Lead2Buy.API.csproj" -c Release -o /app/publish --no-restore

# Estágio 2: Final - Imagem leve, apenas com o necessário para rodar
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Lead2Buy.API.dll"]