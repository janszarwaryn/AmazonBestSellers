#!/bin/bash
set -e

echo "Starting Amazon BestSellers..."

# Validate env vars
REQUIRED_VARS=("DB_HOST" "DB_PORT" "DB_NAME" "DB_USER" "DB_PASSWORD" "JWT_SECRET" "JWT_ISSUER" "JWT_AUDIENCE")
MISSING_VARS=()

for var in "${REQUIRED_VARS[@]}"; do
    if [ -z "${!var}" ]; then
        MISSING_VARS+=("$var")
    fi
done

if [ ${#MISSING_VARS[@]} -ne 0 ]; then
    echo "ERROR: Missing env vars:"
    printf '  - %s\n' "${MISSING_VARS[@]}"
    exit 1
fi

CONNECTION_STRING="Server=${DB_HOST};Port=${DB_PORT};Database=${DB_NAME};User=${DB_USER};Password=${DB_PASSWORD};"

echo "Connecting to database ${DB_HOST}:${DB_PORT}..."

MAX_RETRIES=30
RETRY_COUNT=0

while [ $RETRY_COUNT -lt $MAX_RETRIES ]; do
    if dotnet ef database update \
        --project ./Infrastructure/AmazonBestSellers.Infrastructure.csproj \
        --no-build \
        --connection "$CONNECTION_STRING" 2>/dev/null; then
        echo "✓ Migrations applied"
        break
    fi

    RETRY_COUNT=$((RETRY_COUNT + 1))
    if [ $RETRY_COUNT -lt $MAX_RETRIES ]; then
        echo "Retry $RETRY_COUNT/$MAX_RETRIES..."
        sleep 2
    fi
done

if [ $RETRY_COUNT -eq $MAX_RETRIES ]; then
    echo "ERROR: Database connection timeout"
    echo "Check: DB_HOST=${DB_HOST}, DB_PORT=${DB_PORT}, DB_NAME=${DB_NAME}"
    exit 1
fi

echo "Starting API on port 5196..."

exec dotnet AmazonBestSellers.API.dll
