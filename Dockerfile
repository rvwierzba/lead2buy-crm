# Estágio 1: Build da API .NET
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-api
WORKDIR /src
COPY . .
RUN dotnet restore "Lead2Buy.sln"
RUN dotnet publish "Lead2Buy.API/Lead2Buy.API.csproj" -c Release -o /app/publish

# Estágio 2: Imagem Final de Produção
FROM mcr.microsoft.com/dotnet/aspnet:8.0
RUN curl -fsSL https://ollama.com/install.sh | sh
WORKDIR /app
COPY --from=build-api /app/publish .
COPY start.sh /start.sh
RUN chmod +x /start.sh
EXPOSE 10000
ENTRYPOINT ["/start.sh"]