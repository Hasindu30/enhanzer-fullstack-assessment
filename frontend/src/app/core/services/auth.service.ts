import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { LoginRequest, LoginResponse, MeResponse } from '../models/auth.models';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly baseUrl = environment.apiBaseUrl;

  constructor(private http: HttpClient) {}

  login(credentials: LoginRequest): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(
      `${this.baseUrl}/api/auth/login`,
      credentials,
      { withCredentials: true }
    );
  }

  getMe(): Observable<MeResponse> {
    return this.http.get<MeResponse>(
      `${this.baseUrl}/api/auth/me`,
      { withCredentials: true }
    );
  }

  logout(): Observable<void> {
    return this.http.post<void>(
      `${this.baseUrl}/api/auth/logout`,
      {},
      { withCredentials: true }
    );
  }
}
