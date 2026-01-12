# syntax=docker/dockerfile:1

# Backend build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS backend-build
WORKDIR /src

COPY backend/AmazonBestSellers.Domain/AmazonBestSellers.Domain.csproj backend/AmazonBestSellers.Domain/
COPY backend/AmazonBestSellers.Application/AmazonBestSellers.Application.csproj backend/AmazonBestSellers.Application/
COPY backend/AmazonBestSellers.Infrastructure/AmazonBestSellers.Infrastructure.csproj backend/AmazonBestSellers.Infrastructure/
COPY backend/AmazonBestSellers.API/AmazonBestSellers.API.csproj backend/AmazonBestSellers.API/

RUN dotnet restore backend/AmazonBestSellers.API/AmazonBestSellers.API.csproj

COPY backend/ backend/

RUN dotnet publish backend/AmazonBestSellers.API/AmazonBestSellers.API.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

# Frontend build
FROM node:20-alpine AS frontend-build
WORKDIR /app

COPY frontend/package*.json ./
# Use BuildKit cache mount for npm packages (speeds up rebuilds by ~50%)
RUN --mount=type=cache,target=/root/.npm \
    npm ci --prefer-offline --no-audit

COPY frontend/ .
RUN npm run build -- --configuration=production

# Runtime with MariaDB
FROM mcr.microsoft.com/dotnet/aspnet:9.0

# Install curl, MariaDB server, and supervisor
RUN apt-get update && apt-get install -y --no-install-recommends \
    curl \
    mariadb-server \
    supervisor \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /app

# Copy application files
COPY --from=backend-build /app/publish ./
COPY --from=frontend-build /app/dist/amazon-bestsellers-app ./wwwroot/

# Create necessary directories
RUN mkdir -p /app/logs && chmod 777 /app/logs
RUN mkdir -p /var/run/mysqld && chown mysql:mysql /var/run/mysqld
RUN mkdir -p /var/log/supervisor

# Copy configuration files
COPY supervisord.conf /etc/supervisor/conf.d/supervisord.conf
COPY docker-entrypoint.sh ./
COPY init-mariadb.sh ./
RUN chmod +x docker-entrypoint.sh init-mariadb.sh

EXPOSE 5196

# Healthcheck sprawdza /health endpoint
HEALTHCHECK --interval=30s --timeout=10s --start-period=180s --retries=3 \
    CMD curl -f http://localhost:5196/health || exit 1

ENTRYPOINT ["./docker-entrypoint.sh"]
