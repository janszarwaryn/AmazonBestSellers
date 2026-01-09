#!/bin/bash

set -e

echo "Step 4: Seed Test User"
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

echo "Checking if Users table exists..."
TABLE_EXISTS=$(docker exec amazon-bestsellers-db mariadb -u ${DB_USER} -p${DB_PASSWORD} AmazonBestSellersDb -e "SHOW TABLES LIKE 'Users';" 2>/dev/null | grep -c "Users" || echo "0")

if [ "$TABLE_EXISTS" -eq 0 ]; then
    echo "Error: Users table does not exist. Run migrations first."
    exit 1
fi

PASSWORD_HASH='$2a$11$etiOaotIQZaqNBZ3Ibj/O.SSJC5/YWWje5DAGzWBjSNRQyXop6FgS'

SQL="INSERT INTO Users (Username, PasswordHash, CreatedAt, UpdatedAt)
VALUES ('${ADMIN_USERNAME}', '$PASSWORD_HASH', NOW(), NOW())
ON DUPLICATE KEY UPDATE PasswordHash='$PASSWORD_HASH';"

echo "Seeding test user..."
if docker exec amazon-bestsellers-db mariadb -u ${DB_USER} -p${DB_PASSWORD} AmazonBestSellersDb -e "$SQL" 2>/dev/null; then
    echo "Test user seeded successfully"
else
    echo "Warning: Could not seed test user (may already exist)"
fi

echo ""
echo "Test user created. See README.md for login instructions."
