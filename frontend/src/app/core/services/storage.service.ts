import { Injectable } from '@angular/core';
import { StorageKeys } from '@core/constants';
import { isTokenExpired } from '@core/utils/jwt.utils';

@Injectable({
  providedIn: 'root'
})
export class StorageService {
  private readonly TOKEN_KEY = StorageKeys.AUTH_TOKEN;
  private readonly USER_KEY = StorageKeys.CURRENT_USER;

  setToken(token: string): void {
    localStorage.setItem(this.TOKEN_KEY, token);
  }

  getToken(): string | null {
    return localStorage.getItem(this.TOKEN_KEY);
  }

  getValidToken(): string | null {
    const token = this.getToken();
    if (token && !isTokenExpired(token)) {
      return token;
    }
    return null;
  }

  isTokenValid(): boolean {
    const token = this.getToken();
    return token !== null && !isTokenExpired(token);
  }

  removeToken(): void {
    localStorage.removeItem(this.TOKEN_KEY);
  }

  setUser(user: any): void {
    localStorage.setItem(this.USER_KEY, JSON.stringify(user));
  }

  getUser<T>(): T | null {
    const user = localStorage.getItem(this.USER_KEY);
    return user ? JSON.parse(user) : null;
  }

  removeUser(): void {
    localStorage.removeItem(this.USER_KEY);
  }

  clear(): void {
    localStorage.clear();
  }
}
