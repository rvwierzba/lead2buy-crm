# Estágio 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copia os arquivos de projeto e restaura as dependências
COPY *.csproj .
# CORREÇÃO: Especifica o nome do projeto para o restore
RUN dotnet restore "Lead2Buy.API.csproj"

# Copia o resto do código fonte e publica a aplicação
COPY . .
RUN dotnet publish -c Release -o /app/publish

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