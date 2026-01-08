#!/bin/bash

set -e

echo "Step 2: Database Migration"
echo ""

if ! command -v dotnet &> /dev/null; then
    echo "Error: .NET SDK is not installed"
    echo "Please install .NET 9.0 SDK"
    exit 1
fi

echo "Verifying .NET SDK version..."
DOTNET_VERSION=$(dotnet --version | cut -d'.' -f1)
if [ "$DOTNET_VERSION" -lt 9 ]; then
    echo "Error: .NET 9.0 or higher is required"
    echo "Current version: $(dotnet --version)"
    exit 1
fi
echo ".NET SDK version: $(dotnet --version)"

echo "Trusting development certificates..."
dotnet dev-certs https --trust 2>/dev/null || echo "Note: Manual certificate trust may be required"

cd backend/AmazonBestSellers.API

echo "Setting up Entity Framework Core tools..."
echo "Clearing NuGet caches..."
dotnet nuget locals all --clear

echo "Uninstalling existing dotnet-ef..."
dotnet tool uninstall --global dotnet-ef 2>/dev/null || true

echo "Installing dotnet-ef..."
dotnet tool install --global dotnet-ef --version 9.0.0

echo "Ensuring dotnet tools are in PATH..."
export PATH="$PATH:$HOME/.dotnet/tools"

if ! dotnet tool list -g | grep -q dotnet-ef; then
    echo "Error: Failed to install dotnet-ef"
    exit 1
fi

echo "Verifying dotnet-ef command..."
if ! command -v dotnet-ef &> /dev/null; then
    echo "Error: dotnet-ef command not found in PATH"
    echo "PATH: $PATH"
    exit 1
fi
echo "dotnet-ef is ready"

echo "Checking for existing migrations..."
MIGRATION_EXISTS=$(dotnet ef migrations list --project ../AmazonBestSellers.Infrastructure 2>/dev/null | grep -c "InitialCreate" || true)

if [ "$MIGRATION_EXISTS" -eq 0 ]; then
    echo "Creating database migration..."
    dotnet ef migrations add InitialCreate --project ../AmazonBestSellers.Infrastructure
else
    echo "Migration already exists"
fi

echo ""
echo "Applying migration to database..."
dotnet ef database update --project ../AmazonBestSellers.Infrastructure

echo ""
echo "Verifying database tables..."
docker exec amazon-bestsellers-db mariadb -u amazonuser -pamazonpass123 AmazonBestSellersDb -e "SHOW TABLES;"

echo ""
echo "Database migration complete"

cd ../..
