# ==========================================
# Estágio 1: Runtime (Base)
# ==========================================
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS base
WORKDIR /app
EXPOSE 80

# Instala dependencias necessarias no Alpine para .NET (Globalizacao e Timezone)
RUN apk add --no-cache \
    icu-data-full \
    icu-libs \
    tzdata \
    curl

# Configurações de Globalização e Timezone
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false \
    TZ=America/Sao_Paulo \
    ASPNETCORE_URLS=http://+:80 \
    ASPNETCORE_ENVIRONMENT=Production

# ==========================================
# Estágio 2: Build (SDK)
# ==========================================
FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /src

# Copiar arquivos de projeto primeiro para otimizar o cache das camadas (Docker Layer Caching)
COPY ["FCG.Game.API/FCG.Game.API.csproj", "FCG.Game.API/"]
COPY ["FCG.Game.Application/FCG.Game.Application.csproj", "FCG.Game.Application/"]
COPY ["FCG.Game.Domain/FCG.Game.Domain.csproj", "FCG.Game.Domain/"]
COPY ["FCG.Game.Infrastructure/FCG.Game.Infrastructure.csproj", "FCG.Game.Infrastructure/"]

# Restore das dependências
RUN dotnet restore "FCG.Game.API/FCG.Game.API.csproj"

# Copia o restante do código fonte
COPY . .

# Build da aplicacao
WORKDIR "/src/FCG.Game.API"
RUN dotnet build "FCG.Game.API.csproj" -c Release -o /app/build

# ==========================================
# Estágio 3: Publish
# ==========================================
FROM build AS publish
RUN dotnet publish "FCG.Game.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# ==========================================
# Estágio 4: Final (Imagem enxuta)
# ==========================================
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Health check (Agora funciona pois instalamos o curl no estágio 'base')
HEALTHCHECK --interval=30s --timeout=3s --start-period=40s --retries=3 \
    CMD curl --fail http://localhost:80/health || exit 1

ENTRYPOINT ["dotnet", "FCG.Game.API.dll"]