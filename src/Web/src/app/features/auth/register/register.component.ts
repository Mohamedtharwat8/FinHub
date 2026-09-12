import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../../core/auth/auth.service';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  template: `
    <div class="auth-container">
      <div class="auth-card">
        <h2>FinHub — Create Account</h2>
        <p class="subtitle">Join SAMA Open Banking Platform</p>

        <!-- Mode Toggle Tabs -->
        <div class="mode-tabs">
          <button type="button" [class.active]="regMode() === 'email'" (click)="setMode('email')">Email Address</button>
          <button type="button" [class.active]="regMode() === 'phone'" (click)="setMode('phone')">Phone OTP (+966)</button>
        </div>

        @if (errorMessage()) {
          <div class="error-alert">{{ errorMessage() }}</div>
        }
        @if (infoMessage()) {
          <div class="info-alert">{{ infoMessage() }}</div>
        }

        <!-- Email Registration Form -->
        <form *ngIf="regMode() === 'email'" [formGroup]="registerForm" (ngSubmit)="onSubmitEmail()">
          <div class="form-group">
            <label>Full Name</label>
            <input type="text" formControlName="fullName" placeholder="Ahmed Al-Ghamdi" />
          </div>

          <div class="form-group">
            <label>Email Address</label>
            <input type="email" formControlName="email" placeholder="ahmed@example.sa" />
          </div>

          <div class="form-group">
            <label>Password</label>
            <input type="password" formControlName="password" placeholder="••••••••" />
          </div>

          <button type="submit" [disabled]="registerForm.invalid || isLoading()">
            {{ isLoading() ? 'Creating account...' : 'Create Account' }}
          </button>
        </form>

        <!-- Phone Registration Form -->
        <form *ngIf="regMode() === 'phone'" [formGroup]="phoneForm" (ngSubmit)="onSubmitPhone()">
          <div class="form-group">
            <label>Full Name</label>
            <input type="text" formControlName="fullName" placeholder="Saad Al-Sudairi" />
          </div>

          <div class="form-group">
            <label>Saudi / Mobile Phone Number</label>
            <div class="phone-input-row">
              <input type="text" formControlName="phoneNumber" placeholder="+9665xxxxxxxx" />
              <button type="button" class="btn-send-otp" (click)="onSendOtp()" [disabled]="!phoneForm.get('phoneNumber')?.valid || isOtpSending()">
                {{ isOtpSending() ? 'Sending...' : 'Send OTP' }}
              </button>
            </div>
          </div>

          <div class="form-group">
            <label>6-Digit SMS OTP Code</label>
            <input type="text" formControlName="otpCode" placeholder="123456" maxlength="6" />
          </div>

          <button type="submit" [disabled]="phoneForm.invalid || isLoading()">
            {{ isLoading() ? 'Verifying OTP & Registering...' : 'Register with Phone' }}
          </button>
        </form>

        <div class="footer">
          Already have an account? <a routerLink="/auth/login">Sign in</a>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .auth-container { display: flex; justify-content: center; align-items: center; min-height: 85vh; padding: 1rem; }
    .auth-card { background: #1e1e2d; padding: 2.25rem; border-radius: 14px; width: 100%; max-width: 440px; color: #fff; box-shadow: 0 10px 30px rgba(0,0,0,0.4); }
    h2 { margin: 0; color: #fff; font-size: 1.6rem; text-align: center; }
    .subtitle { color: #8a8a9e; font-size: 0.875rem; margin-bottom: 1.25rem; text-align: center; }
    .mode-tabs { display: flex; gap: 0.5rem; background: #151521; padding: 0.25rem; border-radius: 8px; margin-bottom: 1.25rem; }
    .mode-tabs button { flex: 1; padding: 0.6rem; background: transparent; border: none; color: #8a8a9e; font-weight: 600; border-radius: 6px; cursor: pointer; font-size: 0.85rem; }
    .mode-tabs button.active { background: #3699ff; color: #fff; }
    .form-group { margin-bottom: 1.1rem; display: flex; flex-direction: column; }
    label { font-size: 0.85rem; margin-bottom: 0.4rem; color: #b5b5c3; }
    input { background: #151521; border: 1px solid #2b2b40; padding: 0.75rem; border-radius: 8px; color: #fff; outline: none; }
    input:focus { border-color: #3699ff; }
    .phone-input-row { display: flex; gap: 0.5rem; }
    .phone-input-row input { flex: 1; }
    .btn-send-otp { background: #2b2b40; color: #fff; border: 1px solid #3699ff; padding: 0.75rem 0.9rem; border-radius: 8px; font-weight: 600; cursor: pointer; white-space: nowrap; font-size: 0.8rem; }
    .btn-send-otp:disabled { border-color: #2b2b40; opacity: 0.5; cursor: not-allowed; }
    button[type="submit"] { background: #3699ff; border: none; color: #fff; padding: 0.75rem; border-radius: 8px; font-weight: 600; cursor: pointer; margin-top: 0.75rem; width: 100%; }
    button[type="submit"]:disabled { background: #2b2b40; cursor: not-allowed; }
    .error-alert { background: #f1416c; color: #fff; padding: 0.75rem; border-radius: 8px; font-size: 0.85rem; margin-bottom: 1rem; }
    .info-alert { background: #00c588; color: #fff; padding: 0.75rem; border-radius: 8px; font-size: 0.85rem; margin-bottom: 1rem; }
    .footer { margin-top: 1.5rem; text-align: center; font-size: 0.85rem; color: #8a8a9e; }
    .footer a { color: #3699ff; text-decoration: none; font-weight: 600; }
  `]
})
export class RegisterComponent {
  private fb = inject(FormBuilder);
  private authService = inject(AuthService);
  private router = inject(Router);

  public regMode = signal<'email' | 'phone'>('email');
  public isLoading = signal(false);
  public isOtpSending = signal(false);
  public errorMessage = signal<string | null>(null);
  public infoMessage = signal<string | null>(null);

  public registerForm = this.fb.group({
    fullName: ['', [Validators.required]],
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(8)]]
  });

  public phoneForm = this.fb.group({
    fullName: ['', [Validators.required]],
    phoneNumber: ['', [Validators.required, Validators.minLength(9)]],
    otpCode: ['', [Validators.required, Validators.minLength(6), Validators.maxLength(6)]]
  });

  public setMode(mode: 'email' | 'phone'): void {
    this.regMode.set(mode);
    this.errorMessage.set(null);
    this.infoMessage.set(null);
  }

  public onSubmitEmail(): void {
    if (this.registerForm.invalid) return;

    this.isLoading.set(true);
    this.errorMessage.set(null);

    this.authService.register(this.registerForm.value).subscribe({
      next: () => {
        this.isLoading.set(false);
        this.router.navigate(['/customer/profile']);
      },
      error: (err) => {
        this.isLoading.set(false);
        this.errorMessage.set(err.error?.error || 'Registration failed.');
      }
    });
  }

  public onSendOtp(): void {
    const phone = this.phoneForm.get('phoneNumber')?.value;
    if (!phone) return;

    this.isOtpSending.set(true);
    this.errorMessage.set(null);
    this.infoMessage.set(null);

    this.authService.sendOtp(phone, 'PhoneRegistration').subscribe({
      next: (res) => {
        this.isOtpSending.set(false);
        this.infoMessage.set(`SMS OTP sent to ${phone}! Demo Code: [ ${res.otpDemo} ]`);
      },
      error: (err) => {
        this.isOtpSending.set(false);
        this.errorMessage.set(err.error?.error || 'Failed to send OTP.');
      }
    });
  }

  public onSubmitPhone(): void {
    if (this.phoneForm.invalid) return;

    const { phoneNumber, fullName, otpCode } = this.phoneForm.value;

    this.isLoading.set(true);
    this.errorMessage.set(null);

    this.authService.registerWithPhone(phoneNumber!, fullName!, otpCode!).subscribe({
      next: () => {
        this.isLoading.set(false);
        this.router.navigate(['/customer/profile']);
      },
      error: (err) => {
        this.isLoading.set(false);
        this.errorMessage.set(err.error?.error || 'Phone registration failed.');
      }
    });
  }
}
