import { Component, OnInit, inject, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { AuthService } from '../../../core/auth/auth.service';
import { BankingService, BankAccountDto, TransactionDto } from '../../../core/banking/banking.service';

@Component({
  selector: 'app-banking-dashboard',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  template: `
    <div class="dashboard-container">
      <div class="dashboard-header">
        <div>
          <h1>Accounts & Banking Ledger</h1>
          <p class="subtitle">SAMA Open Banking Balance & Real-Time Ledger Tracking</p>
        </div>
        <button class="btn-primary" (click)="openCreateModal()">+ Open New Account</button>
      </div>

      <!-- Balance Summary Cards -->
      <div class="summary-grid">
        <div class="summary-card gold">
          <div class="card-title">Total Portfolio Balance</div>
          <div class="card-amount">{{ totalBalance() | number:'1.2-2' }} <span class="curr">SAR</span></div>
          <div class="card-sub">{{ accounts().length }} Active Accounts</div>
        </div>

        <div class="summary-card blue">
          <div class="card-title">Savings Accounts</div>
          <div class="card-amount">{{ savingsBalance() | number:'1.2-2' }} <span class="curr">SAR</span></div>
        </div>

        <div class="summary-card green">
          <div class="card-title">Current Accounts</div>
          <div class="card-amount">{{ currentBalance() | number:'1.2-2' }} <span class="curr">SAR</span></div>
        </div>
      </div>

      @if (errorMessage()) {
        <div class="alert error">{{ errorMessage() }}</div>
      }
      @if (successMessage()) {
        <div class="alert success">{{ successMessage() }}</div>
      }

      <!-- Accounts List Grid -->
      <h2 class="section-title">Your Bank Accounts</h2>
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
          <div class="empty-state">No bank accounts found. Click "+ Open New Account" above to create your first Saudi IBAN account!</div>
        }
      </div>

      <!-- Transaction History Ledger -->
      @if (selectedAccount()) {
        <div class="transactions-section">
          <div class="tx-header">
            <h2>Transaction Ledger: {{ selectedAccount()?.iban }}</h2>
            <span class="tx-count">{{ transactions().length }} Transactions</span>
          </div>

          <div class="table-responsive">
            <table class="tx-table">
              <thead>
                <tr>
                  <th>Date & Time</th>
                  <th>Type</th>
                  <th>Reference #</th>
                  <th>Description</th>
                  <th>Amount</th>
                </tr>
              </thead>
              <tbody>
                @for (tx of transactions(); track tx.id) {
                  <tr>
                    <td>{{ tx.transactionDate | date:'medium' }}</td>
                    <td><span class="tx-type-badge" [class]="tx.type.toLowerCase()">{{ tx.type }}</span></td>
                    <td><code>{{ tx.referenceNumber }}</code></td>
                    <td>{{ tx.description }}</td>
                    <td class="tx-amount" [class.positive]="tx.type === 'Deposit' || tx.type === 'TransferIn'" [class.negative]="tx.type === 'Withdrawal' || tx.type === 'TransferOut'">
                      {{ tx.type === 'Withdrawal' || tx.type === 'TransferOut' ? '-' : '+' }}{{ tx.amount | number:'1.2-2' }} {{ tx.currency }}
                    </td>
                  </tr>
                } @empty {
                  <tr><td colspan="5" class="empty-cell">No transactions found for this account.</td></tr>
                }
              </tbody>
            </table>
          </div>
        </div>
      }

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
    .dashboard-container { padding: 2rem; color: #fff; max-width: 1200px; margin: 0 auto; }
    .dashboard-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 2rem; }
    h1 { font-size: 1.8rem; margin: 0; }
    .subtitle { color: #8a8a9e; font-size: 0.9rem; margin-top: 0.2rem; }
    .btn-primary { background: #3699ff; color: #fff; border: none; padding: 0.75rem 1.25rem; border-radius: 8px; font-weight: 600; cursor: pointer; }
    .summary-grid { display: grid; grid-template-columns: repeat(auto-fit, minmax(280px, 1fr)); gap: 1.25rem; margin-bottom: 2rem; }
    .summary-card { background: #1e1e2d; padding: 1.5rem; border-radius: 12px; border-left: 4px solid #3699ff; box-shadow: 0 4px 15px rgba(0,0,0,0.2); }
    .summary-card.gold { border-left-color: #ffa800; }
    .summary-card.blue { border-left-color: #3699ff; }
    .summary-card.green { border-left-color: #0bb783; }
    .card-title { color: #8a8a9e; font-size: 0.85rem; text-transform: uppercase; font-weight: 600; }
    .card-amount { font-size: 1.8rem; font-weight: 700; margin: 0.4rem 0; color: #fff; }
    .curr { font-size: 1rem; color: #3699ff; }
    .card-sub { font-size: 0.8rem; color: #6c7293; }
    .section-title { font-size: 1.25rem; margin-bottom: 1rem; color: #fff; }
    .accounts-grid { display: grid; grid-template-columns: repeat(auto-fit, minmax(320px, 1fr)); gap: 1.25rem; margin-bottom: 2.5rem; }
    .account-card { background: #151521; border: 1px solid #2b2b40; padding: 1.25rem; border-radius: 12px; cursor: pointer; transition: all 0.2s ease; }
    .account-card:hover, .account-card.selected { border-color: #3699ff; background: #1e1e2d; }
    .acc-header { display: flex; justify-content: space-between; margin-bottom: 0.75rem; }
    .acc-type-badge { background: #2b2b40; color: #3699ff; padding: 0.25rem 0.6rem; border-radius: 6px; font-size: 0.75rem; font-weight: 600; }
    .acc-status-badge { background: rgba(11, 183, 131, 0.15); color: #0bb783; padding: 0.25rem 0.6rem; border-radius: 6px; font-size: 0.75rem; font-weight: 600; }
    .acc-balance { font-size: 1.5rem; font-weight: 700; margin-bottom: 0.4rem; }
    .acc-iban { font-family: monospace; font-size: 0.85rem; color: #8a8a9e; margin-bottom: 1rem; word-break: break-all; }
    .acc-actions { display: flex; gap: 0.5rem; }
    .btn-action { flex: 1; padding: 0.5rem; border-radius: 6px; border: none; font-weight: 600; font-size: 0.8rem; cursor: pointer; }
    .btn-action.deposit { background: #0bb783; color: #fff; }
    .btn-action.withdraw { background: #f1416c; color: #fff; }
    .transactions-section { background: #1e1e2d; padding: 1.5rem; border-radius: 12px; box-shadow: 0 4px 15px rgba(0,0,0,0.2); }
    .tx-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 1rem; }
    .tx-table { width: 100%; border-collapse: collapse; text-align: left; }
    .tx-table th, .tx-table td { padding: 0.85rem; border-bottom: 1px solid #2b2b40; font-size: 0.85rem; }
    .tx-table th { color: #8a8a9e; font-weight: 600; }
    .tx-type-badge { padding: 0.2rem 0.5rem; border-radius: 4px; font-size: 0.75rem; font-weight: 600; }
    .tx-type-badge.deposit { background: rgba(11, 183, 131, 0.15); color: #0bb783; }
    .tx-type-badge.withdrawal { background: rgba(241, 65, 108, 0.15); color: #f1416c; }
    .tx-amount.positive { color: #0bb783; font-weight: 600; }
    .tx-amount.negative { color: #f1416c; font-weight: 600; }
    .alert { padding: 0.85rem; border-radius: 8px; margin-bottom: 1.25rem; font-size: 0.85rem; }
    .alert.error { background: #f1416c; color: #fff; }
    .alert.success { background: #0bb783; color: #fff; }
    .empty-state { grid-column: 1 / -1; padding: 2rem; background: #151521; text-align: center; border-radius: 12px; color: #8a8a9e; }
    .modal-backdrop { position: fixed; top: 0; left: 0; right: 0; bottom: 0; background: rgba(0,0,0,0.7); display: flex; justify-content: center; align-items: center; z-index: 1000; }
    .modal-card { background: #1e1e2d; padding: 2rem; border-radius: 12px; width: 100%; max-width: 440px; box-shadow: 0 10px 30px rgba(0,0,0,0.5); }
    .form-group { margin-bottom: 1.1rem; display: flex; flex-direction: column; }
    label { font-size: 0.85rem; margin-bottom: 0.4rem; color: #b5b5c3; }
    input, select { background: #151521; border: 1px solid #2b2b40; padding: 0.75rem; border-radius: 8px; color: #fff; outline: none; }
    .modal-actions { display: flex; justify-content: flex-end; gap: 0.75rem; margin-top: 1.25rem; }
    .btn-cancel { background: #2b2b40; color: #fff; border: none; padding: 0.75rem 1.25rem; border-radius: 8px; cursor: pointer; }
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
  public savingsBalance = computed(() => this.accounts().filter(a => a.type === 'Savings').reduce((sum, a) => sum + a.balanceAmount, 0));
  public currentBalance = computed(() => this.accounts().filter(a => a.type === 'Current').reduce((sum, a) => sum + a.balanceAmount, 0));

  public createForm = this.fb.group({
    type: ['Savings', [Validators.required]],
    initialDeposit: [1000, [Validators.required, Validators.min(0)]]
  });

  public actionForm = this.fb.group({
    amount: [500, [Validators.required, Validators.min(1)]],
    description: ['ATM Deposit', [Validators.required]]
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
