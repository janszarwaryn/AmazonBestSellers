# Amazon BestSellers Explorer

Full-stack web application for browsing Amazon bestseller products and managing personal favorites list.

## System Requirements

- Docker Desktop (running)
- .NET 9.0 SDK
- Node.js 18+ and npm
- 4GB RAM minimum
- macOS, Linux, or Windows with WSL2

## Quick Start

### Three Steps

1. Initialize the project
   ```bash
   ./init.sh
   ```

2. Start the application
   ```bash
   ./start.sh
   ```

3. Open browser
   ```
   http://localhost:4200
   ```

## Configuration

### Environment Variables (Optional)

The application works out-of-the-box with default settings. To customize:

1. Copy `.env.example` to `.env`:
   ```bash
   cp .env.example .env
   ```

2. Edit `.env` with your preferred values

#### RapidAPI Key (Optional)

**For Development:** The app uses mock data when RapidAPI key is not configured.

**For Production/Real Data:**
1. Go to [RapidAPI - Real-Time Amazon Data](https://rapidapi.com/letscrape-6bRBa3QguO5/api/real-time-amazon-data)
2. Subscribe to the API (free tier available)
3. Copy your API key
4. Update `appsettings.Development.json`:
   ```json
   "RapidAPI": {
     "Key": "your-actual-api-key-here"
   }
   ```

### Default Configuration

| Component | Default | Configurable via |
|-----------|---------|------------------|
| Database Port | 3306 | `.env` (DB_PORT) |
| Backend HTTPS | 7196 | `launchSettings.json` |
| Frontend | 4200 | `angular.json` |
| Database Password | amazonpass123 | `.env` (DB_PASSWORD) |
| JWT Secret | Dev key | `appsettings.Development.json` |

### Important Security Notes

⚠️ **Never commit real API keys or production secrets to version control!**

- `.env` is git-ignored
- Use `appsettings.Development.json` for local development
- Use environment variables or Azure Key Vault for production

## Authentication

### Test Account

Username: admin
Password: Admin123!@#

### Creating New Account

1. Navigate to http://localhost:4200
2. Click Register
3. Fill the form (min 8 characters, uppercase, lowercase, number, special character)
4. Submit and login

## About

Amazon BestSellers Explorer allows users to browse current Amazon bestselling products, authenticate securely, and maintain a personalized favorites list. Built with modern technologies following Clean Architecture and Clean Code principles.

## Technologies

### Frontend
| Technology | Version | Purpose |
|------------|---------|---------|
| Angular | 19.2.19 | Framework |
| TypeScript | 5.7+ | Language |
| PrimeNG | 19.0.0 | UI Components |
| Angular Signals | - | State Management |
| RxJS | 7.8+ | Reactive Programming |

### Backend
| Technology | Version | Purpose |
|------------|---------|---------|
| .NET | 9.0 | Framework |
| C# | 13.0 | Language |
| Entity Framework Core | 8.0.2 | ORM |
| MariaDB | 11.2 | Database |
| JWT | 8.2.1 | Authentication |
| BCrypt | 4.0.3 | Password Hashing |
| FluentValidation | 11.10.0 | Input Validation |
| Serilog | 8.0.3 | Logging |

## Features

- User registration and authentication
- JWT token-based security
- Browse Amazon bestseller products
- Add products to personal favorites
- Remove products from favorites
- Responsive design
- Real-time product data integration
- Secure password hashing
- Input validation
- Error handling
- Audit logging

## Access Points

| Service | URL | Description |
|---------|-----|-------------|
| Frontend | http://localhost:4200 | Main application |
| Backend API | https://localhost:7196 | REST API |
| Swagger UI | https://localhost:7196/swagger | API documentation |
| Database | localhost:3306 | MariaDB |

## Project Structure

```
AmazonBestSellers/
├── backend/                          Backend .NET application
│   ├── AmazonBestSellers.Domain/     Entities and interfaces
│   ├── AmazonBestSellers.Application/ Business logic and DTOs
│   ├── AmazonBestSellers.Infrastructure/ Database and repositories
│   └── AmazonBestSellers.API/        Controllers and middleware
├── frontend/                         Frontend Angular application
├── scripts/                          Initialization scripts
│   ├── 01-setup-docker.sh
│   ├── 02-migrate-database.sh
│   ├── 03-install-dependencies.sh
│   └── 04-seed-test-user.sh
├── docker/                           Docker configuration
│   └── init/                         Database initialization SQL
├── docker-compose.yml                Database configuration
├── init.sh                           One-command initialization
├── start.sh                          One-command startup
└── README.md                         This file
```

## Architecture

### Backend - Clean Architecture

```
API Layer
  ↓
Application Layer (Business Logic)
  ↓
Domain Layer (Entities)
  ↓
Infrastructure Layer (Database, External APIs)
```

Principles: Dependency Inversion, Single Responsibility, Repository Pattern, SOLID, DRY, KISS

### Frontend - Feature-Based

```
App (Root)
  ↓
Core (Services, Guards, Interceptors)
  ↓
Features (Auth, Products, Favorites)
  ↓
Shared (Reusable Components)
```

Patterns: Signal-based State Management, Functional Guards and Interceptors, Reactive Forms, Lazy Loading, Standalone Components

## API Endpoints

### Authentication
- POST /api/auth/register - Register new user
- POST /api/auth/login - User login

### Products
- GET /api/products/bestsellers - Get bestsellers list

### Favorites
- GET /api/favorites - Get user favorites
- POST /api/favorites - Add to favorites
- DELETE /api/favorites/{id} - Remove from favorites

## Database Schema

### Users Table
- Id (Primary Key)
- Username (Unique)
- Email (Unique)
- PasswordHash
- CreatedAt
- UpdatedAt

### FavoriteProducts Table
- Id (Primary Key)
- UserId (Foreign Key)
- ProductAsin
- ProductTitle
- ProductImageUrl
- ProductPrice
- CreatedAt

## Configuration

### Backend Configuration

Edit backend/AmazonBestSellers.API/appsettings.json

### Frontend Configuration

Edit frontend/src/environments/environment.ts

## Development

Backend development:
```bash
cd backend/AmazonBestSellers.API
dotnet watch run
```

Frontend development:
```bash
cd frontend
npm start
```

## Stopping Services

Press Ctrl+C in terminal running start.sh

To stop database:
```bash
docker-compose down
```

## Troubleshooting

### Docker Not Running
Start Docker Desktop application

### Port Already in Use
```bash
lsof -ti:4200 | xargs kill -9
lsof -ti:7196 | xargs kill -9
```

### Database Connection Failed
```bash
docker-compose down -v
docker-compose up -d
```

### Reset Everything
```bash
docker-compose down -v
./init.sh
```

## Security Features

- BCrypt password hashing (work factor 11)
- JWT token authentication
- Token expiration (60 minutes)
- Password complexity validation
- SQL injection prevention
- XSS protection
- CORS configuration
- HTTPS support

## Code Quality

- Zero comments (self-documenting code)
- Zero emojis
- SOLID principles
- Clean Code methodology
- Async/await throughout
- Proper error handling
- Input validation
- Type safety

## Production Deployment

Before deploying to production:

1. Change JWT secret to cryptographically secure value
2. Use environment variables for sensitive data
3. Configure production database connection
4. Enable HTTPS only
5. Set proper CORS origins
6. Configure logging aggregation
7. Set up monitoring and health checks
8. Use managed database service
9. Implement rate limiting
10. Add comprehensive testing

## License

Proprietary - All rights reserved

## Support

For issues or questions, please contact the development team.
