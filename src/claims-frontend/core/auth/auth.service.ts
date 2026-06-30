import { HttpClient } from '@angular/common/http';
import { Injectable, computed, inject, signal } from '@angular/core';
import { Observable, tap } from 'rxjs';
import { environment } from '../../../environments/environment';

interface LoginRequest {
  email: string;
  password: string;
}

interface LoginResponse {
  token: string;
}

interface DecodedToken {
  sub: string; // userId
  role: string;
  organisation_id: string;
  exp: number;
}

const TOKEN_KEY = 'claims_module_token';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/auth`;

  private readonly _token = signal<string | null>(localStorage.getItem(TOKEN_KEY));

  readonly token = this._token.asReadonly();
  readonly isAuthenticated = computed(() => this._token() !== null && !this.isTokenExpired());

  readonly currentUser = computed(() => {
    const token = this._token();
    if (!token) return null;

    const decoded = this.decodeToken(token);
    if (!decoded) return null;

    return {
      userId: decoded.sub,
      role: decoded.role,
      organisationId: decoded.organisation_id
    };
  });

  login(email: string, password: string): Observable<LoginResponse> {
    return this.http
      .post<LoginResponse>(`${this.baseUrl}/login`, { email, password } satisfies LoginRequest)
      .pipe(
        tap((response) => {
          localStorage.setItem(TOKEN_KEY, response.token);
          this._token.set(response.token);
        })
      );
  }

  logout(): void {
    localStorage.removeItem(TOKEN_KEY);
    this._token.set(null);
  }

  private decodeToken(token: string): DecodedToken | null {
    try {
      const payload = token.split('.')[1];
      const decoded = atob(payload.replace(/-/g, '+').replace(/_/g, '/'));
      return JSON.parse(decoded) as DecodedToken;
    } catch {
      return null;
    }
  }

  private isTokenExpired(): boolean {
    const token = this._token();
    if (!token) return true;

    const decoded = this.decodeToken(token);
    if (!decoded) return true;

    return decoded.exp * 1000 < Date.now();
  }
}
