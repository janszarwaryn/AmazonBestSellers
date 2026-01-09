-- Database initialization script
-- Note: MariaDB automatically creates the user specified in MYSQL_USER environment variable
-- This script only sets the database character set

ALTER DATABASE AmazonBestSellersDb CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

-- User creation is handled automatically by docker-compose via MYSQL_USER and MYSQL_PASSWORD
-- No need to create user manually here
