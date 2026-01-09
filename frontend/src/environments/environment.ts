// WARNING: Angular doesn't support runtime .env files
//
// For RapidAPI integration, we use BACKEND PROXY pattern for security:
// Frontend -> Backend API (/api/products/bestsellers) -> RapidAPI
//
// This approach provides:
// ✓ Keeps API keys secure on backend only (stored in .env)
// ✓ Enables response caching to reduce API calls (10 min TTL)
// ✓ Prevents CORS issues
// ✓ Allows rate limiting and monitoring
// ✓ No secrets in frontend code or git
//
// Backend handles RapidAPI authentication using RAPIDAPI_KEY from .env

export const environment = {
  production: false,
  apiUrl: 'https://localhost:7196/api'
};
