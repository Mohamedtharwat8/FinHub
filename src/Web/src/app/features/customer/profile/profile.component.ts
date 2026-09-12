import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { AuthService } from '../../../core/auth/auth.service';
import { CustomerService, CustomerProfileDto } from '../../../core/customer/customer.service';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  template: `
    <div class="profile-container">
      <div class="profile-card" *ngIf="profile() as p">
        <div class="header">
          <h2>Customer Profile</h2>
          <span class="badge" [class.citizen]="p.isCitizen">{{ p.isCitizen ? 'Saudi Citizen' : 'Resident' }}</span>
        </div>

        <div class="info-grid">
          <div class="info-item">
            <label>Full Name</label>
            <span>{{ p.fullName }}</span>
          </div>
          <div class="info-item">
            <label>Email Address</label>
            <span>{{ p.email }}</span>
          </div>
          <div class="info-item">
            <label>National ID / Iqama</label>
            <span>{{ p.nationalId || 'Not Configured' }}</span>
          </div>
          <div class="info-item">
            <label>Account Role</label>
            <span>{{ p.role }}</span>
          </div>
        </div>

        <hr class="divider" />

        <h3>Saudi National Address</h3>

        @if (successMessage()) {
          <div class="success-alert">{{ successMessage() }}</div>
        }

        <form [formGroup]="addressForm" (ngSubmit)="onSaveAddress()">
          <div class="form-row">
            <div class="form-group">
              <label>Building Number</label>
              <input type="text" formControlName="buildingNumber" placeholder="1234" />
            </div>
            <div class="form-group">
              <label>Street</label>
              <input type="text" formControlName="street" placeholder="King Fahd Road" />
            </div>
          </div>

          <div class="form-row">
            <div class="form-group">
              <label>District</label>
              <input type="text" formControlName="district" placeholder="Al Olaya" />
            </div>
            <div class="form-group">
              <label>City</label>
              <input type="text" formControlName="city" placeholder="Riyadh" />
            </div>
          </div>

          <div class="form-row">
            <div class="form-group">
              <label>Postal Code</label>
              <input type="text" formControlName="postalCode" placeholder="12211" />
            </div>
            <div class="form-group">
              <label>Country</label>
              <input type="text" formControlName="country" readonly />
            </div>
          </div>

          <button type="submit" [disabled]="addressForm.invalid || isSaving()">
            {{ isSaving() ? 'Saving Address...' : 'Update Address' }}
          </button>
        </form>
      </div>
    </div>
  `,
  styles: [`
    .profile-container { padding: 2rem; display: flex; justify-content: center; }
    .profile-card { background: #1e1e2d; padding: 2rem; border-radius: 12px; width: 100%; max-width: 650px; color: #fff; box-shadow: 0 8px 24px rgba(0,0,0,0.3); }
    .header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 1.5rem; }
    h2, h3 { margin: 0; color: #fff; }
    .badge { background: #00c588; color: #fff; padding: 0.25rem 0.75rem; border-radius: 20px; font-size: 0.8rem; font-weight: 600; }
    .badge.citizen { background: #3699ff; }
    .info-grid { display: grid; grid-template-columns: 1fr 1fr; gap: 1rem; margin-bottom: 1.5rem; }
    .info-item { display: flex; flex-direction: column; }
    .info-item label { font-size: 0.8rem; color: #b5b5c3; margin-bottom: 0.25rem; }
    .info-item span { font-size: 0.95rem; font-weight: 500; }
    .divider { border: none; border-top: 1px solid #2b2b40; margin: 1.5rem 0; }
    .form-row { display: grid; grid-template-columns: 1fr 1fr; gap: 1rem; }
    .form-group { margin-bottom: 1rem; display: flex; flex-direction: column; }
    label { font-size: 0.85rem; margin-bottom: 0.5rem; color: #b5b5c3; }
    input { background: #151521; border: 1px solid #2b2b40; padding: 0.65rem; border-radius: 6px; color: #fff; outline: none; }
    input:focus { border-color: #3699ff; }
    button { background: #3699ff; border: none; color: #fff; padding: 0.75rem; border-radius: 6px; font-weight: 600; cursor: pointer; margin-top: 1rem; width: 100%; }
    .success-alert { background: #00c588; color: #fff; padding: 0.75rem; border-radius: 6px; font-size: 0.85rem; margin-bottom: 1rem; }
  `]
})
export class ProfileComponent implements OnInit {
  private authService = inject(AuthService);
  private customerService = inject(CustomerService);
  private fb = inject(FormBuilder);

  public profile = signal<CustomerProfileDto | null>(null);
  public isSaving = signal(false);
  public successMessage = signal<string | null>(null);

  public addressForm = this.fb.group({
    buildingNumber: ['', [Validators.required]],
    street: ['', [Validators.required]],
    district: ['', [Validators.required]],
    city: ['', [Validators.required]],
    postalCode: ['', [Validators.required]],
    country: ['Saudi Arabia']
  });

  public ngOnInit(): void {
    const user = this.authService.currentUser();
    if (user?.customerId) {
      this.customerService.getProfile(user.customerId).subscribe(p => {
        this.profile.set(p);
        if (p.address) {
          this.addressForm.patchValue(p.address);
        }
      });
    }
  }

  public onSaveAddress(): void {
    if (this.addressForm.invalid) return;

    const user = this.authService.currentUser();
    if (!user?.customerId) return;

    this.isSaving.set(true);
    this.successMessage.set(null);

    this.customerService.updateAddress(user.customerId, this.addressForm.value as any).subscribe({
      next: (updatedProfile) => {
        this.isSaving.set(false);
        this.profile.set(updatedProfile);
        this.successMessage.set('Saudi National Address updated successfully!');
      },
      error: () => this.isSaving.set(false)
    });
  }
}
