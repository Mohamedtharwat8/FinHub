import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../../core/auth/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  template: `
    <div class="auth-container">
      <div class="auth-card">
        <h2>FinHub — Sign In</h2>
        <p class="subtitle">SAMA Open Banking & Finance Platform</p>

        @if (errorMessage()) {
          <div class="error-alert">{{ errorMessage() }}</div>
        }

        <!-- Social OAuth Buttons -->
        <div class="social-auth-group">
          <button type="button" class="btn-social google" (click)="onSocialLogin('Google')">
            <span>Continue with Google</span>
          </button>
          <button type="button" class="btn-social github" (click)="onSocialLogin('GitHub')">
            <span>Continue with GitHub</span>
          </button>
          <button type="button" class="btn-social microsoft" (click)="onSocialLogin('Microsoft')">
            <span>Continue with Microsoft</span>
          </button>
        </div>

        <div class="separator"><span>OR EMAIL / PHONE SIGN IN</span></div>

        <form [formGroup]="loginForm" (ngSubmit)="onSubmit()">
          <div class="form-group">
            <label>Email Address or Phone Number</label>
            <input type="text" formControlName="email" placeholder="name@example.sa or +9665xxxxxxxx" />
          </div>

          <div class="form-group">
            <label>Password</label>
            <input type="password" formControlName="password" placeholder="••••••••" />
          </div>

          <button type="submit" [disabled]="loginForm.invalid || isLoading()">
            {{ isLoading() ? 'Signing in...' : 'Sign In' }}
          </button>
        </form>

        <div class="footer">
          Don't have an account? <a routerLink="/auth/register">Register now</a>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .auth-container { display: flex; justify-content: center; align-items: center; min-height: 85vh; padding: 1rem; }
    .auth-card { background: #1e1e2d; padding: 2.25rem; border-radius: 14px; width: 100%; max-width: 440px; color: #fff; box-shadow: 0 10px 30px rgba(0,0,0,0.4); }
    h2 { margin: 0; color: #fff; font-size: 1.6rem; text-align: center; }
    .subtitle { color: #8a8a9e; font-size: 0.875rem; margin-bottom: 1.5rem; text-align: center; }
    .social-auth-group { display: flex; flex-direction: column; gap: 0.65rem; margin-bottom: 1.25rem; }
    .btn-social { display: flex; align-items: center; justify-content: center; padding: 0.7rem; border-radius: 8px; font-size: 0.875rem; font-weight: 600; cursor: pointer; border: 1px solid #2b2b40; color: #fff; background: #151521; transition: all 0.2s ease; }
    .btn-social:hover { background: #2b2b40; }
    .btn-social.google { border-color: #ea4335; }
    .btn-social.github { border-color: #333; }
    .btn-social.microsoft { border-color: #00a4ef; }
    .separator { text-align: center; margin: 1.25rem 0; position: relative; }
    .separator span { background: #1e1e2d; padding: 0 0.5rem; color: #6c7293; font-size: 0.75rem; font-weight: 600; }
    .form-group { margin-bottom: 1.1rem; display: flex; flex-direction: column; }
    label { font-size: 0.85rem; margin-bottom: 0.4rem; color: #b5b5c3; }
    input { background: #151521; border: 1px solid #2b2b40; padding: 0.75rem; border-radius: 8px; color: #fff; outline: none; }
    input:focus { border-color: #3699ff; }
    button[type="submit"] { background: #3699ff; border: none; color: #fff; padding: 0.75rem; border-radius: 8px; font-weight: 600; cursor: pointer; margin-top: 0.75rem; width: 100%; }
    button[type="submit"]:disabled { background: #2b2b40; cursor: not-allowed; }
    .error-alert { background: #f1416c; color: #fff; padding: 0.75rem; border-radius: 8px; font-size: 0.85rem; margin-bottom: 1rem; }
    .footer { margin-top: 1.5rem; text-align: center; font-size: 0.85rem; color: #8a8a9e; }
    .footer a { color: #3699ff; text-decoration: none; font-weight: 600; }
  `]
})
export class LoginComponent {
  private fb = inject(FormBuilder);
  private authService = inject(AuthService);
  private router = inject(Router);

  public isLoading = signal(false);
  public errorMessage = signal<string | null>(null);

  public loginForm = this.fb.group({
    email: ['', [Validators.required]],
    password: ['', [Validators.required, Validators.minLength(8)]]
  });

  public onSubmit(): void {
    if (this.loginForm.invalid) return;

    this.isLoading.set(true);
    this.errorMessage.set(null);

    this.authService.login(this.loginForm.value).subscribe({
      next: () => {
        this.isLoading.set(false);
        this.router.navigate(['/customer/profile']);
      },
      error: (err) => {
        this.isLoading.set(false);
        this.errorMessage.set(err.error?.error || 'Invalid credentials or server unavailable.');
      }
    });
  }

  public onSocialLogin(provider: string): void {
    this.isLoading.set(true);
    this.errorMessage.set(null);

    const dummyToken = `${provider.toLowerCase()}-oauth-token-${Date.now()}`;

    this.authService.externalLogin(provider, dummyToken).subscribe({
      next: () => {
        this.isLoading.set(false);
        this.router.navigate(['/customer/profile']);
      },
      error: (err) => {
        this.isLoading.set(false);
        this.errorMessage.set(err.error?.error || `${provider} authentication failed.`);
      }
    });
  }
}
