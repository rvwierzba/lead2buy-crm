# Etapa de build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar csproj e restaurar dependências
COPY *.sln .
COPY Lead2Buy.Api/*.csproj ./Lead2Buy.Api/
RUN dotnet restore

# Copiar todo o código e buildar
COPY . .
WORKDIR /src/Lead2Buy.Api
RUN dotnet publish -c Release -o /app/publish

# Etapa final (runtime)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Lead2Buy.Api.dll"]
