#!/bin/bash

set -e

echo "Step 3: Install Dependencies"
echo ""

echo "Installing backend dependencies..."
if ! cd backend/AmazonBestSellers.API; then
    echo "Error: Cannot find backend directory"
    exit 1
fi

dotnet restore
if [ $? -ne 0 ]; then
    echo "Error: dotnet restore failed"
    exit 1
fi

dotnet build
if [ $? -ne 0 ]; then
    echo "Error: dotnet build failed"
    exit 1
fi

cd ../..

echo ""
echo "Installing frontend dependencies..."
if ! cd frontend; then
    echo "Error: Cannot find frontend directory"
    exit 1
fi

if [ ! -d "node_modules" ]; then
    npm install
    if [ $? -ne 0 ]; then
        echo "Error: npm install failed"
        exit 1
    fi
else
    echo "Dependencies already installed"
fi

echo ""
echo "Building frontend..."
npm run build
if [ $? -ne 0 ]; then
    echo "Error: npm build failed"
    exit 1
fi

cd ..

echo ""
echo "Dependencies installed successfully"
echo "Projects built successfully"
