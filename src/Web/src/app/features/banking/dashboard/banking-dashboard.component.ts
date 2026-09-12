import { Component, OnInit, inject, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { SidebarComponent } from '../../../shared/components/sidebar/sidebar.component';
import { HeaderComponent } from '../../../shared/components/header/header.component';
import { AuthService } from '../../../core/auth/auth.service';
import { BankingService, BankAccountDto, TransactionDto } from '../../../core/banking/banking.service';

@Component({
  selector: 'app-banking-dashboard',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule, SidebarComponent, HeaderComponent],
  template: `
    <div class="govera-layout">
      <!-- Left Sidebar -->
      <app-sidebar></app-sidebar>

      <!-- Main Layout Body -->
      <div class="main-body">
        <!-- Top Header -->
        <app-header></app-header>

        <!-- 3-Column Content Wrapper -->
        <div class="content-container">
          <!-- Center Main Content -->
          <main class="center-content">
            <!-- Alert Banner -->
            <div class="trial-banner">
              <div class="banner-info">
                <div class="info-circle">i</div>
                <span>During your trial you have 25 free contacts. You can start your subscription now if you would like to import or email the 1000 contacts that come with your plan.</span>
              </div>
              <button class="btn-banner-action">Start your subscription <span>›</span></button>
            </div>

            <!-- Welcome Header -->
            <div class="welcome-header">
              <h1>Hey there, {{ userName() }}</h1>
              <p class="subtext">Here's what's happening in your Govera account today</p>
            </div>

            <!-- Stat Metric Cards Row -->
            <div class="kpi-grid">
              <div class="kpi-card">
                <div class="kpi-icon blue">
                  <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" width="18" height="18">
                    <path d="M16 21v-2a4 4 0 0 0-4-4H5a4 4 0 0 0-4 4v2"/><circle cx="8.5" cy="7" r="4"/><line x1="20" y1="8" x2="20" y2="14"/><line x1="23" y1="11" x2="17" y2="11"/>
                  </svg>
                </div>
                <div class="kpi-label">Account Created</div>
                <div class="kpi-val-row">
                  <span class="kpi-value">{{ totalBalance() > 0 ? (totalBalance() | number:'1.0-0') : '4,861' }}</span>
                  <span class="badge-trend green">↑ 20%</span>
                </div>
              </div>

              <div class="kpi-card">
                <div class="kpi-icon blue">
                  <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" width="18" height="18">
                    <path d="M17 21v-2a4 4 0 0 0-4-4H5a4 4 0 0 0-4 4v2"/><circle cx="9" cy="7" r="4"/>
                  </svg>
                </div>
                <div class="kpi-label">People Created</div>
                <div class="kpi-val-row">
                  <span class="kpi-value">{{ accounts().length > 0 ? accounts().length : '424' }}</span>
                  <span class="badge-trend red">↓ 40%</span>
                </div>
              </div>

              <div class="kpi-card">
                <div class="kpi-icon blue">
                  <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" width="18" height="18">
                    <path d="M21 15a2 2 0 0 1-2 2H7l-4 4V5a2 2 0 0 1 2-2h14a2 2 0 0 1 2 2z"/>
                  </svg>
                </div>
                <div class="kpi-label">Open Ticket</div>
                <div class="kpi-val-row">
                  <span class="kpi-value">24,258</span>
                  <span class="badge-trend green">↑ 16%</span>
                </div>
              </div>
            </div>

            <!-- Guidance / Integration Cards Section -->
            <div class="guidance-card">
              <div class="guidance-header">
                <div class="guidance-title-row">
                  <div class="kpi-icon blue">
                    <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" width="18" height="18">
                      <path d="M12 2L2 7l10 5 10-5-10-5zM2 17l10 5 10-5M2 12l10 5 10-5"/>
                    </svg>
                  </div>
                  <div>
                    <h3>A little guidance</h3>
                    <p class="subtext">Since you told us you're building a <strong>community...</strong></p>
                  </div>
                </div>
                <button class="btn-close-guidance">✕</button>
              </div>

              <div class="guidance-grid">
                <div class="integration-box">
                  <div class="int-header">
                    <div class="int-brand circle">C</div>
                    <span class="arrow-icon">↗</span>
                  </div>
                  <h4>Integrated Govera with Circle</h4>
                  <p>Monetize and manage your Circle community with our Single Sign On (SSO) integration</p>
                </div>

                <div class="integration-box">
                  <div class="int-header">
                    <div class="int-brand discord">
                      <svg viewBox="0 0 24 24" fill="currentColor" width="20" height="20"><path d="M20.317 4.37a19.791 19.791 0 0 0-4.885-1.515.074.074 0 0 0-.079.037c-.21.375-.444.864-.608 1.25a18.27 18.27 0 0 0-5.487 0 12.64 12.64 0 0 0-.617-1.25.077.077 0 0 0-.079-.037A19.736 19.736 0 0 0 3.677 4.37a.07.07 0 0 0-.032.027C.533 9.046-.32 13.58.099 18.057a.082.082 0 0 0 .031.057 19.9 19.9 0 0 0 5.993 3.03.078.078 0 0 0 .084-.028c.462-.63.874-1.295 1.226-1.994.021-.041.001-.09-.041-.106a13.107 13.107 0 0 1-1.872-.892.077.077 0 0 1-.008-.128 10.2 10.2 0 0 0 .372-.292.074.074 0 0 1 .077-.01c3.928 1.793 8.18 1.793 12.061 0a.074.074 0 0 1 .078.01c.12.098.246.198.373.292a.077.077 0 0 1-.006.127 12.299 12.299 0 0 1-1.873.893.077.077 0 0 0-.041.107c.36.698.772 1.362 1.225 1.993a.076.076 0 0 0 .084.028 19.839 19.839 0 0 0 6.002-3.03.077.077 0 0 0 .032-.054c.5-5.177-.838-9.674-3.549-13.66a.061.061 0 0 0-.031-.028zM8.02 15.33c-1.183 0-2.157-1.085-2.157-2.419 0-1.333.956-2.419 2.157-2.419 1.21 0 2.176 1.096 2.157 2.42 0 1.333-.956 2.418-2.157 2.418zm7.975 0c-1.183 0-2.157-1.085-2.157-2.419 0-1.333.955-2.419 2.157-2.419 1.21 0 2.176 1.096 2.157 2.42 0 1.333-.946 2.418-2.157 2.418z"/></svg>
                    </div>
                    <span class="arrow-icon">↗</span>
                  </div>
                  <h4>Integrated Govera with Discord</h4>
                  <p>Monetize and manage as Discord server. Automatically invite, manage roles, and remove members if they cancel</p>
                </div>
              </div>
            </div>

            <!-- Accounts Action Control Header -->
            <div class="accounts-control-header">
              <div>
                <h2>Your Bank Accounts (SAMA IBAN)</h2>
                <p class="subtext">Select an account to view real-time ledger entries</p>
              </div>
              <button class="btn-primary" (click)="openCreateModal()">+ Open New Account</button>
            </div>

            <!-- Accounts Grid -->
            <div class="accounts-grid">
              @for (acc of accounts(); track acc.id) {
                <div class="account-card" [class.selected]="selectedAccount()?.id === acc.id" (click)="selectAccount(acc)">
                  <div class="acc-header">
                    <span class="acc-type-badge">{{ acc.type }}</span>
                    <span class="acc-status-badge">{{ acc.status }}</span>
                  </div>
                  <div class="acc-balance">{{ acc.balanceAmount | number:'1.2-2' }} <span class="curr">{{ acc.currency }}</span></div>
                  <div class="acc-iban">{{ acc.iban }}</div>
                  <div class="acc-actions">
                    <button class="btn-action deposit" (click)="openDepositModal(acc); $event.stopPropagation()">+ Deposit</button>
                    <button class="btn-action withdraw" (click)="openWithdrawModal(acc); $event.stopPropagation()">- Withdraw</button>
                  </div>
                </div>
              } @empty {
                <div class="empty-state">No bank accounts found. Click "+ Open New Account" to generate your Saudi IBAN.</div>
              }
            </div>

            <!-- Activity Section -->
            <div class="activity-section">
              <div class="activity-header">
                <h2>Activity</h2>
                <div class="activity-filter">All (20) ▾</div>
              </div>

              <div class="activity-group">
                <div class="date-tag">🗓 Today</div>

                <div class="activity-list">
                  <div class="activity-item">
                    <div class="dot blue"></div>
                    <div class="act-text">Alex Chen addressed the support ticket regarding the 'Export Function Glitch'.</div>
                    <div class="act-time">3:58 pm</div>
                  </div>

                  <div class="activity-item">
                    <div class="dot blue"></div>
                    <div class="act-text">Jordan Lee resolved the support ticket titled 'Export Tool Malfunction'.</div>
                    <div class="act-time">3:56 pm</div>
                  </div>

                  <div class="activity-item">
                    <div class="dot blue"></div>
                    <div class="act-text">Taylor Kim updated.</div>
                    <div class="act-time">3:45 pm</div>
                  </div>

                  <div class="activity-item">
                    <div class="dot blue"></div>
                    <div class="act-text">Morgan Smith updated the support ticket concerning the 'Export Capability Issue'.</div>
                    <div class="act-time">3:41 pm</div>
                  </div>

                  <div class="activity-item">
                    <div class="dot blue"></div>
                    <div class="act-text">Jamie Park reviewed the support ticket named 'Export Functionality Problem'.</div>
                    <div class="act-time">3:40 pm</div>
                  </div>

                  <div class="activity-item">
                    <div class="dot blue"></div>
                    <div class="act-text">Casey Wong handled the support ticket about the 'Export Feature Error'.</div>
                    <div class="act-time">3:33 pm</div>
                  </div>

                  <div class="activity-item">
                    <div class="dot blue"></div>
                    <div class="act-text">Riley Johnson checked the support ticket for the 'Export Feature Concern'.</div>
                    <div class="act-time">3:32 pm</div>
                  </div>
                </div>
              </div>

              <div class="activity-group">
                <div class="date-tag">🗓 December 13</div>

                <div class="activity-list">
                  <div class="activity-item">
                    <div class="dot blue"></div>
                    <div class="act-text">Avery Davis examined the support ticket titled 'Export Feature Challenge'.</div>
                    <div class="act-time">3:18 pm</div>
                  </div>

                  <div class="activity-item">
                    <div class="dot blue"></div>
                    <div class="act-text">Quinn Taylor updated the support ticket about the 'Export Functionality Glitch'.</div>
                    <div class="act-time">3:17 pm</div>
                  </div>

                  <div class="activity-item">
                    <div class="dot blue"></div>
                    <div class="act-text">Peyton White addressed the support ticket concerning the 'Export Feature Trouble'.</div>
                    <div class="act-time">3:17 pm</div>
                  </div>

                  <div class="activity-item">
                    <div class="dot blue"></div>
                    <div class="act-text">Charlie Green resolved the support ticket labeled 'Export Feature Dilemma'.</div>
                    <div class="act-time">2:41 pm</div>
                  </div>
                </div>
              </div>

              <button class="btn-load-more">Load more</button>
            </div>
          </main>

          <!-- Right Analytics Column -->
          <aside class="right-analytics">
            <!-- Engagement / Billing Pill Switcher -->
            <div class="toggle-pill-row">
              <button class="toggle-btn active">Engagement</button>
              <button class="toggle-btn">Billing</button>
            </div>

            <!-- Date Controls -->
            <div class="date-control-row">
              <span class="date-nav">‹ Q4 2024 ›</span>
              <button class="btn-dropdown">Button ▾</button>
            </div>

            <!-- Chart Card 1: People Sparkline -->
            <div class="chart-card">
              <div class="chart-card-header">
                <div>
                  <div class="chart-number">424</div>
                  <div class="chart-title">People</div>
                  <div class="chart-sub">as of 14-Dec-2025</div>
                </div>
                <button class="btn-share">Share</button>
              </div>

              <div class="sparkline-wrapper">
                <!-- Tooltip Mock -->
                <div class="chart-tooltip">
                  <div class="tt-date">17-Dec-2025</div>
                  <div class="tt-val">People: 245</div>
                </div>
                <svg viewBox="0 0 300 100" class="line-chart">
                  <path d="M0 60 Q 40 20 80 45 T 160 50 T 240 10 L 300 60" fill="none" stroke="#2563eb" stroke-width="2.5"/>
                  <circle cx="255" cy="18" r="4.5" fill="#2563eb" stroke="#ffffff" stroke-width="2"/>
                </svg>
                <div class="chart-x-labels">
                  <span>Oct 1</span>
                  <span>Oct 29</span>
                  <span>Nov 26</span>
                  <span>Dec 24</span>
                </div>
              </div>
            </div>

            <!-- Chart Card 2: Accounts Curve -->
            <div class="chart-card">
              <div class="chart-card-header">
                <div>
                  <div class="chart-number">4,861</div>
                  <div class="chart-title">Accounts</div>
                  <div class="chart-sub">as of 14-Dec-2025</div>
                </div>
                <button class="btn-share">Share</button>
              </div>

              <div class="sparkline-wrapper">
                <svg viewBox="0 0 300 90" class="line-chart">
                  <path d="M0 70 Q 50 30 100 60 T 200 65 T 280 15" fill="none" stroke="#2563eb" stroke-width="2.5"/>
                </svg>
                <div class="chart-x-labels">
                  <span>Oct 1</span>
                  <span>Oct 29</span>
                  <span>Nov 26</span>
                  <span>Dec 24</span>
                </div>
              </div>
            </div>

            <!-- Chart Card 3: Emails Volume Bar Chart -->
            <div class="chart-card">
              <div class="chart-card-header">
                <div>
                  <div class="chart-number">2,245</div>
                  <div class="chart-title">Emails</div>
                  <div class="chart-sub">as of Q4 2025</div>
                </div>
                <button class="btn-share">Share ▾</button>
              </div>

              <div class="bar-chart-container">
                <div class="bars-row">
                  <div class="bar-col" style="height: 40%;"></div>
                  <div class="bar-col" style="height: 75%;"></div>
                  <div class="bar-col active" style="height: 50%;">
                    <div class="bar-pill-tag">1,245</div>
                  </div>
                  <div class="bar-col" style="height: 85%;"></div>
                </div>
                <div class="chart-x-labels">
                  <span>Oct 1</span>
                  <span>Oct 29</span>
                  <span>Nov 26</span>
                  <span>Dec 24</span>
                </div>
              </div>
            </div>

            <!-- Chart Card 4: Tickets Volume Bar Chart -->
            <div class="chart-card">
              <div class="chart-card-header">
                <div>
                  <div class="chart-number">7,254</div>
                  <div class="chart-title">Tickets</div>
                  <div class="chart-sub">as of 14-Dec-2025</div>
                </div>
                <button class="btn-share">Share ▾</button>
              </div>

              <div class="bar-chart-container">
                <div class="bars-row">
                  <div class="bar-col" style="height: 60%;"></div>
                  <div class="bar-col" style="height: 45%;"></div>
                  <div class="bar-col active" style="height: 90%;">
                    <div class="bar-pill-tag">7,254</div>
                  </div>
                  <div class="bar-col" style="height: 65%;"></div>
                </div>
                <div class="chart-x-labels">
                  <span>Oct 1</span>
                  <span>Oct 29</span>
                  <span>Nov 26</span>
                  <span>Dec 24</span>
                </div>
              </div>
            </div>
          </aside>
        </div>
      </div>

      <!-- Create Account Modal -->
      @if (showCreateModal()) {
        <div class="modal-backdrop">
          <div class="modal-card">
            <h3>Open New Bank Account</h3>
            <form [formGroup]="createForm" (ngSubmit)="submitCreateAccount()">
              <div class="form-group">
                <label>Account Category</label>
                <select formControlName="type">
                  <option value="Savings">Savings Account</option>
                  <option value="Current">Current Checking Account</option>
                  <option value="Investment">Investment Portfolio Account</option>
                  <option value="Credit">Credit Line Account</option>
                </select>
              </div>

              <div class="form-group">
                <label>Initial Opening Deposit (SAR)</label>
                <input type="number" formControlName="initialDeposit" min="0" placeholder="1000" />
              </div>

              <div class="modal-actions">
                <button type="button" class="btn-cancel" (click)="closeCreateModal()">Cancel</button>
                <button type="submit" class="btn-primary" [disabled]="createForm.invalid || isSubmitting()">
                  {{ isSubmitting() ? 'Opening Account...' : 'Confirm & Open' }}
                </button>
              </div>
            </form>
          </div>
        </div>
      }

      <!-- Deposit / Withdraw Modal -->
      @if (showActionModal()) {
        <div class="modal-backdrop">
          <div class="modal-card">
            <h3>{{ actionType() === 'deposit' ? 'Deposit Money' : 'Withdraw Money' }}</h3>
            <p class="modal-sub">Account: {{ targetAccount()?.iban }}</p>

            <form [formGroup]="actionForm" (ngSubmit)="submitAction()">
              <div class="form-group">
                <label>Amount (SAR)</label>
                <input type="number" formControlName="amount" min="1" placeholder="500" />
              </div>

              <div class="form-group">
                <label>Description / Note</label>
                <input type="text" formControlName="description" placeholder="ATM Deposit or Transfer" />
              </div>

              <div class="modal-actions">
                <button type="button" class="btn-cancel" (click)="closeActionModal()">Cancel</button>
                <button type="submit" class="btn-primary" [disabled]="actionForm.invalid || isSubmitting()">
                  {{ isSubmitting() ? 'Processing...' : 'Submit Transaction' }}
                </button>
              </div>
            </form>
          </div>
        </div>
      }
    </div>
  `,
  styles: [`
    .govera-layout { min-height: 100vh; background: #f8fafc; font-family: 'Inter', system-ui, -apple-system, sans-serif; color: #0f172a; }
    .main-body { display: flex; flex-direction: column; min-height: 100vh; }
    .content-container { display: flex; margin-left: 220px; flex: 1; }
    
    /* Center Main Content */
    .center-content { flex: 1; padding: 1.5rem 2rem; background: #ffffff; border-right: 1px solid #e2e8f0; min-width: 0; }
    
    /* Alert Banner */
    .trial-banner { background: #eff6ff; border: 1px solid #dbeafe; padding: 0.85rem 1.25rem; border-radius: 12px; display: flex; align-items: center; justify-content: space-between; gap: 1rem; margin-bottom: 1.5rem; }
    .banner-info { display: flex; align-items: center; gap: 0.75rem; font-size: 0.85rem; color: #1e40af; }
    .info-circle { width: 22px; height: 22px; background: #2563eb; color: #fff; border-radius: 50%; display: flex; align-items: center; justify-content: center; font-size: 0.75rem; font-weight: 700; flex-shrink: 0; }
    .btn-banner-action { background: #2563eb; color: #fff; border: none; padding: 0.5rem 1rem; border-radius: 20px; font-weight: 600; font-size: 0.8rem; cursor: pointer; white-space: nowrap; }
    .btn-banner-action:hover { background: #1d4ed8; }

    /* Welcome Header */
    .welcome-header { margin-bottom: 1.5rem; }
    .welcome-header h1 { font-size: 1.6rem; font-weight: 700; color: #0f172a; margin: 0; letter-spacing: -0.02em; }
    .subtext { color: #64748b; font-size: 0.875rem; margin-top: 0.25rem; }

    /* KPI Grid */
    .kpi-grid { display: grid; grid-template-columns: repeat(3, 1fr); gap: 1rem; margin-bottom: 1.5rem; }
    .kpi-card { background: #f8fafc; border: 1px solid #f1f5f9; padding: 1.25rem; border-radius: 12px; }
    .kpi-icon { width: 32px; height: 32px; border-radius: 8px; display: flex; align-items: center; justify-content: center; margin-bottom: 0.75rem; }
    .kpi-icon.blue { background: #dbeafe; color: #2563eb; }
    .kpi-label { font-size: 0.8rem; color: #64748b; font-weight: 500; }
    .kpi-val-row { display: flex; align-items: center; justify-content: space-between; margin-top: 0.25rem; }
    .kpi-value { font-size: 1.5rem; font-weight: 700; color: #0f172a; }
    .badge-trend { font-size: 0.75rem; font-weight: 600; padding: 0.15rem 0.5rem; border-radius: 12px; }
    .badge-trend.green { background: #dcfce7; color: #166534; }
    .badge-trend.red { background: #fee2e2; color: #991b1b; }

    /* Guidance Card */
    .guidance-card { background: #f0f9ff; border: 1px solid #e0f2fe; padding: 1.25rem; border-radius: 12px; margin-bottom: 2rem; }
    .guidance-header { display: flex; justify-content: space-between; align-items: flex-start; margin-bottom: 1rem; }
    .guidance-title-row { display: flex; align-items: center; gap: 0.75rem; }
    .guidance-title-row h3 { margin: 0; font-size: 1.1rem; font-weight: 700; }
    .btn-close-guidance { background: none; border: none; color: #94a3b8; font-size: 1rem; cursor: pointer; }
    .guidance-grid { display: grid; grid-template-columns: 1fr 1fr; gap: 1rem; }
    .integration-box { background: #ffffff; border: 1px solid #e2e8f0; padding: 1.1rem; border-radius: 10px; }
    .int-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 0.75rem; }
    .int-brand { width: 32px; height: 32px; border-radius: 8px; display: flex; align-items: center; justify-content: center; font-weight: 700; color: #fff; }
    .int-brand.circle { background: #2563eb; }
    .int-brand.discord { background: #5865f2; }
    .arrow-icon { color: #64748b; font-size: 1rem; font-weight: 600; }
    .integration-box h4 { margin: 0 0 0.35rem 0; font-size: 0.9rem; font-weight: 600; color: #0f172a; }
    .integration-box p { margin: 0; font-size: 0.8rem; color: #64748b; line-height: 1.4; }

    /* Accounts Control Header & Grid */
    .accounts-control-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 1rem; }
    .accounts-control-header h2 { font-size: 1.25rem; font-weight: 700; margin: 0; }
    .btn-primary { background: #2563eb; color: #ffffff; border: none; padding: 0.65rem 1.25rem; border-radius: 8px; font-weight: 600; cursor: pointer; }
    .btn-primary:hover { background: #1d4ed8; }
    .accounts-grid { display: grid; grid-template-columns: repeat(auto-fit, minmax(280px, 1fr)); gap: 1rem; margin-bottom: 2rem; }
    .account-card { background: #ffffff; border: 1px solid #e2e8f0; padding: 1.1rem; border-radius: 12px; cursor: pointer; transition: all 0.15s ease; }
    .account-card:hover, .account-card.selected { border-color: #2563eb; box-shadow: 0 4px 12px rgba(37, 99, 235, 0.08); }
    .acc-header { display: flex; justify-content: space-between; margin-bottom: 0.5rem; }
    .acc-type-badge { background: #eff6ff; color: #2563eb; padding: 0.2rem 0.5rem; border-radius: 6px; font-size: 0.75rem; font-weight: 600; }
    .acc-status-badge { background: #dcfce7; color: #166534; padding: 0.2rem 0.5rem; border-radius: 6px; font-size: 0.75rem; font-weight: 600; }
    .acc-balance { font-size: 1.4rem; font-weight: 700; color: #0f172a; margin-bottom: 0.2rem; }
    .curr { font-size: 0.9rem; color: #2563eb; }
    .acc-iban { font-family: monospace; font-size: 0.8rem; color: #64748b; margin-bottom: 0.85rem; }
    .acc-actions { display: flex; gap: 0.5rem; }
    .btn-action { flex: 1; padding: 0.45rem; border-radius: 6px; border: none; font-weight: 600; font-size: 0.75rem; cursor: pointer; }
    .btn-action.deposit { background: #10b981; color: #ffffff; }
    .btn-action.withdraw { background: #ef4444; color: #ffffff; }

    /* Activity Section */
    .activity-section { margin-top: 2rem; }
    .activity-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 1rem; }
    .activity-header h2 { font-size: 1.2rem; font-weight: 700; margin: 0; }
    .activity-filter { background: #f1f5f9; padding: 0.35rem 0.75rem; border-radius: 16px; font-size: 0.8rem; color: #475569; font-weight: 500; cursor: pointer; }
    .activity-group { margin-bottom: 1.25rem; }
    .date-tag { font-size: 0.8rem; font-weight: 600; color: #64748b; margin-bottom: 0.65rem; background: #f8fafc; padding: 0.3rem 0.6rem; border-radius: 6px; display: inline-block; }
    .activity-list { display: flex; flex-direction: column; gap: 0.65rem; }
    .activity-item { display: flex; align-items: center; gap: 0.85rem; font-size: 0.85rem; }
    .dot { width: 8px; height: 8px; border-radius: 50%; flex-shrink: 0; }
    .dot.blue { background: #2563eb; }
    .act-text { color: #334155; flex: 1; }
    .act-time { color: #94a3b8; font-size: 0.8rem; }
    .btn-load-more { width: 100%; background: #f1f5f9; border: none; color: #475569; padding: 0.65rem; border-radius: 20px; font-weight: 600; font-size: 0.8rem; cursor: pointer; margin-top: 1rem; }
    .btn-load-more:hover { background: #e2e8f0; }

    /* Right Analytics Column */
    .right-analytics { width: 340px; background: #ffffff; padding: 1.5rem 1.25rem; display: flex; flex-direction: column; gap: 1.5rem; flex-shrink: 0; }
    .toggle-pill-row { background: #f1f5f9; padding: 0.25rem; border-radius: 20px; display: flex; }
    .toggle-btn { flex: 1; background: none; border: none; padding: 0.45rem; border-radius: 16px; font-size: 0.8rem; font-weight: 600; color: #64748b; cursor: pointer; }
    .toggle-btn.active { background: #0f172a; color: #ffffff; }
    .date-control-row { display: flex; justify-content: space-between; align-items: center; }
    .date-nav { font-size: 0.85rem; font-weight: 600; color: #334155; }
    .btn-dropdown { background: #f8fafc; border: 1px solid #e2e8f0; padding: 0.35rem 0.75rem; border-radius: 6px; font-size: 0.8rem; color: #475569; cursor: pointer; }
    
    /* Chart Cards */
    .chart-card { background: #ffffff; border: 1px solid #e2e8f0; padding: 1.1rem; border-radius: 12px; box-shadow: 0 1px 3px rgba(0,0,0,0.03); }
    .chart-card-header { display: flex; justify-content: space-between; align-items: flex-start; margin-bottom: 0.85rem; }
    .chart-number { font-size: 1.6rem; font-weight: 800; color: #0f172a; line-height: 1; }
    .chart-title { font-size: 0.85rem; font-weight: 600; color: #334155; margin-top: 0.2rem; }
    .chart-sub { font-size: 0.75rem; color: #94a3b8; }
    .btn-share { background: #f8fafc; border: 1px solid #e2e8f0; padding: 0.25rem 0.6rem; border-radius: 6px; font-size: 0.75rem; color: #475569; cursor: pointer; }
    .sparkline-wrapper { position: relative; margin-top: 0.5rem; }
    .line-chart { width: 100%; height: 75px; overflow: visible; }
    .chart-x-labels { display: flex; justify-content: space-between; font-size: 0.7rem; color: #94a3b8; margin-top: 0.4rem; }
    .chart-tooltip { position: absolute; top: -10px; right: 20px; background: #2563eb; color: #fff; padding: 0.4rem 0.6rem; border-radius: 6px; font-size: 0.7rem; font-weight: 600; box-shadow: 0 4px 10px rgba(37, 99, 235, 0.3); z-index: 10; }
    .tt-date { opacity: 0.8; font-size: 0.65rem; }
    .bar-chart-container { margin-top: 0.75rem; }
    .bars-row { display: flex; align-items: flex-end; justify-content: space-around; height: 110px; border-bottom: 1px solid #f1f5f9; padding-bottom: 0.2rem; }
    .bar-col { width: 36px; background: #dbeafe; border-radius: 6px 6px 0 0; position: relative; transition: all 0.2s ease; }
    .bar-col.active { background: #2563eb; }
    .bar-pill-tag { position: absolute; top: -24px; left: 50%; transform: translateX(-50%); background: #2563eb; color: #fff; padding: 0.15rem 0.4rem; border-radius: 4px; font-size: 0.65rem; font-weight: 700; white-space: nowrap; }

    /* Modals */
    .modal-backdrop { position: fixed; top: 0; left: 0; right: 0; bottom: 0; background: rgba(15, 23, 42, 0.6); display: flex; justify-content: center; align-items: center; z-index: 1000; }
    .modal-card { background: #ffffff; padding: 2rem; border-radius: 12px; width: 100%; max-width: 440px; box-shadow: 0 20px 25px -5px rgba(0, 0, 0, 0.1); }
    .modal-card h3 { margin: 0 0 0.5rem 0; font-size: 1.25rem; font-weight: 700; color: #0f172a; }
    .modal-sub { color: #64748b; font-size: 0.85rem; margin-bottom: 1.25rem; }
    .form-group { margin-bottom: 1.1rem; display: flex; flex-direction: column; }
    label { font-size: 0.85rem; margin-bottom: 0.4rem; color: #475569; font-weight: 500; }
    input, select { background: #f8fafc; border: 1px solid #cbd5e1; padding: 0.75rem; border-radius: 8px; color: #0f172a; outline: none; font-size: 0.9rem; }
    .modal-actions { display: flex; justify-content: flex-end; gap: 0.75rem; margin-top: 1.25rem; }
    .btn-cancel { background: #f1f5f9; color: #475569; border: none; padding: 0.75rem 1.25rem; border-radius: 8px; cursor: pointer; font-weight: 600; }
    .empty-state { grid-column: 1 / -1; padding: 2rem; background: #f8fafc; border: 1px dashed #cbd5e1; text-align: center; border-radius: 12px; color: #64748b; }
  `]
})
export class BankingDashboardComponent implements OnInit {
  private authService = inject(AuthService);
  private bankingService = inject(BankingService);
  private fb = inject(FormBuilder);

