#!/bin/bash

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
cd "$SCRIPT_DIR"

BACKEND_PID=0
FRONTEND_PID=0

cleanup() {
    echo ""
    echo "shutting down services..."

    if [ $BACKEND_PID -ne 0 ]; then
        echo "stopping backend (PID: $BACKEND_PID)..."
        kill $BACKEND_PID 2>/dev/null || true
    fi

    if [ $FRONTEND_PID -ne 0 ]; then
        echo "stopping frontend (PID: $FRONTEND_PID)..."
        kill $FRONTEND_PID 2>/dev/null || true
    fi

    echo "services stopped"
    exit 0
}

trap cleanup SIGINT SIGTERM

clear

echo "amazon bestsellers explorer - starting"
echo ""

if ! docker ps | grep -q amazon-bestsellers-db; then
    echo "database not running, starting..."
    docker-compose up -d
    sleep 5
fi

# Check if port 7196 is free
if lsof -i:7196 >/dev/null 2>&1; then
    echo "error: port 7196 is already in use"
    echo "run './stop.sh' first to stop all services"
    echo ""
    echo "or manually kill the process:"
    echo "  lsof -ti:7196 | xargs kill -9"
    exit 1
fi

echo "starting backend..."
if ! cd backend/AmazonBestSellers.API; then
    echo "error: cannot find backend directory"
    exit 1
fi

dotnet run --launch-profile https &
BACKEND_PID=$!
cd ../..

sleep 2

if ! kill -0 $BACKEND_PID 2>/dev/null; then
    echo "error: backend failed to start"
    echo "check logs above for details"
    exit 1
fi

echo "backend started (PID: $BACKEND_PID)"
echo ""

echo "starting frontend..."
if ! cd frontend; then
    echo "error: cannot find frontend directory"
    kill $BACKEND_PID 2>/dev/null || true
    exit 1
fi

npm start &
FRONTEND_PID=$!
cd ..

sleep 3

if ! kill -0 $FRONTEND_PID 2>/dev/null; then
    echo "error: frontend failed to start"
    echo "check logs above for details"
    kill $BACKEND_PID 2>/dev/null || true
    exit 1
fi

echo "frontend started (PID: $FRONTEND_PID)"
echo ""
echo "application is running"
echo ""
echo "access points:"
echo "  frontend:  http://localhost:4200"
echo "  backend:   https://localhost:7196"
echo "  swagger:   https://localhost:7196/swagger"
echo ""
echo "login with credentials you configured in .env file"
echo "see README.md for more information"
echo ""
echo "press Ctrl+C to stop all services"
echo ""

wait
