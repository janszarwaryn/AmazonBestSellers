#!/bin/bash

set -e

echo "step 1: docker setup"
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

if ! command -v docker &> /dev/null; then
    echo "error: docker is not installed"
    echo "install docker desktop from https://www.docker.com/products/docker-desktop"
    exit 1
fi

if ! docker info &> /dev/null; then
    echo "error: docker daemon is not running"
    echo "start docker desktop"
    exit 1
fi

echo "starting mariadb container..."
docker-compose up -d

echo ""
echo "waiting for database to be ready..."
sleep 10

RETRIES=10
until docker exec amazon-bestsellers-db mariadb -u ${DB_USER} -p${DB_PASSWORD} -e "SELECT 1" &> /dev/null || [ $RETRIES -eq 0 ]; do
    echo "waiting for database... ($RETRIES attempts remaining)"
    sleep 3
    RETRIES=$((RETRIES-1))
done

if [ $RETRIES -eq 0 ]; then
    echo "error: database failed to start"
    echo "check logs: docker logs amazon-bestsellers-db"
    exit 1
fi

echo ""
echo "docker setup complete"
echo "mariadb is ready"
