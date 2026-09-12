import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { AuthService } from '../../../core/auth/auth.service';

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    <aside class="sidebar">
      <div class="brand">
        <div class="logo-icon">
          <svg width="22" height="22" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5">
            <path d="M12 2L2 7l10 5 10-5-10-5zM2 17l10 5 10-5M2 12l10 5 10-5"/>
          </svg>
        </div>
        <span class="brand-name" i18n="@@brandName">FinHub</span>
      </div>

      <nav class="nav-menu">
        <a routerLink="/dashboard/accounts" routerLinkActive="active" class="nav-item">
          <svg class="nav-icon" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <path d="M3 9l9-7 9 7v11a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2z"/>
            <polyline points="9 22 9 12 15 12 15 22"/>
          </svg>
          <span i18n="@@nav.home">Home</span>
        </a>

        <a routerLink="/dashboard/accounts" routerLinkActive="active" class="nav-item">
          <svg class="nav-icon" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <rect x="2" y="5" width="20" height="14" rx="2"/>
            <line x1="2" y1="10" x2="22" y2="10"/>
          </svg>
          <span i18n="@@nav.accounts">Accounts &amp; Banking</span>
        </a>

        <a routerLink="/customer/profile" routerLinkActive="active" class="nav-item">
          <svg class="nav-icon" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <path d="M17 21v-2a4 4 0 0 0-4-4H5a4 4 0 0 0-4 4v2"/>
            <circle cx="9" cy="7" r="4"/>
          </svg>
          <span i18n="@@nav.profile">Profile &amp; KYC</span>
        </a>

        <a routerLink="/dashboard/accounts" routerLinkActive="active" class="nav-item">
          <svg class="nav-icon" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <path d="M12 2a10 10 0 1 0 10 10"/>
            <path d="M12 2a10 10 0 0 1 10 10"/>
            <path d="M8 12h8"/>
            <path d="M12 8v8"/>
          </svg>
          <span i18n="@@nav.sama">SAMA Open Banking</span>
        </a>

        <a routerLink="/dashboard/accounts" routerLinkActive="active" class="nav-item">
          <svg class="nav-icon" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <path d="M3 12h7l3-7 3 14 2-7h5"/>
          </svg>
          <span i18n="@@nav.transactions">Transactions &amp; Ledger</span>
        </a>

        <a routerLink="/dashboard/accounts" routerLinkActive="active" class="nav-item">
          <svg class="nav-icon" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <path d="M4 19h16"/>
            <path d="M5 15l4-4 4 4 5-8"/>
          </svg>
          <span i18n="@@nav.budgeting">Budgeting &amp; Alerts</span>
        </a>

        <a routerLink="/dashboard/accounts" routerLinkActive="active" class="nav-item">
          <svg class="nav-icon" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <rect x="3" y="7" width="18" height="13" rx="2"/>
            <path d="M3 10h18"/>
            <path d="M7 15h10"/>
          </svg>
          <span i18n="@@nav.paycore">PayCore Payments</span>
        </a>

        <a routerLink="/dashboard/accounts" routerLinkActive="active" class="nav-item">
          <svg class="nav-icon" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <path d="M12 3l8 4v5c0 5.6-2 8.4-8 10-6-1.6-8-4.4-8-10V7z"/>
            <path d="M9 12l2 2 4-4"/>
          </svg>
          <span i18n="@@nav.fraud">Fraud Engine</span>
        </a>

        <a routerLink="/dashboard/accounts" routerLinkActive="active" class="nav-item">
          <svg class="nav-icon" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <path d="M4 7h16v10H4z"/>
            <path d="M4 17h16v4H4z"/>
          </svg>
          <span i18n="@@nav.reconciliation">Reconciliation</span>
        </a>
      </nav>

      <div class="sidebar-footer">
        <a href="javascript:void(0)" class="nav-item">
          <svg class="nav-icon" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <circle cx="12" cy="12" r="3"/>
            <path d="M19.4 15a1.65 1.65 0 0 0 .33 1.82l.06.06a2 2 0 0 1 0 2.83 2 2 0 0 1-2.83 0l-.06-.06a1.65 1.65 0 0 0-1.82-.33 1.65 1.65 0 0 0-1 1.51V21a2 2 0 0 1-2 2 2 2 0 0 1-2-2v-.09A1.65 1.65 0 0 0 9 19.4a1.65 1.65 0 0 0-1.82.33l-.06.06a2 2 0 0 1-2.83 0 2 2 0 0 1 0-2.83l.06-.06a1.65 1.65 0 0 0 .33-1.82 1.65 1.65 0 0 0-1.51-1H3a2 2 0 0 1-2-2 2 2 0 0 1 2-2h.09A1.65 1.65 0 0 0 4.6 9a1.65 1.65 0 0 0-.33-1.82l-.06-.06a2 2 0 0 1 0-2.83 2 2 0 0 1 2.83 0l.06.06a1.65 1.65 0 0 0 1.82.33H9a1.65 1.65 0 0 0 1-1.51V3a2 2 0 0 1 2-2 2 2 0 0 1 2 2v.09a1.65 1.65 0 0 0 1 1.51 1.65 1.65 0 0 0 1.82-.33l.06-.06a2 2 0 0 1 2.83 0 2 2 0 0 1 0 2.83l-.06.06a1.65 1.65 0 0 0-.33 1.82V9a1.65 1.65 0 0 0 1.51 1H21a2 2 0 0 1 2 2 2 2 0 0 1-2 2h-.09a1.65 1.65 0 0 0-1.51 1z"/>
          </svg>
          <span i18n="@@nav.settings">Settings</span>
        </a>
        <button class="btn-logout" (click)="logout()" i18n="@@nav.signOut">Sign Out</button>
      </div>
    </aside>
  `,
  styles: [`
    .sidebar { width: 220px; background: #ffffff; border-right: 1px solid #e2e8f0; height: 100vh; display: flex; flex-direction: column; padding: 1.25rem 1rem; position: fixed; top: 0; inset-inline-start: 0; z-index: 100; font-family: 'Inter', system-ui, sans-serif; }
    .brand { display: flex; align-items: center; gap: 0.75rem; padding: 0.5rem 0.5rem 1.5rem 0.5rem; }
    .logo-icon { width: 36px; height: 36px; background: #2563eb; color: #fff; border-radius: 10px; display: flex; align-items: center; justify-content: center; flex-shrink: 0; }
    .brand-name { font-size: 1.25rem; font-weight: 700; color: #0f172a; letter-spacing: -0.02em; }
    .nav-menu { display: flex; flex-direction: column; gap: 0.25rem; flex: 1; overflow-y: auto; }
    .nav-item { display: flex; align-items: center; gap: 0.75rem; padding: 0.65rem 0.85rem; color: #64748b; font-size: 0.875rem; font-weight: 500; border-radius: 10px; text-decoration: none; transition: all 0.15s ease; }
    .nav-item:hover { color: #2563eb; background: #f1f5f9; }
    .nav-item.active { color: #2563eb; background: #eff6ff; font-weight: 600; }
    .nav-icon { width: 18px; height: 18px; flex-shrink: 0; }
    .sidebar-footer { border-top: 1px solid #e2e8f0; padding-top: 1rem; display: flex; flex-direction: column; gap: 0.5rem; }
    .btn-logout { background: #fef2f2; color: #dc2626; border: none; padding: 0.6rem 0.85rem; border-radius: 8px; font-weight: 600; font-size: 0.8rem; cursor: pointer; text-align: start; }
    .btn-logout:hover { background: #fee2e2; }
  `]
})
export class SidebarComponent {
  private authService = inject(AuthService);
  logout(): void { this.authService.logout(); }
}
