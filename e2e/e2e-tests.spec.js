// E2E Tests for Amazon Bestsellers Explorer
// Run with: npx playwright test e2e-tests.spec.js

const { test, expect } = require('@playwright/test');

const BASE_URL = 'http://localhost:4200';
const API_URL = 'https://localhost:7196/api';

// Test user credentials
const TEST_USER = {
  username: 'testuser' + Date.now(),
  password: 'TestPassword123!@#'
};

test.describe('Amazon Bestsellers Explorer E2E Tests', () => {

  test.describe.configure({ mode: 'serial' });

  let page;
  let context;

  test.beforeAll(async ({ browser }) => {
    context = await browser.newContext({
      ignoreHTTPSErrors: true // For localhost SSL certificates
    });
    page = await context.newPage();
  });

  test.afterAll(async () => {
    await page.close();
    await context.close();
  });

  test('1. Should load homepage and redirect to login', async () => {
    await page.goto(BASE_URL);
    await page.waitForLoadState('networkidle');

    // Should redirect to login if not authenticated
    const currentUrl = page.url();
    expect(currentUrl).toContain('/login');

    // Check for login form elements
    await expect(page.locator('input[type="text"]')).toBeVisible();
    await expect(page.locator('input[type="password"]')).toBeVisible();
  });

  test('2. Should register a new user', async () => {
    // Navigate to register page
    await page.goto(`${BASE_URL}/register`);
    await page.waitForLoadState('networkidle');

    // Fill registration form
    await page.fill('input[type="text"]', TEST_USER.username);
    const passwordInputs = await page.locator('input[type="password"]').all();
    await passwordInputs[0].fill(TEST_USER.password);
    await passwordInputs[1].fill(TEST_USER.password);

    // Submit registration
    await page.click('button[type="submit"]');

    // Wait for navigation to products page
    await page.waitForURL(`${BASE_URL}/products`, { timeout: 5000 });

    // Verify we're on products page
    expect(page.url()).toContain('/products');
  });

  test('3. Should display products list', async () => {
    await page.goto(`${BASE_URL}/products`);
    await page.waitForLoadState('networkidle');

    // Wait for products to load (might take a while due to API call)
    await page.waitForSelector('.product-card, [class*="product"], [class*="card"]', {
      timeout: 10000
    });

    // Check if products are displayed
    const products = await page.locator('.product-card, [class*="product"], [class*="card"]').all();
    expect(products.length).toBeGreaterThan(0);
  });

  test('4. Should add product to favorites', async () => {
    await page.goto(`${BASE_URL}/products`);
    await page.waitForLoadState('networkidle');

    // Wait for products to load
    await page.waitForSelector('.product-card, [class*="product"], [class*="card"]', {
      timeout: 10000
    });

    // Find and click the first "Add to Favorites" button
    const addButton = page.locator('button').filter({ hasText: /dodaj|favorite|ulubione/i }).first();
    await addButton.waitFor({ state: 'visible', timeout: 5000 });
    await addButton.click();

    // Wait for success message or confirmation
    await page.waitForTimeout(1000);
  });

  test('5. Should view favorites list', async () => {
    await page.goto(`${BASE_URL}/favorites`);
    await page.waitForLoadState('networkidle');

    // Wait for favorites to load
    await page.waitForTimeout(1000);

    // Check if at least one favorite is displayed
    const favorites = await page.locator('.product-card, [class*="product"], [class*="favorite"]').all();
    expect(favorites.length).toBeGreaterThan(0);
  });

  test('6. Should remove product from favorites', async () => {
    await page.goto(`${BASE_URL}/favorites`);
    await page.waitForLoadState('networkidle');

    // Wait for favorites to load
    await page.waitForTimeout(1000);

    // Count favorites before removal
    const favoritesBefore = await page.locator('.product-card, [class*="product"], [class*="favorite"]').all();
    const countBefore = favoritesBefore.length;

    // Find and click the first "Remove" button
    const removeButton = page.locator('button').filter({ hasText: /usuń|remove|delete/i }).first();
    await removeButton.waitFor({ state: 'visible', timeout: 5000 });
    await removeButton.click();

    // Wait for removal to complete
    await page.waitForTimeout(1000);

    // Count favorites after removal
    const favoritesAfter = await page.locator('.product-card, [class*="product"], [class*="favorite"]').all();
    const countAfter = favoritesAfter.length;

    // Verify one item was removed
    expect(countAfter).toBe(countBefore - 1);
  });

  test('7. Should navigate using header menu', async () => {
    await page.goto(`${BASE_URL}/products`);
    await page.waitForLoadState('networkidle');

    // Click on Favorites link in navigation
    const favoritesLink = page.locator('a').filter({ hasText: /ulubione|favorites/i });
    await favoritesLink.click();
    await page.waitForURL(`${BASE_URL}/favorites`);
    expect(page.url()).toContain('/favorites');

    // Click on Products link in navigation
    const productsLink = page.locator('a').filter({ hasText: /produkty|products|bestsellers/i }).first();
    await productsLink.click();
    await page.waitForURL(`${BASE_URL}/products`);
    expect(page.url()).toContain('/products');
  });

  test('8. Should logout successfully', async () => {
    await page.goto(`${BASE_URL}/products`);
    await page.waitForLoadState('networkidle');

    // Click logout button
    const logoutButton = page.locator('button').filter({ hasText: /wyloguj|logout/i });
    await logoutButton.click();

    // Wait for redirect to login
    await page.waitForURL(`${BASE_URL}/login`, { timeout: 5000 });
    expect(page.url()).toContain('/login');
  });

  test('9. Should login with existing user', async () => {
    await page.goto(`${BASE_URL}/login`);
    await page.waitForLoadState('networkidle');

    // Fill login form
    await page.fill('input[type="text"]', TEST_USER.username);
    await page.fill('input[type="password"]', TEST_USER.password);

    // Submit login
    await page.click('button[type="submit"]');

    // Wait for navigation to products page
    await page.waitForURL(`${BASE_URL}/products`, { timeout: 5000 });
    expect(page.url()).toContain('/products');
  });

  test('10. Should handle invalid login', async () => {
    await page.goto(`${BASE_URL}/login`);
    await page.waitForLoadState('networkidle');

    // Fill login form with invalid credentials
    await page.fill('input[type="text"]', 'invaliduser');
    await page.fill('input[type="password"]', 'wrongpassword');

    // Submit login
    await page.click('button[type="submit"]');

    // Wait for error message
    await page.waitForTimeout(1000);

    // Should still be on login page
    expect(page.url()).toContain('/login');
  });
});
