import { Injectable, signal, computed, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { StorageService } from './storage.service';
import { LoginRequest, RegisterRequest, AuthResponse, User } from '@core/models';
import { environment } from '@environments/environment';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly storage = inject(StorageService);
  private readonly apiUrl = `${environment.apiUrl}/auth`;

  private currentUserSignal = signal<User | null>(null);

  public readonly currentUser = this.currentUserSignal.asReadonly();
  public readonly isAuthenticated = computed(() => this.currentUserSignal() !== null);

  constructor() {
    this.loadUserFromStorage();
  }

  login(credentials: LoginRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiUrl}/login`, credentials)
      .pipe(
        tap(response => this.handleAuthResponse(response))
      );
  }

  register(data: RegisterRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiUrl}/register`, data)
      .pipe(
        tap(response => this.handleAuthResponse(response))
      );
  }

  logout(): void {
    this.storage.clear();
    this.currentUserSignal.set(null);
  }

  private handleAuthResponse(response: AuthResponse): void {
    this.storage.setToken(response.token);
    const user: User = {
      id: response.userId,
      username: response.username,
      createdAt: new Date()
    };
    this.storage.setUser(user);
    this.currentUserSignal.set(user);
  }

  private loadUserFromStorage(): void {
    const user = this.storage.getUser<User>();
    if (user && this.storage.isTokenValid()) {
      this.currentUserSignal.set(user);
    } else if (user) {
      this.storage.clear();
    }
  }
}
