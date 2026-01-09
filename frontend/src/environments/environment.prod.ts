// Production environment configuration
// Use environment variables or CI/CD secrets for sensitive data

export const environment = {
  production: true,
  apiUrl: 'https://api.amazonbestsellers.com/api',
  rapidApiUrl: 'https://real-time-amazon-data.p.rapidapi.com',
  rapidApiHost: 'real-time-amazon-data.p.rapidapi.com',
  // In production, inject this via environment variables or build-time replacement
  rapidApiKey: process.env['RAPIDAPI_KEY'] || ''
};
