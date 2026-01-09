#!/bin/bash

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
cd "$SCRIPT_DIR"

BACKEND_PID=0
FRONTEND_PID=0

cleanup() {
    echo ""
    echo "Shutting down services..."

    if [ $BACKEND_PID -ne 0 ]; then
        echo "Stopping backend (PID: $BACKEND_PID)..."
        kill $BACKEND_PID 2>/dev/null || true
    fi

    if [ $FRONTEND_PID -ne 0 ]; then
        echo "Stopping frontend (PID: $FRONTEND_PID)..."
        kill $FRONTEND_PID 2>/dev/null || true
    fi

    echo "Services stopped"
    exit 0
}

trap cleanup SIGINT SIGTERM

clear

echo "Amazon BestSellers Explorer - Starting"
echo ""

if ! docker ps | grep -q amazon-bestsellers-db; then
    echo "Database not running. Starting..."
    docker-compose up -d
    sleep 5
fi

echo "Starting backend..."
if ! cd backend/AmazonBestSellers.API; then
    echo "Error: Cannot find backend directory"
    exit 1
fi

dotnet run --launch-profile https &
BACKEND_PID=$!
cd ../..

sleep 2

if ! kill -0 $BACKEND_PID 2>/dev/null; then
    echo "Error: Backend failed to start"
    echo "Check logs above for details"
    exit 1
fi

echo "Backend started (PID: $BACKEND_PID)"
echo ""

echo "Starting frontend..."
if ! cd frontend; then
    echo "Error: Cannot find frontend directory"
    kill $BACKEND_PID 2>/dev/null || true
    exit 1
fi

npm start &
FRONTEND_PID=$!
cd ..

sleep 3

if ! kill -0 $FRONTEND_PID 2>/dev/null; then
    echo "Error: Frontend failed to start"
    echo "Check logs above for details"
    kill $BACKEND_PID 2>/dev/null || true
    exit 1
fi

echo "Frontend started (PID: $FRONTEND_PID)"
echo ""
echo "Application is running"
echo ""
echo "Access points:"
echo "  Frontend:  http://localhost:4200"
echo "  Backend:   https://localhost:7196"
echo "  Swagger:   https://localhost:7196/swagger"
echo ""
echo "Login with credentials you configured in .env file"
echo "See README.md for more information"
echo ""
echo "Press Ctrl+C to stop all services"
echo ""

wait
