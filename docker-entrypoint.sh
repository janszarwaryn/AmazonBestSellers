#!/bin/bash
set -e

echo "Starting Amazon BestSellers with embedded MariaDB..."

# Validate required env vars
REQUIRED_VARS=("DB_NAME" "DB_USER" "DB_PASSWORD" "JWT_SECRET" "JWT_ISSUER" "JWT_AUDIENCE")
MISSING_VARS=()

for var in "${REQUIRED_VARS[@]}"; do
    if [ -z "${!var}" ]; then
        MISSING_VARS+=("$var")
    fi
done

if [ ${#MISSING_VARS[@]} -ne 0 ]; then
    echo "ERROR: Missing env vars:"
    printf '  - %s\n' "${MISSING_VARS[@]}"
    exit 1
fi

# Set DB_HOST to localhost (internal MariaDB)
export DB_HOST=localhost
export DB_PORT=3306

echo "Initializing MariaDB..."
./init-mariadb.sh

echo "Starting services with supervisord..."
exec /usr/bin/supervisord -c /etc/supervisor/conf.d/supervisord.conf
