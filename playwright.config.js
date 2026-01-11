// Playwright configuration for Amazon Bestsellers Explorer E2E tests
const { defineConfig, devices } = require('@playwright/test');

module.exports = defineConfig({
  testDir: './e2e',
  testMatch: '**/*.spec.js',

  // Maximum time one test can run
  timeout: 60 * 1000,

  // Test configuration
  expect: {
    timeout: 10000
  },

  // Run tests in files in parallel
  fullyParallel: false,

  // Fail the build on CI if you accidentally left test.only in the source code
  forbidOnly: !!process.env.CI,

  // Retry on CI only
  retries: process.env.CI ? 2 : 0,

  // Reporter to use
  reporter: 'html',

  // Shared settings for all the projects below
  use: {
    // Base URL to use in actions like `await page.goto('/')`
    baseURL: 'http://localhost:4200',

    // Collect trace when retrying the failed test
    trace: 'on-first-retry',

    // Screenshot on failure
    screenshot: 'only-on-failure',

    // Ignore HTTPS errors (for localhost development)
    ignoreHTTPSErrors: true
  },

  // Configure projects for major browsers
  projects: [
    {
      name: 'chromium',
      use: { ...devices['Desktop Chrome'] },
    },
  ],

  // Run your local dev server before starting the tests
  // Note: Application should already be running via start.sh
  // webServer: {
  //   command: 'cd frontend && npm start',
  //   url: 'http://localhost:4200',
  //   reuseExistingServer: true,
  //   timeout: 120 * 1000,
  // },
});
