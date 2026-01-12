#!/bin/bash

set -e

clear

echo "amazon bestsellers explorer - initialization"
echo ""

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
cd "$SCRIPT_DIR"

echo "running initialization scripts..."
echo ""

./scripts/01-setup-docker.sh

echo ""

./scripts/02-migrate-database.sh

echo ""

./scripts/03-install-dependencies.sh

echo ""

./scripts/04-seed-test-user.sh

echo ""
echo "initialization complete"
echo ""
echo "next steps:"
echo "  1. run './start.sh' to start the application"
echo "  2. open http://localhost:4200 in your browser"
echo "  3. login with credentials configured in .env file (see README.md)"
echo ""
