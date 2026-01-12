#!/bin/bash

set -e

echo "step 3: install dependencies"
echo ""

echo "installing backend dependencies..."
if ! cd backend/AmazonBestSellers.API; then
    echo "error: cannot find backend directory"
    exit 1
fi

dotnet restore
if [ $? -ne 0 ]; then
    echo "error: dotnet restore failed"
    exit 1
fi

dotnet build
if [ $? -ne 0 ]; then
    echo "error: dotnet build failed"
    exit 1
fi

cd ../..

echo ""
echo "installing frontend dependencies..."
if ! cd frontend; then
    echo "error: cannot find frontend directory"
    exit 1
fi

if [ ! -d "node_modules" ]; then
    npm install
    if [ $? -ne 0 ]; then
        echo "error: npm install failed"
        exit 1
    fi
else
    echo "dependencies already installed"
fi

echo ""
echo "building frontend..."
npm run build
if [ $? -ne 0 ]; then
    echo "error: npm build failed"
    exit 1
fi

cd ..

echo ""
echo "dependencies installed"
echo "projects built"
