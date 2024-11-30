import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  httpClient: HttpClient = inject(HttpClient);

  private serverLink: string = "https://localhost:7284/";

  register(userData: { firstName: string; lastName: string; email: string; password: string; confirmPassword: string }) {
    return this.httpClient.post(`${this.serverLink}account/register`, userData);
  }

  login(credentials: { email: string; password: string, rememberMe: boolean, returnUrl: string }): Observable<any> {
    return this.httpClient.post<any>(`${this.serverLink}account/login`, credentials)
      .pipe(
        catchError(this.handleError)
      );
  }

  private handleError(error: HttpErrorResponse) {
    let errorMessage = 'An unknown error occurred';
    if (error.error && typeof error.error === 'object' && 'Message' in error.error) {
      errorMessage = error.error.Message;
    } else if (error.message) {
      errorMessage = error.message;
    }
    return throwError(() => errorMessage);
  }
  
  logout(): void {
    localStorage.removeItem('accessToken');
    localStorage.removeItem('userId');
    localStorage.removeItem('userRole');
  }

  isAuthenticated(): boolean {
    return !!localStorage.getItem('accessToken');
  }
}