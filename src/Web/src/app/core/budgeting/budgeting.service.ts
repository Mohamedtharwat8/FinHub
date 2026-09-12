import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface BudgetAlertDto {
    type: string;
    title: string;
    message: string;
    threshold: number;
    isActive: boolean;
}

export interface BudgetOverviewDto {
    customerId: string;
    month: string;
    monthlyBudget: number;
    monthlySpent: number;
    remainingBudget: number;
    savingsGoal: number;
    currency: string;
    alerts: BudgetAlertDto[];
}

@Injectable({
    providedIn: 'root'
})
export class BudgetingService {
    private http = inject(HttpClient);

    private get API_URL(): string {
        if (typeof window !== 'undefined' && window.location.hostname !== 'localhost' && window.location.hostname !== '127.0.0.1') {
            return 'https://finhub-w9xq.onrender.com/api/v1/budgets';
        }

        return 'http://localhost:5000/api/v1/budgets';
    }

    public getCustomerBudgetOverview(customerId: string): Observable<BudgetOverviewDto> {
        return this.http.get<BudgetOverviewDto>(`${this.API_URL}/customer/${customerId}`);
    }
}
