import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, catchError, throwError } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { MonsterAbilityCatalogItem } from '../models/monster-ability-catalog-item';
import { MonsterDetail } from '../models/monster-detail';
import { MonsterItemCatalogItem } from '../models/monster-item-catalog-item';
import { MonsterSaveRequest } from '../models/monster-save-request';
import { MonsterStatCatalogItem } from '../models/monster-stat-catalog-item';
import { MonsterSummary } from '../models/monster-summary';

@Injectable({
  providedIn: 'root',
})
export class AdminMonstersService {
  private readonly apiUrl = `${environment.apiUrl}/monsters`;

  constructor(private readonly http: HttpClient) {}

  getMonsters(): Observable<MonsterSummary[]> {
    return this.http.get<MonsterSummary[]>(this.apiUrl).pipe(catchError(this.handleError));
  }

  getMonster(monsterName: string): Observable<MonsterDetail> {
    return this.http
      .get<MonsterDetail>(`${this.apiUrl}/${encodeURIComponent(monsterName)}`)
      .pipe(catchError(this.handleError));
  }

  getItemCatalog(): Observable<MonsterItemCatalogItem[]> {
    return this.http
      .get<MonsterItemCatalogItem[]>(`${this.apiUrl}/catalog/items`)
      .pipe(catchError(this.handleError));
  }

  getAbilityCatalog(): Observable<MonsterAbilityCatalogItem[]> {
    return this.http
      .get<MonsterAbilityCatalogItem[]>(`${this.apiUrl}/catalog/abilities`)
      .pipe(catchError(this.handleError));
  }

  getStatCatalog(): Observable<MonsterStatCatalogItem[]> {
    return this.http
      .get<MonsterStatCatalogItem[]>(`${this.apiUrl}/catalog/stats`)
      .pipe(catchError(this.handleError));
  }

  createMonster(request: MonsterSaveRequest): Observable<MonsterDetail> {
    return this.http.post<MonsterDetail>(this.apiUrl, request).pipe(catchError(this.handleError));
  }

  updateMonster(currentMonsterName: string, request: MonsterSaveRequest): Observable<void> {
    return this.http
      .put<void>(`${this.apiUrl}/${encodeURIComponent(currentMonsterName)}`, request)
      .pipe(catchError(this.handleError));
  }

  deleteMonster(monsterName: string): Observable<void> {
    return this.http
      .delete<void>(`${this.apiUrl}/${encodeURIComponent(monsterName)}`)
      .pipe(catchError(this.handleError));
  }

  private handleError(error: HttpErrorResponse): Observable<never> {
    const message =
      typeof error.error === 'string' && error.error.trim().length > 0
        ? error.error
        : 'Failed to complete monster admin request.';

    return throwError(() => new Error(message));
  }
}
