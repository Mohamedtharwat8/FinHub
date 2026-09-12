import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface AddressDto {
  buildingNumber: string;
  street: string;
  district: string;
  city: string;
  postalCode: string;
  additionalNumber?: string;
  country: string;
}

export interface CustomerProfileDto {
  id: string;
  email: string;
  fullName: string;
  role: string;
  nationalId?: string;
  isCitizen: boolean;
  address?: AddressDto;
  isEmailVerified: boolean;
  isMfaEnabled: boolean;
  createdAt: string;
}

@Injectable({
  providedIn: 'root'
})
export class CustomerService {
  private http = inject(HttpClient);
  private readonly API_URL = 'http://localhost:5000/api/v1/customer';

  public getProfile(customerId: string): Observable<CustomerProfileDto> {
    return this.http.get<CustomerProfileDto>(`${this.API_URL}/${customerId}/profile`);
  }

  public updateAddress(customerId: string, address: AddressDto): Observable<CustomerProfileDto> {
    return this.http.put<CustomerProfileDto>(`${this.API_URL}/${customerId}/address`, address);
  }

  public setNationalId(customerId: string, nationalId: string): Observable<CustomerProfileDto> {
    return this.http.put<CustomerProfileDto>(`${this.API_URL}/${customerId}/national-id`, JSON.stringify(nationalId), {
      headers: { 'Content-Type': 'application/json' }
    });
  }
}
