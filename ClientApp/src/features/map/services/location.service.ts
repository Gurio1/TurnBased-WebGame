import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, catchError, throwError } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ExploreLocationResponse } from '../contracts/explore-location-response';

@Injectable({
  providedIn: 'root',
})
export class LocationService {
  private readonly apiUrl = `${environment.apiUrl}/locations`;

  constructor(private readonly http: HttpClient) {}

  explore(locationName: string): Observable<ExploreLocationResponse> {
    return this.http
      .post<ExploreLocationResponse>(
        `${this.apiUrl}/explore/${encodeURIComponent(locationName)}`,
        null
      )
      .pipe(catchError(this.handleError));
  }

  private handleError(error: HttpErrorResponse): Observable<never> {
    const errorMessage =
      typeof error.error === 'string' && error.error.trim().length > 0
        ? error.error
        : 'Failed to explore location.';

    return throwError(() => new Error(errorMessage));
  }
}
