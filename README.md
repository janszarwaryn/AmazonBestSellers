# Amazon BestSellers Explorer

Full-stack application for browsing Amazon bestseller products with user authentication and favorites management.

## Requirements

- Docker Desktop (running)
- .NET 9.0 SDK
- Node.js 18+
- 4GB RAM minimum

## Quick Start

```bash
./init.sh
./start.sh
```

Open http://localhost:4200

## ⚠️ Security Setup (REQUIRED)

### Environment Variables

**NEVER commit `.env` file to git!** It contains sensitive secrets.

1. Copy example file:
   ```bash
   cp .env.example .env
   ```

2. Generate secure secrets:

   **JWT Secret (64+ characters):**
   ```bash
   openssl rand -base64 64 | tr -d '\n'
   ```

   **Database Passwords (32+ characters):**
   ```bash
   openssl rand -base64 32 | tr -d '\n'
   ```

3. Get RapidAPI Key:
   - Subscribe at [RapidAPI - Real-Time Amazon Data](https://rapidapi.com/letscrape-6bRBa3QguO5/api/real-time-amazon-data)
   - Copy your API key from dashboard
   - Add to `.env`: `RAPIDAPI_KEY=your_key_here`

4. Set admin credentials in `.env` (CHANGE defaults!):
   ```
   ADMIN_USERNAME=your_secure_username
   ADMIN_PASSWORD=your_secure_password
   ```

### Test Account

After running `./init.sh`, you can login with credentials you set in `.env` file
(ADMIN_USERNAME and ADMIN_PASSWORD).

**DO NOT share these credentials publicly!**

### Backend Development Settings (Optional)

**For local development only**, you can create `backend/AmazonBestSellers.API/appsettings.Development.json`:

```bash
# Copy example file
cp backend/AmazonBestSellers.API/appsettings.Development.json.example \
   backend/AmazonBestSellers.API/appsettings.Development.json
```

**⚠️ IMPORTANT:**
- This file is `.gitignore`d and **should NEVER be committed** to git
- Backend loads secrets from `.env` file via `Program.cs` - appsettings are overridden
- The example file contains placeholders only - DO NOT add real secrets there
- All sensitive configuration comes from `.env` file

**Architecture:** Backend uses **secure proxy pattern** for RapidAPI:
```
Frontend → Backend API (/api/products/bestsellers) → RapidAPI
```

This keeps API keys secure on backend only, enables caching, and prevents CORS issues.

## Configuration

The `.env` file should be configured with all required variables. See `.env.example` for reference.

## Technologies

**Frontend:** Angular 19, TypeScript, PrimeNG, Tailwind CSS
**Backend:** .NET 9, Entity Framework Core, MariaDB, JWT
**Testing:** xUnit, Playwright

## Testing

### Backend Unit Tests (xUnit)

**Location:** `backend/AmazonBestSellers.Tests/`

**Test Coverage:**
- `AuthServiceTests` (5 tests) - User registration, login, validation
- `FavoriteProductServiceTests` (7 tests) - Add, remove, list favorites
- `PasswordHasherTests` (6 tests) - BCrypt hashing and verification
- `JwtTokenServiceTests` (2 tests) - JWT token generation and validation
- `UserRepositoryTests` (2 tests) - Database user operations

**Run all tests:**
```bash
cd backend
dotnet test
```

**Run with detailed output:**
```bash
cd backend
dotnet test --verbosity detailed
```

**Expected result:** 22 tests passed, 0 failed

### E2E Tests (Playwright)

**Location:** `init-scripts/e2e-tests.spec.js`

**Test Coverage:**
- User registration flow
- User login/logout
- Product list display
- Add product to favorites
- View favorites list
- Remove from favorites
- Navigation between pages
- Invalid login handling

**Prerequisites:**
- Application must be running (`./start.sh`)
- Both frontend and backend servers active

**Run E2E tests:**
```bash
npx playwright test
```

**Run with UI mode:**
```bash
npx playwright test --ui
```

**Run specific test:**
```bash
npx playwright test -g "Should register a new user"
```

**View test report:**
```bash
npx playwright show-report
```

## Access Points

- Frontend: http://localhost:4200
- Backend API: https://localhost:7196
- Swagger: https://localhost:7196/swagger
- Database: localhost:3306

## API Endpoints

**Auth**
- `POST /api/auth/register` - Register user
- `POST /api/auth/login` - Login

**Products**
- `GET /api/products/bestsellers` - Get products

**Favorites**
- `GET /api/favorites` - List favorites
- `POST /api/favorites` - Add favorite
- `DELETE /api/favorites/{id}` - Remove favorite

## Project Structure

```
backend/
  ├── AmazonBestSellers.Domain/       # Entities, interfaces
  ├── AmazonBestSellers.Application/  # Business logic, DTOs
  ├── AmazonBestSellers.Infrastructure/ # Database, repositories
  ├── AmazonBestSellers.API/          # Controllers, middleware
  └── AmazonBestSellers.Tests/        # Unit tests
frontend/src/app/
  ├── core/                           # Services, guards
  ├── features/                       # Auth, Products, Favorites
  └── shared/                         # Reusable components
init-scripts/
  └── e2e-tests.spec.js              # Playwright E2E tests
```

## Development

Backend:
```bash
cd backend/AmazonBestSellers.API
dotnet watch run
```

Frontend:
```bash
cd frontend
npm start
```

## Stopping Services

Press `Ctrl+C` or:
```bash
./stop.sh
```

Stop database:
```bash
docker-compose down
```

## Troubleshooting

**Port in use:**
```bash
lsof -ti:4200 | xargs kill -9
lsof -ti:7196 | xargs kill -9
```

**Database issues:**
```bash
docker-compose down -v
./init.sh
```

## Production Deployment

1. Update `.env` with secure credentials
2. Change JWT secret (min 32 characters)
3. Configure production database
4. Enable HTTPS only
5. Set proper CORS origins
6. Configure logging and monitoring
