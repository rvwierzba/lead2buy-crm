# Estágio 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia os arquivos de projeto/solução primeiro
COPY ["Lead2Buy.sln", "."]
COPY ["Lead2Buy.API/Lead2Buy.API.csproj", "Lead2Buy.API/"]

# Restaura as dependências para todos os projetos na solução
RUN dotnet restore "Lead2Buy.sln"

# Copia o resto do código fonte
COPY . .
WORKDIR "/src/Lead2Buy.API"

# Publica a aplicação
RUN dotnet publish "Lead2Buy.API.csproj" -c Release -o /app/publish --no-restore

# Estágio 2: Final
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Lead2Buy.API.dll"]