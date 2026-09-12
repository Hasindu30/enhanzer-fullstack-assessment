import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Location, PurchaseBillCalculateRequest, PurchaseBillCalculateResponse } from '../models/purchase-bill.models';

@Injectable({ providedIn: 'root' })
export class PurchaseBillService {
  private readonly baseUrl = environment.apiBaseUrl;

  constructor(private http: HttpClient) {}

  getLocations(): Observable<Location[]> {
    return this.http.get<Location[]>(
      `${this.baseUrl}/api/locations`, 
      { withCredentials: true }
    );
  }

  calculatePurchaseBill(request: PurchaseBillCalculateRequest): Observable<PurchaseBillCalculateResponse> {
    return this.http.post<PurchaseBillCalculateResponse>(
      `${this.baseUrl}/api/purchase-bill/calculate`, 
      request, 
      { withCredentials: true }
    );
  }
}
