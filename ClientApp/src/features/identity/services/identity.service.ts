import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { catchError, Observable, tap, throwError } from 'rxjs';
import { environment } from '../../../environments/environment';
import { IdentityTokenResponse } from '../contracts/responses/token-response';
import { LoginUser } from '../login/models/login-user';
import { RegisterUser } from '../register/models/register-user';

@Injectable({
  providedIn: 'root',
})
export class IdentityService {
  private readonly apiUrl = `${environment.apiUrl}/users`;

  constructor(private readonly http: HttpClient) {}

  getToken(): string | null {
    return localStorage.getItem(environment.jwtToken);
  }

  registerUser(user: RegisterUser): Observable<IdentityTokenResponse> {
    return this.http.post<IdentityTokenResponse>(this.apiUrl, user).pipe(
      tap((response) => {
        localStorage.setItem(environment.jwtToken, response.token);
      }),
      catchError(this.handleError)
    );
  }

  login(user: LoginUser): Observable<IdentityTokenResponse> {
    return this.http.post<IdentityTokenResponse>(`${this.apiUrl}/login`, user).pipe(
      tap((response) => {
        localStorage.setItem(environment.jwtToken, response.token);
      }),
      catchError(this.handleError)
    );
  }

  isEmailTaken(email: string): Observable<boolean> {
    return this.http.post<boolean>(`${this.apiUrl}/check-email`, {
      Email: email,
    });
  }

  isUserNameTaken(_userName: string): Observable<boolean> {
    return this.http.get<boolean>(this.apiUrl);
  }

  private handleError(error: HttpErrorResponse): Observable<never> {
    if (error.status === 0) {
      console.error('An error occurred:', error.error);
    } else {
      console.error(`Backend returned code ${error.status}, body was: `, error.error);
    }

    return throwError(() => new Error('Something bad happened; please try again later.'));
  }
}
