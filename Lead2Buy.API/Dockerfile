# Estágio 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Lead2Buy.API/Lead2Buy.API.csproj", "Lead2Buy.API/"]
RUN dotnet restore "Lead2Buy.API/Lead2Buy.API.csproj"
COPY . .
WORKDIR "/src/Lead2Buy.API"
RUN dotnet publish "Lead2Buy.API.csproj" -c Release -o /app/publish

# Estágio 2: Final
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
# Adiciona a ferramenta 'dotnet-ef' à imagem final para rodar as migrações
RUN dotnet tool install --global dotnet-ef --version 8.0.0
ENV PATH="$PATH:/root/.dotnet/tools"
ENTRYPOINT ["dotnet", "Lead2Buy.API.dll"]