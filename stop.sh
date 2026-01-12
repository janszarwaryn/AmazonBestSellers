#!/bin/bash

# colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # no color

echo -e "${YELLOW}amazon bestsellers explorer - stopping${NC}"
echo

# function to stop process
stop_process() {
    local process_name=$1
    local search_pattern=$2

    echo -e "stopping ${process_name}..."
    if pkill -f "$search_pattern" 2>/dev/null; then
        echo -e "${GREEN}✓ ${process_name} stopped${NC}"
    else
        echo -e "${YELLOW}⚠ ${process_name} not running${NC}"
    fi
}

# stop frontend (ng serve)
stop_process "Frontend (Angular)" "ng serve"

# stop backend (dotnet)
stop_process "Backend (.NET)" "dotnet.*AmazonBestSellers"

# stop start.sh if running
stop_process "Start script" "start.sh"

# stop any background bash processes from start.sh
stop_process "Background processes" "bash.*start.sh"

echo -e "checking port 7196..."
# Kill any remaining process on port 7196
if lsof -ti:7196 >/dev/null 2>&1; then
    lsof -ti:7196 | xargs kill -9 2>/dev/null || true
    echo -e "${GREEN}✓ Port 7196 freed${NC}"
    sleep 1
else
    echo -e "${GREEN}✓ Port 7196 already free${NC}"
fi

echo
echo -e "${GREEN}all services stopped${NC}"
echo
echo "note: MariaDB Docker container is still running"
echo "to stop it, run: docker stop amazon-bestsellers-db"
