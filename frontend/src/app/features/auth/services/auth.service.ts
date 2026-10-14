import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface LoginRequest {
    email: string;
    password: string;
}

export interface RegisterRequest {
    firstName: string;
    lastName: string;
    email: string;
    password: string;
}

export interface ForgotRequest {
    email: string;
}

export interface ResetRequest {
    email: string;
    password: string;
}

export interface AuthResponse {
    userId: string;
    email: string;
}

@Injectable({
    providedIn: 'root',
})
export class AuthService {
    private http = inject(HttpClient);
    private apiUrl = 'http://localhost:5088/api/auth';

    private post<T>(endpoint: string, body: unknown): Observable<T> {
        return this.http.post<T>(`${this.apiUrl}/${endpoint}`, body, { withCredentials: true });
    }

    login(body: LoginRequest): Observable<AuthResponse> {
        return this.post<AuthResponse>('login', body);
    }

    logout(): Observable<void> {
        return this.post<void>('logout', null);
    }

    register(body: RegisterRequest): Observable<AuthResponse> {
        return this.post<AuthResponse>('register', body);
    }

    forgot(body: ForgotRequest): Observable<AuthResponse> {
        return this.post<AuthResponse>('forgot-password', body);
    }

    reset(body: ResetRequest): Observable<AuthResponse> {
        return this.post<AuthResponse>('reset-password', body);
    }
}
