# syntax=docker/dockerfile:1

# Backend build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS backend-build
WORKDIR /src

COPY backend/AmazonBestSellers.Domain/AmazonBestSellers.Domain.csproj backend/AmazonBestSellers.Domain/
COPY backend/AmazonBestSellers.Application/AmazonBestSellers.Application.csproj backend/AmazonBestSellers.Application/
COPY backend/AmazonBestSellers.Infrastructure/AmazonBestSellers.Infrastructure.csproj backend/AmazonBestSellers.Infrastructure/
COPY backend/AmazonBestSellers.API/AmazonBestSellers.API.csproj backend/AmazonBestSellers.API/

# Use BuildKit cache mount for NuGet packages (speeds up rebuilds by ~30%)
RUN --mount=type=cache,target=/root/.nuget/packages \
    dotnet restore backend/AmazonBestSellers.API/AmazonBestSellers.API.csproj

COPY backend/ backend/

RUN dotnet publish backend/AmazonBestSellers.API/AmazonBestSellers.API.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

RUN dotnet tool install --global dotnet-ef --version 9.0.0

# Frontend build
FROM node:20-alpine AS frontend-build
WORKDIR /app

COPY frontend/package*.json ./
# Use BuildKit cache mount for npm packages (speeds up rebuilds by ~50%)
RUN --mount=type=cache,target=/root/.npm \
    npm ci --prefer-offline --no-audit

COPY frontend/ .
RUN npm run build -- --configuration=production

# Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0

# Install curl for healthcheck (optimized with --no-install-recommends to reduce image size)
RUN apt-get update && apt-get install -y --no-install-recommends curl && rm -rf /var/lib/apt/lists/*

# Copy dotnet-ef tools from backend-build instead of installing again (avoids timeout issues)
COPY --from=backend-build /root/.dotnet/tools /root/.dotnet/tools
ENV PATH="${PATH}:/root/.dotnet/tools"

WORKDIR /app

COPY --from=backend-build /app/publish ./
COPY --from=frontend-build /app/dist/amazon-bestsellers-app ./wwwroot/
COPY --from=backend-build /src/backend/AmazonBestSellers.Infrastructure ./Infrastructure/

RUN mkdir -p /app/logs && chmod 777 /app/logs

COPY docker-entrypoint.sh ./
RUN chmod +x docker-entrypoint.sh

EXPOSE 5196

HEALTHCHECK --interval=30s --timeout=10s --start-period=180s --retries=3 \
    CMD curl -f http://localhost:5196/health || exit 1

ENTRYPOINT ["./docker-entrypoint.sh"]
