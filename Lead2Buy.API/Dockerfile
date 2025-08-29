# Estágio 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copia os arquivos de projeto e restaura as dependências
COPY *.csproj .
RUN dotnet restore

# Copia o resto do código fonte e publica a aplicação
COPY . .
RUN dotnet publish -c Release -o /app/publish

# Estágio 2: Final
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

# --- INÍCIO DA CORREÇÃO DEFINITIVA ---
# Instala TODAS as dependências do Tesseract no Linux (Debian)
RUN apt-get update && apt-get install -y \
    libgdiplus \
    libleptonica-dev \
    tesseract-ocr \
    tesseract-ocr-por \
    && rm -rf /var/lib/apt/lists/*
# --- FIM DA CORREÇÃO DEFINITIVA ---

WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "Lead2Buy.API.dll"]