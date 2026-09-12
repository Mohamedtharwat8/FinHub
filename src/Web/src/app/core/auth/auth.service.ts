import { Injectable, signal, computed, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable, tap, catchError, throwError } from 'rxjs';

export interface User {
  customerId: string;
  email: string;
  fullName: string;
}

export interface AuthResponse {
  customerId: string;
  email: string;
  fullName: string;
  accessToken: string;
  refreshToken: string;
  expiration: string;
  requiresMfa?: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private http = inject(HttpClient);
  private router = inject(Router);

  private get API_URL(): string {
    if (typeof window !== 'undefined' && window.location.hostname !== 'localhost' && window.location.hostname !== '127.0.0.1') {
      return 'https://finhub-w9xq.onrender.com/api/v1/auth';
    }
    return 'http://localhost:5000/api/v1/auth';
  }

  private readonly TOKEN_KEY = 'finhub_access_token';
  private readonly REFRESH_KEY = 'finhub_refresh_token';

  // Signal State Management
  private currentUserSignal = signal<User | null>(this.getUserFromStorage());
  public currentUser = computed(() => this.currentUserSignal());
  public isAuthenticated = computed(() => !!this.currentUserSignal() && !!this.getAccessToken());

  public getAccessToken(): string | null {
    return localStorage.getItem(this.TOKEN_KEY);
  }

  public getRefreshToken(): string | null {
    return localStorage.getItem(this.REFRESH_KEY);
  }

  public register(payload: any): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.API_URL}/register`, payload).pipe(
      tap(res => this.handleAuthSuccess(res))
    );
  }

  public login(payload: any): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.API_URL}/login`, payload).pipe(
      tap(res => {
        if (!res.requiresMfa) {
          this.handleAuthSuccess(res);
        }
      })
    );
  }

  public externalLogin(provider: string, token: string): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.API_URL}/external-login`, { provider, token }).pipe(
      tap(res => this.handleAuthSuccess(res))
    );
  }

  public sendOtp(target: string, purpose: string = 'Registration'): Observable<any> {
    return this.http.post<any>(`${this.API_URL}/send-otp`, { target, purpose });
  }

  public verifyOtp(target: string, code: string, purpose: string = 'Registration'): Observable<any> {
    return this.http.post<any>(`${this.API_URL}/verify-otp`, { target, code, purpose });
  }

  public registerWithPhone(phoneNumber: string, fullName: string, otpCode: string): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.API_URL}/register-phone`, { phoneNumber, fullName, otpCode }).pipe(
      tap(res => this.handleAuthSuccess(res))
    );
  }

  public refreshToken(): Observable<AuthResponse> {
    const refreshToken = this.getRefreshToken();
    if (!refreshToken) {
      this.logout();
      return throwError(() => new Error('No refresh token available'));
    }

    return this.http.post<AuthResponse>(`${this.API_URL}/refresh-token`, { refreshToken }).pipe(
      tap(res => this.handleAuthSuccess(res)),
      catchError(err => {
        this.logout();
        return throwError(() => err);
      })
    );
  }

  public logout(): void {
    localStorage.removeItem(this.TOKEN_KEY);
    localStorage.removeItem(this.REFRESH_KEY);
    localStorage.removeItem('finhub_user');
    this.currentUserSignal.set(null);
    this.router.navigate(['/auth/login']);
  }

  private handleAuthSuccess(res: AuthResponse): void {
    if (res.accessToken) {
      localStorage.setItem(this.TOKEN_KEY, res.accessToken);
      localStorage.setItem(this.REFRESH_KEY, res.refreshToken);

      const user: User = {
        customerId: res.customerId,
        email: res.email,
        fullName: res.fullName
      };
      localStorage.setItem('finhub_user', JSON.stringify(user));
      this.currentUserSignal.set(user);
    }
  }

  private getUserFromStorage(): User | null {
    const stored = localStorage.getItem('finhub_user');
    return stored ? JSON.parse(stored) : null;
  }
}
