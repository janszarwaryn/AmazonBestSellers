#!/bin/bash

set -e

echo "step 2: database migration"
echo ""

# load environment variables from .env file
if [ -f "../.env" ]; then
    source ../.env
elif [ -f ".env" ]; then
    source .env
else
    echo "error: .env file not found"
    echo "copy .env.example to .env and configure it"
    exit 1
fi

if ! command -v dotnet &> /dev/null; then
    echo "error: .NET SDK is not installed"
    echo "install .NET 9.0 SDK"
    exit 1
fi

echo "verifying .NET SDK version..."
DOTNET_VERSION=$(dotnet --version | cut -d'.' -f1)
if [ "$DOTNET_VERSION" -lt 9 ]; then
    echo "error: .NET 9.0 or higher is required"
    echo "current version: $(dotnet --version)"
    exit 1
fi
echo ".NET SDK version: $(dotnet --version)"

echo "trusting development certificates..."
dotnet dev-certs https --trust 2>/dev/null || echo "note: manual certificate trust may be required"

cd backend/AmazonBestSellers.API

echo "setting up Entity Framework Core tools..."
echo "clearing NuGet caches..."
dotnet nuget locals all --clear

echo "uninstalling existing dotnet-ef..."
dotnet tool uninstall --global dotnet-ef 2>/dev/null || true

echo "installing dotnet-ef..."
dotnet tool install --global dotnet-ef --version 9.0.0

echo "ensuring dotnet tools are in PATH..."
export PATH="$PATH:$HOME/.dotnet/tools"

if ! dotnet tool list -g | grep -q dotnet-ef; then
    echo "error: failed to install dotnet-ef"
    exit 1
fi

echo "verifying dotnet-ef command..."
if ! command -v dotnet-ef &> /dev/null; then
    echo "error: dotnet-ef command not found in PATH"
    echo "PATH: $PATH"
    exit 1
fi
echo "dotnet-ef is ready"

echo "checking for existing migrations..."
MIGRATION_EXISTS=$(dotnet ef migrations list --project ../AmazonBestSellers.Infrastructure 2>/dev/null | grep -c "InitialCreate" || true)

if [ "$MIGRATION_EXISTS" -eq 0 ]; then
    echo "creating database migration..."
    dotnet ef migrations add InitialCreate --project ../AmazonBestSellers.Infrastructure
else
    echo "migration already exists"
fi

echo ""
echo "applying migration to database..."
dotnet ef database update --project ../AmazonBestSellers.Infrastructure

echo ""
echo "verifying database tables..."
docker exec amazon-bestsellers-db mariadb -u ${DB_USER} -p${DB_PASSWORD} AmazonBestSellersDb -e "SHOW TABLES;"

echo ""
echo "database migration complete"

cd ../..
