import { Routes } from '@angular/router';
import { LoginComponent } from './features/auth/login/login.component';
import { RegisterComponent } from './features/auth/register/register.component';
import { ProfileComponent } from './features/customer/profile/profile.component';
import { BankingDashboardComponent } from './features/banking/dashboard/banking-dashboard.component';
import { authGuard } from './core/auth/auth.guard';

export const routes: Routes = [
  { path: '', redirectTo: 'auth/login', pathMatch: 'full' },
  { path: 'auth/login', component: LoginComponent },
  { path: 'auth/register', component: RegisterComponent },
  { path: 'customer/profile', component: ProfileComponent, canActivate: [authGuard] },
  { path: 'dashboard/accounts', component: BankingDashboardComponent, canActivate: [authGuard] },
  { path: '**', redirectTo: 'auth/login' }
];
