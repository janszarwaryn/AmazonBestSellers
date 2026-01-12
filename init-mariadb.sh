#!/bin/bash
set -e

# Initialize MariaDB data directory if not exists
if [ ! -d "/var/lib/mysql/mysql" ]; then
    echo "Initializing MariaDB data directory..."
    mysql_install_db --user=mysql --datadir=/var/lib/mysql
fi

# Start MariaDB temporarily to set up database
echo "Starting MariaDB for initialization..."
mysqld_safe --datadir=/var/lib/mysql --skip-networking &
MYSQL_PID=$!

# Wait for MariaDB to be ready
echo "Waiting for MariaDB to start..."
for i in {1..30}; do
    if mysqladmin ping &>/dev/null; then
        echo "MariaDB is ready"
        break
    fi
    if [ $i -eq 30 ]; then
        echo "ERROR: MariaDB failed to start"
        exit 1
    fi
    sleep 1
done

# Set root password and create database
echo "Configuring database..."
mysql -u root <<-EOSQL
    ALTER USER 'root'@'localhost' IDENTIFIED BY '${DB_PASSWORD}';
    CREATE DATABASE IF NOT EXISTS ${DB_NAME};
    CREATE USER IF NOT EXISTS '${DB_USER}'@'localhost' IDENTIFIED BY '${DB_PASSWORD}';
    GRANT ALL PRIVILEGES ON ${DB_NAME}.* TO '${DB_USER}'@'localhost';
    FLUSH PRIVILEGES;
EOSQL

# Stop temporary MariaDB
echo "Stopping temporary MariaDB..."
mysqladmin -u root -p${DB_PASSWORD} shutdown
wait $MYSQL_PID

echo "MariaDB initialization complete"
