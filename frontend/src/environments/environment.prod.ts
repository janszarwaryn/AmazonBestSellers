export const environment = {
  production: true,
  apiUrl: 'https://api.amazonbestsellers.com/api',
  rapidApiUrl: 'https://real-time-amazon-data.p.rapidapi.com',
  rapidApiHost: 'real-time-amazon-data.p.rapidapi.com',
  rapidApiKey: import.meta.env['NG_APP_RAPIDAPI_KEY'] || ''
};
