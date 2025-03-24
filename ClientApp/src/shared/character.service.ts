import { Injectable } from '@angular/core';
import { Character } from '../core/models/player';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { API_URL } from '../constants';
import { BehaviorSubject, catchError, Observable, tap, throwError } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class CharacterService {
  private characterSubject = new BehaviorSubject<Character | null>(null);

  character$ = this.characterSubject.asObservable();

  constructor(private http: HttpClient) {}

  getPlayer(): Observable<Character> {
    return this.http.get<Character>(`${API_URL}players`).pipe(
      tap((character) => this.characterSubject.next(character)),
      catchError(this.handleError)
    );
  }

  makeAction(actionName: string, equipmentId: string): Observable<Character> {
    return this.http
      .post<Character>(
        `${API_URL}players/${actionName.toLowerCase()}/${equipmentId}`,
        null
      )
      .pipe(
        tap((character) => this.characterSubject.next(character)),
        catchError(this.handleError)
      );
  }

  private handleError(error: HttpErrorResponse) {
    console.error('API Error:', error);
    return throwError(
      () => new Error('An error occurred. Please try again later.')
    );
  }
}
