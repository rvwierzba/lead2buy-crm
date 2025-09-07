# Estágio de Build: usa o SDK completo do .NET para compilar a aplicação
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia o arquivo de projeto e restaura as dependências (otimiza o cache do Docker)
COPY ["Lead2Buy.API.csproj", "."]
RUN dotnet restore "Lead2Buy.API.csproj"

# Copia o resto dos arquivos do projeto e compila
COPY . .
WORKDIR "/src/."
RUN dotnet build "Lead2Buy.API.csproj" -c Release -o /app/build

# Estágio de Publicação: cria a versão final otimizada da aplicação
FROM build AS publish
RUN dotnet publish "Lead2Buy.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Estágio Final: usa uma imagem leve do .NET apenas com o necessário para rodar
# --- ALTERAÇÃO CRÍTICA AQUI ---
# A imagem base foi corrigida de 'nginx:alpine' para 'aspnet:8.0'
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
# --- FIM DA ALTERAÇÃO ---
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Lead2Buy.API.dll"]