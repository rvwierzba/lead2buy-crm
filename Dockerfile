# Dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 10000

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Lead2Buy.API/Lead2Buy.API.csproj", "Lead2Buy.API/"]
RUN dotnet restore "Lead2Buy.API/Lead2Buy.API.csproj"
COPY . .
WORKDIR "/src/Lead2Buy.API"
RUN dotnet build "Lead2Buy.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Lead2Buy.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Lead2Buy.API.dll"]