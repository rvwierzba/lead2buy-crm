echo '# Estágio de build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia o arquivo .csproj e restaura as dependências
COPY ["Lead2Buy.API.csproj", "."]
RUN dotnet restore "./Lead2Buy.API.csproj"

# Copia todo o resto do código da API
COPY . .

# Builda o projeto
RUN dotnet build "Lead2Buy.API.csproj" -c Release -o /app/build

# Estágio de publicação
FROM build AS publish
RUN dotnet publish "Lead2Buy.API.csproj" -c Release -o /app/publish

# Estágio final
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Lead2Buy.API.dll"]' > ./Lead2Buy.API/Dockerfile