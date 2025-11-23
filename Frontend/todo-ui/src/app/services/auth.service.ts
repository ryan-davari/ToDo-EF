import { computed, inject, Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { tap } from 'rxjs';
import { Router } from '@angular/router';

import { AuthenticatedUser, LoginRequest, RegisterRequest } from '../models/auth.models';
import { environment } from '../../environments/environment';

const STORAGE_KEY = 'todo-auth-user';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly router = inject(Router);
  private readonly baseUrl = `${environment.apiBaseUrl}/api/Account`;

  private readonly _currentUser = signal<AuthenticatedUser | null>(this.restoreUser());

  readonly currentUser = this._currentUser.asReadonly();
  readonly isAuthenticated = computed(() => !!this._currentUser());

  login(request: LoginRequest) {
    return this.http.post<AuthenticatedUser>(`${this.baseUrl}/login`, request).pipe(
      tap((user) => {
        this.persistUser(user);
        this._currentUser.set(user);
      })
    );
  }

  register(request: RegisterRequest) {
    return this.http.post<AuthenticatedUser>(`${this.baseUrl}/register`, request).pipe(
      tap((user) => {
        this.persistUser(user);
        this._currentUser.set(user);
      })
    );
  }

  logout(): void {
    localStorage.removeItem(STORAGE_KEY);
    this._currentUser.set(null);
    this.router.navigate(['/login']);
  }

  get token(): string | null {
    return this._currentUser()?.token ?? null;
  }

  private persistUser(user: AuthenticatedUser): void {
    localStorage.setItem(STORAGE_KEY, JSON.stringify(user));
  }

  private restoreUser(): AuthenticatedUser | null {
    const stored = localStorage.getItem(STORAGE_KEY);

    if (!stored) {
      return null;
    }

    try {
      return JSON.parse(stored) as AuthenticatedUser;
    } catch (err) {
      console.error('Failed to parse stored auth user', err);
      localStorage.removeItem(STORAGE_KEY);
      return null;
    }
  }
}
