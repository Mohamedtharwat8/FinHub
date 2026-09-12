import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../../core/auth/auth.service';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [CommonModule],
  template: `
    <header class="top-header">
      <div class="search-container">
        <svg class="search-icon" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
          <circle cx="11" cy="11" r="8"/>
          <line x1="21" y1="21" x2="16.65" y2="16.65"/>
        </svg>
        <input
          type="text"
          class="search-input"
          i18n-placeholder="@@header.searchPlaceholder"
          placeholder="Search accounts, transactions..."
        />
      </div>

      <div class="header-actions">
        <button class="icon-btn" [attr.aria-label]="helpLabel">
          <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" width="20" height="20">
            <circle cx="12" cy="12" r="10"/>
            <path d="M9.09 9a3 3 0 0 1 5.83 1c0 2-3 3-3 3"/>
            <line x1="12" y1="17" x2="12.01" y2="17"/>
          </svg>
        </button>

        <button class="icon-btn" [attr.aria-label]="notificationsLabel">
          <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" width="20" height="20">
            <path d="M21 15a2 2 0 0 1-2 2H7l-4 4V5a2 2 0 0 1 2-2h14a2 2 0 0 1 2 2z"/>
          </svg>
        </button>

        <div class="user-profile">
          <div class="avatar">{{ userInitials() }}</div>
        </div>
      </div>
    </header>
  `,
  styles: [`
    .top-header { height: 64px; background: #ffffff; border-bottom: 1px solid #e2e8f0; display: flex; align-items: center; justify-content: space-between; padding: 0 1.75rem; margin-inline-start: 220px; position: sticky; top: 0; z-index: 90; font-family: 'Inter', system-ui, sans-serif; }
    .search-container { position: relative; width: 340px; }
    .search-icon { position: absolute; inset-inline-start: 1rem; top: 50%; transform: translateY(-50%); width: 16px; height: 16px; color: #94a3b8; }
    .search-input { width: 100%; background: #f8fafc; border: 1px solid #f1f5f9; padding: 0.55rem 1rem 0.55rem 2.5rem; border-radius: 20px; font-size: 0.875rem; color: #1e293b; outline: none; transition: all 0.15s ease; }
    :host-context([dir="rtl"]) .search-input { padding: 0.55rem 2.5rem 0.55rem 1rem; }
    .search-input:focus { background: #ffffff; border-color: #cbd5e1; box-shadow: 0 0 0 3px rgba(37,99,235,0.1); }
    .header-actions { display: flex; align-items: center; gap: 0.75rem; }
    .icon-btn { background: #f8fafc; border: 1px solid #f1f5f9; width: 38px; height: 38px; border-radius: 50%; display: flex; align-items: center; justify-content: center; color: #64748b; cursor: pointer; transition: all 0.15s ease; }
    .icon-btn:hover { background: #f1f5f9; color: #1e293b; }
    .user-profile { display: flex; align-items: center; margin-inline-start: 0.5rem; }
    .avatar { width: 38px; height: 38px; background: #1e293b; color: #ffffff; font-size: 0.875rem; font-weight: 600; border-radius: 50%; display: flex; align-items: center; justify-content: center; }
  `]
})
export class HeaderComponent {
  private authService = inject(AuthService);

  // $localize for runtime strings used in attribute bindings
  readonly helpLabel = $localize`:@@header.help:Help & Support`;
  readonly notificationsLabel = $localize`:@@header.notifications:Notifications`;

  userInitials(): string {
    const user = this.authService.currentUser();
    if (!user?.fullName) return 'U';
    const parts = user.fullName.trim().split(' ');
    return parts.length >= 2
      ? (parts[0][0] + parts[parts.length - 1][0]).toUpperCase()
      : user.fullName.substring(0, 2).toUpperCase();
  }
}
