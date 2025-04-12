import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { catchError, Observable, tap, throwError } from 'rxjs';
import { identityTokenResponse } from '../contracts/responses/token.response';
import { registerUser } from '../register/models/register-user';
import { loginUser } from '../login/models/login-user';
import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class IdentityService {
  private apiUrl = environment.apiUrl + '/users';
  getToken() {
    return localStorage.getItem(environment.jwtToken);
  }
  constructor(private http: HttpClient) {}

  registerUser(user: registerUser): Observable<identityTokenResponse> {
    console.log(user);
    return this.http.post<identityTokenResponse>(this.apiUrl, user).pipe(
      tap((response: identityTokenResponse) => {
        localStorage.setItem(environment.jwtToken, response.token);
      }),
      catchError(this.handleError)
    );
  }

  login(user: loginUser): Observable<identityTokenResponse> {
    return this.http
      .post<identityTokenResponse>(this.apiUrl + `/login`, user)
      .pipe(
        tap((response: identityTokenResponse) => {
          localStorage.setItem(environment.jwtToken, response.token);
        }),
        catchError(this.handleError)
      );
  }

  isEmailTaken(email: string): Observable<boolean> {
    return this.http.post<boolean>(this.apiUrl + `/check-email`, {
      Email: email,
    });
  }

  isUserNameTaken(userName: string): Observable<boolean> {
    return this.http.get<boolean>(this.apiUrl);
  }

  private handleError(error: HttpErrorResponse) {
    if (error.status === 0) {
      // A client-side or network error occurred. Handle it accordingly.
      console.error('An error occurred:', error.error);
    } else {
      // The backend returned an unsuccessful response code.
      // The response body may contain clues as to what went wrong.
      console.error(
        `Backend returned code ${error.status}, body was: `,
        error.error
      );
    }
    // Return an observable with a user-facing error message.
    return throwError(
      () => new Error('Something bad happened; please try again later.')
    );
  }
}