  public accounts = signal<BankAccountDto[]>([]);
  public selectedAccount = signal<BankAccountDto | null>(null);
  public transactions = signal<TransactionDto[]>([]);

  public showCreateModal = signal(false);
  public showActionModal = signal(false);
  public actionType = signal<'deposit' | 'withdraw'>('deposit');
  public targetAccount = signal<BankAccountDto | null>(null);

  public isSubmitting = signal(false);
  public errorMessage = signal<string | null>(null);
  public successMessage = signal<string | null>(null);

  public totalBalance = computed(() => this.accounts().reduce((sum, a) => sum + a.balanceAmount, 0));
  public userName = computed(() => this.authService.currentUser()?.fullName?.split(' ')[0] || 'User');

  public createForm = this.fb.group({
    type: ['Savings', [Validators.required]],
    initialDeposit: [1000, [Validators.required, Validators.min(0)]]
  });

  public actionForm = this.fb.group({
    amount: [500, [Validators.required, Validators.min(1)]],
    description: ['Cash Deposit', [Validators.required]]
  });

  ngOnInit(): void {
    this.loadAccounts();
  }

  public loadAccounts(): void {
    const user = this.authService.currentUser();
    if (!user) return;

    this.bankingService.getAccountsByCustomer(user.customerId).subscribe({
      next: (res) => {
        this.accounts.set(res);
        if (res.length > 0 && !this.selectedAccount()) {
          this.selectAccount(res[0]);
        }
      },
      error: (err) => this.errorMessage.set(err.error?.error || 'Failed to load bank accounts.')
    });
  }

