import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface BankAccountDto {
  id: string;
  customerId: string;
  iban: string;
  accountNumber: string;
  type: string;
  status: string;
  balanceAmount: number;
  currency: string;
  createdAt: string;
}

export interface TransactionDto {
  id: string;
  accountId: string;
  type: string;
  amount: number;
  currency: string;
  description: string;
  referenceNumber: string;
  transactionDate: string;
}

export interface CreateAccountCommand {
  customerId: string;
  type: string;
  initialDeposit: number;
  currency: string;
}

export interface DepositCommand {
  amount: number;
  currency: string;
  description: string;
}

export interface WithdrawCommand {
  amount: number;
  currency: string;
  description: string;
}

@Injectable({
  providedIn: 'root'
})
export class BankingService {
  private http = inject(HttpClient);

  private get API_URL(): string {
    if (typeof window !== 'undefined' && window.location.hostname !== 'localhost' && window.location.hostname !== '127.0.0.1') {
      return 'https://finhub-w9xq.onrender.com/api/v1/accounts';
    }
    return 'http://localhost:5000/api/v1/accounts';
  }

  public getAccountsByCustomer(customerId: string): Observable<BankAccountDto[]> {
    return this.http.get<BankAccountDto[]>(`${this.API_URL}/customer/${customerId}`);
  }

  public getAccountById(accountId: string): Observable<BankAccountDto> {
    return this.http.get<BankAccountDto>(`${this.API_URL}/${accountId}`);
  }

  public createAccount(command: CreateAccountCommand): Observable<BankAccountDto> {
    return this.http.post<BankAccountDto>(this.API_URL, command);
  }

  public deposit(accountId: string, command: DepositCommand): Observable<BankAccountDto> {
    return this.http.post<BankAccountDto>(`${this.API_URL}/${accountId}/deposit`, command);
  }

  public withdraw(accountId: string, command: WithdrawCommand): Observable<BankAccountDto> {
    return this.http.post<BankAccountDto>(`${this.API_URL}/${accountId}/withdraw`, command);
  }

  public getTransactions(accountId: string): Observable<TransactionDto[]> {
    return this.http.get<TransactionDto[]>(`${this.API_URL}/${accountId}/transactions`);
  }
}
