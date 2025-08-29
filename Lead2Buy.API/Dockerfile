# Estágio 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia o arquivo .csproj da pasta da API para uma pasta correspondente no container
COPY ["Lead2Buy.API/Lead2Buy.API.csproj", "Lead2Buy.API/"]

# Restaura as dependências do projeto que acabamos de copiar
RUN dotnet restore "Lead2Buy.API/Lead2Buy.API.csproj"

# Copia todo o resto do código fonte
COPY . .
WORKDIR "/src/Lead2Buy.API"
RUN dotnet publish "Lead2Buy.API.csproj" -c Release -o /app/publish

# Estágio 2: Final
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

# Instala as dependências do Tesseract no Linux (Debian)
RUN apt-get update && apt-get install -y \
    libgdiplus \
    libleptonica-dev \
    tesseract-ocr \
    tesseract-ocr-por \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Lead2Buy.API.dll"]