#!/bin/bash

set -e

echo "Step 1: Docker Setup"
echo ""

# Load environment variables from .env file
if [ -f "../.env" ]; then
    source ../.env
elif [ -f ".env" ]; then
    source .env
else
    echo "Error: .env file not found"
    echo "Please copy .env.example to .env and configure it"
    exit 1
fi

if ! command -v docker &> /dev/null; then
    echo "Error: Docker is not installed"
    echo "Please install Docker Desktop from https://www.docker.com/products/docker-desktop"
    exit 1
fi

if ! docker info &> /dev/null; then
    echo "Error: Docker daemon is not running"
    echo "Please start Docker Desktop"
    exit 1
fi

echo "Starting MariaDB container..."
docker-compose up -d

echo ""
echo "Waiting for database to be ready..."
sleep 10

RETRIES=10
until docker exec amazon-bestsellers-db mariadb -u ${DB_USER} -p${DB_PASSWORD} -e "SELECT 1" &> /dev/null || [ $RETRIES -eq 0 ]; do
    echo "Waiting for database... ($RETRIES attempts remaining)"
    sleep 3
    RETRIES=$((RETRIES-1))
done

if [ $RETRIES -eq 0 ]; then
    echo "Error: Database failed to start"
    echo "Check logs: docker logs amazon-bestsellers-db"
    exit 1
fi

echo ""
echo "Docker setup complete"
echo "MariaDB is ready"
