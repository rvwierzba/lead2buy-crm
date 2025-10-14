FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

# instala cliente psql para o start.sh poder rodar ALTER USER
RUN apt-get update && apt-get install -y postgresql-client && rm -rf /var/lib/apt/lists/*

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Lead2Buy.API.csproj", "."]
RUN dotnet restore "./Lead2Buy.API.csproj"
COPY . .
RUN dotnet build "Lead2Buy.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Lead2Buy.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY start.sh .
RUN chmod +x start.sh
ENTRYPOINT ["./start.sh"]
