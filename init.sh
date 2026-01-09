#!/bin/bash

set -e

clear

echo "Amazon BestSellers Explorer - Initialization"
echo ""

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
cd "$SCRIPT_DIR"

echo "Running initialization scripts..."
echo ""

./scripts/01-setup-docker.sh

echo ""

./scripts/02-migrate-database.sh

echo ""

./scripts/03-install-dependencies.sh

echo ""

./scripts/04-seed-test-user.sh

echo ""
echo "Initialization complete"
echo ""
echo "Next steps:"
echo "  1. Run './start.sh' to start the application"
echo "  2. Open http://localhost:4200 in your browser"
echo "  3. Login with credentials configured in .env file (see README.md)"
echo ""
