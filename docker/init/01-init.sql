ALTER DATABASE AmazonBestSellersDb CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

CREATE USER IF NOT EXISTS 'amazonuser'@'%' IDENTIFIED BY 'amazonpass123';
GRANT ALL PRIVILEGES ON AmazonBestSellersDb.* TO 'amazonuser'@'%';
FLUSH PRIVILEGES;
