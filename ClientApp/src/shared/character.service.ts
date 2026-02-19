import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { BehaviorSubject, catchError, Observable, tap, throwError } from 'rxjs';
import { environment } from '../environments/environment';
import { PlayerHomeViewModel } from '../features/home/contracts/playerHomeViewModel';

@Injectable({
  providedIn: 'root',
})
export class CharacterService {
  private apiUrl = environment.apiUrl + '/players';

  private characterSubject = new BehaviorSubject<PlayerHomeViewModel | null>(
    null
  );

  character$ = this.characterSubject.asObservable();

  constructor(private http: HttpClient) {}

  getPlayer(): Observable<PlayerHomeViewModel> {
    return this.http.get<PlayerHomeViewModel>(this.apiUrl).pipe(
      tap((character) => {
        this.characterSubject.next(character);
        console.log('Character fetched:', character);
      }),
      catchError(this.handleError)
    );
  }

  makeAction(actionName: string): Observable<PlayerHomeViewModel> {
    return this.http.post<PlayerHomeViewModel>(actionName, null).pipe(
      tap((character) => this.characterSubject.next(character)),
      catchError(this.handleError)
    );
  }

  unequipItem(slot: string): Observable<PlayerHomeViewModel> {
    console.log(`Unequipping item from slot: ${slot}`);
    return this.http
      .post<PlayerHomeViewModel>(this.apiUrl + '/unequip/' + slot, null)
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
