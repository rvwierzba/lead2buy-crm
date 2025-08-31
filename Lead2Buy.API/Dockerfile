# Estágio 1: Build (com o SDK completo)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Instala a ferramenta dotnet-ef AQUI, no ambiente de build
RUN dotnet tool install --global dotnet-ef --version 8.0.0

COPY ["Lead2Buy.API/Lead2Buy.API.csproj", "Lead2Buy.API/"]
RUN dotnet restore "Lead2Buy.API/Lead2Buy.API.csproj"
COPY . .
WORKDIR "/src/Lead2Buy.API"
RUN dotnet publish "Lead2Buy.API.csproj" -c Release -o /app/publish

# Estágio 2: Final (imagem leve de runtime)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

# Copia a ferramenta dotnet-ef JÁ INSTALADA do estágio de build para a imagem final
COPY --from=build /root/.dotnet/tools /root/.dotnet/tools
# Adiciona o diretório de ferramentas ao PATH do sistema para que os comandos sejam encontrados
ENV PATH="$PATH:/root/.dotnet/tools"

WORKDIR /app
COPY --from=build /app/publish .

# Copia e dá permissão ao nosso script de inicialização
COPY ["Lead2Buy.API/entrypoint.sh", "."]
RUN chmod +x ./entrypoint.sh

# Define o NOSSO SCRIPT como o ponto de entrada do container
ENTRYPOINT ["./entrypoint.sh"]