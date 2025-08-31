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

# Adiciona a ferramenta 'dotnet-ef' à imagem final
# A forma correta é instalar no build e copiar, mas vamos simplificar para garantir o deploy
RUN dotnet tool install --global dotnet-ef --version 8.0.0
ENV PATH="$PATH:/root/.dotnet/tools"

WORKDIR /app
COPY --from=build /app/publish .

# Copia e dá permissão ao nosso script de inicialização
COPY ["Lead2Buy.API/entrypoint.sh", "."]
RUN chmod +x ./entrypoint.sh

ENTRYPOINT ["./entrypoint.sh"]