  public selectAccount(acc: BankAccountDto): void {
    this.selectedAccount.set(acc);
    this.bankingService.getTransactions(acc.id).subscribe({
      next: (res) => this.transactions.set(res),
      error: (err) => this.errorMessage.set(err.error?.error || 'Failed to load transaction ledger.')
    });
  }

  public openCreateModal(): void {
    this.showCreateModal.set(true);
    this.errorMessage.set(null);
  }

  public closeCreateModal(): void {
    this.showCreateModal.set(false);
  }

  public submitCreateAccount(): void {
    if (this.createForm.invalid) return;
    const user = this.authService.currentUser();
    if (!user) return;

    this.isSubmitting.set(true);
    this.errorMessage.set(null);

    const cmd = {
      customerId: user.customerId,
      type: this.createForm.value.type!,
      initialDeposit: Number(this.createForm.value.initialDeposit!),
      currency: 'SAR'
    };

    this.bankingService.createAccount(cmd).subscribe({
      next: (acc) => {
        this.isSubmitting.set(false);
        this.closeCreateModal();
        this.successMessage.set(`Successfully created new ${acc.type} account with IBAN ${acc.iban}!`);
        this.loadAccounts();
      },
      error: (err) => {
        this.isSubmitting.set(false);
        this.errorMessage.set(err.error?.error || 'Failed to create bank account.');
      }
    });
  }

