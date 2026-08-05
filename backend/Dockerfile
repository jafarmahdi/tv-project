# syntax=docker/dockerfile:1

## ---- Build stage ----
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project files first so `dotnet restore` is cached independently of source changes.
COPY Directory.Build.props .
COPY src/WatchLog.Domain/WatchLog.Domain.csproj src/WatchLog.Domain/
COPY src/WatchLog.Application/WatchLog.Application.csproj src/WatchLog.Application/
COPY src/WatchLog.Infrastructure/WatchLog.Infrastructure.csproj src/WatchLog.Infrastructure/
COPY src/WatchLog.Api/WatchLog.Api.csproj src/WatchLog.Api/
RUN dotnet restore src/WatchLog.Api/WatchLog.Api.csproj

COPY src/ src/
RUN dotnet publish src/WatchLog.Api/WatchLog.Api.csproj -c Release -o /app --no-restore

## ---- Runtime stage ----
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

RUN apt-get update && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/* \
    && addgroup --system --gid 1000 watchlog && adduser --system --uid 1000 --gid 1000 watchlog

COPY --from=build /app .
USER watchlog

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

HEALTHCHECK --interval=30s --timeout=5s --start-period=15s --retries=3 \
    CMD curl -fsS http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "WatchLog.Api.dll"]
