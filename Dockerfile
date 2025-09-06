# Estágio 1: Build da API .NET
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Lead2Buy.sln", "."]
COPY ["Lead2Buy.API/Lead2Buy.API.csproj", "Lead2Buy.API/"]
RUN dotnet restore "Lead2Buy.sln"
COPY . .
WORKDIR "/src/Lead2Buy.API"
RUN dotnet publish "Lead2Buy.API.csproj" -c Release -o /app/publish --no-restore

# Estágio 2: Imagem Final de Produção
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Lead2Buy.API.dll"]