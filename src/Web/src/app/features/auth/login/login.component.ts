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
        <p class="subtitle">Access your SAMA Open Banking & Finance Dashboard</p>

        @if (errorMessage()) {
          <div class="error-alert">{{ errorMessage() }}</div>
        }

        <form [formGroup]="loginForm" (ngSubmit)="onSubmit()">
          <div class="form-group">
            <label>Email Address</label>
            <input type="email" formControlName="email" placeholder="name@example.sa" />
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
    .auth-container { display: flex; justify-content: center; align-items: center; min-height: 80vh; }
    .auth-card { background: #1e1e2d; padding: 2rem; border-radius: 12px; width: 100%; max-width: 400px; color: #fff; box-shadow: 0 8px 24px rgba(0,0,0,0.3); }
    h2 { margin: 0; color: #fff; font-size: 1.5rem; }
    .subtitle { color: #8a8a9e; font-size: 0.875rem; margin-bottom: 1.5rem; }
    .form-group { margin-bottom: 1.25rem; display: flex; flex-direction: column; }
    label { font-size: 0.85rem; margin-bottom: 0.5rem; color: #b5b5c3; }
    input { background: #151521; border: 1px solid #2b2b40; padding: 0.75rem; border-radius: 6px; color: #fff; outline: none; }
    input:focus { border-color: #3699ff; }
    button { background: #3699ff; border: none; color: #fff; padding: 0.75rem; border-radius: 6px; font-weight: 600; cursor: pointer; margin-top: 1rem; width: 100%; }
    button:disabled { background: #2b2b40; cursor: not-allowed; }
    .error-alert { background: #f1416c; color: #fff; padding: 0.75rem; border-radius: 6px; font-size: 0.85rem; margin-bottom: 1rem; }
    .footer { margin-top: 1.5rem; text-align: center; font-size: 0.85rem; color: #8a8a9e; }
    .footer a { color: #3699ff; text-decoration: none; }
  `]
})
export class LoginComponent {
  private fb = inject(FormBuilder);
  private authService = inject(AuthService);
  private router = inject(Router);

  public isLoading = signal(false);
  public errorMessage = signal<string | null>(null);

  public loginForm = this.fb.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(8)]]
  });

  public onSubmit(): void {
    if (this.loginForm.invalid) return;

    this.isLoading.set(true);
    this.errorMessage.set(null);

    this.authService.login(this.loginForm.value).subscribe({
      next: () => {
        this.isLoading.set(false);
        this.router.navigate(['/dashboard']);
      },
      error: (err) => {
        this.isLoading.set(false);
        this.errorMessage.set(err.error?.error || 'Invalid credentials or server unavailable.');
      }
    });
  }
}