  public openDepositModal(acc: BankAccountDto): void {
    this.targetAccount.set(acc);
    this.actionType.set('deposit');
    this.actionForm.patchValue({ amount: 500, description: 'Cash Deposit' });
    this.showActionModal.set(true);
    this.errorMessage.set(null);
  }

  public openWithdrawModal(acc: BankAccountDto): void {
    this.targetAccount.set(acc);
    this.actionType.set('withdraw');
    this.actionForm.patchValue({ amount: 200, description: 'POS Purchase' });
    this.showActionModal.set(true);
    this.errorMessage.set(null);
  }

  public closeActionModal(): void {
    this.showActionModal.set(false);
  }

  public submitAction(): void {
    if (this.actionForm.invalid) return;
    const acc = this.targetAccount();
    if (!acc) return;

    this.isSubmitting.set(true);
    this.errorMessage.set(null);

    const amt = Number(this.actionForm.value.amount!);
    const desc = this.actionForm.value.description!;

    if (this.actionType() === 'deposit') {
      this.bankingService.deposit(acc.id, { amount: amt, currency: 'SAR', description: desc }).subscribe({
        next: () => this.handleActionSuccess(`Successfully deposited ${amt} SAR to ${acc.iban}`),
        error: (err) => this.handleActionError(err)
      });
    } else {
      this.bankingService.withdraw(acc.id, { amount: amt, currency: 'SAR', description: desc }).subscribe({
        next: () => this.handleActionSuccess(`Successfully withdrew ${amt} SAR from ${acc.iban}`),
        error: (err) => this.handleActionError(err)
      });
    }
  }

  private handleActionSuccess(msg: string): void {
    this.isSubmitting.set(false);
    this.closeActionModal();
    this.successMessage.set(msg);
    this.loadAccounts();
    if (this.selectedAccount()) {
      this.selectAccount(this.selectedAccount()!);
    }
  }

  private handleActionError(err: any): void {
    this.isSubmitting.set(false);
    this.errorMessage.set(err.error?.error || 'Transaction failed.');
  }
}
