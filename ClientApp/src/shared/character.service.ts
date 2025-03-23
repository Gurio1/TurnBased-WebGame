import { Injectable } from '@angular/core';
import { Character } from '../core/models/player';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { API_URL } from '../constants';
import { catchError, Observable, Subject, tap, throwError } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class CharacterService {
  private characterSubject = new Subject<Character>();

  constructor(private http: HttpClient) {}

  getPlayer(): Observable<Character> {
    return this.http
      .get<Character>(API_URL + `players`)
      .pipe(catchError(this.handleError))
      .pipe(tap((char) => this.characterSubject.next(char)));
  }

  makeAction(actionName: string, equipmentId: string): Observable<Character> {
    const url = `${API_URL}players/${actionName.toLowerCase()}/${equipmentId}`;
    return this.http
      .post<Character>(url, null)
      .pipe(catchError(this.handleError))
      .pipe(tap((char) => this.characterSubject.next(char)));
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
