export function isTokenExpired(token: string | null): boolean {
  if (!token) {
    return true;
  }

  try {
    const payload = parseJwt(token);
    if (!payload || !payload.exp) {
      return true;
    }

    const expirationDate = new Date(payload.exp * 1000);
    const now = new Date();

    return expirationDate <= now;
  } catch {
    return true;
  }
}

export function parseJwt(token: string): any {
  try {
    const base64Url = token.split('.')[1];
    const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
    const jsonPayload = decodeURIComponent(
      atob(base64)
        .split('')
        .map(c => '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2))
        .join('')
    );
    return JSON.parse(jsonPayload);
  } catch {
    return null;
  }
}
