import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, catchError, throwError } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { EquipmentBlueprintDetail } from '../models/equipment-blueprint-detail';
import { EquipmentBlueprintEquipmentCatalogItem } from '../models/equipment-blueprint-equipment-catalog-item';
import { EquipmentBlueprintCreatedResponse } from '../models/equipment-blueprint-created-response';
import { EquipmentBlueprintSaveRequest } from '../models/equipment-blueprint-save-request';
import { EquipmentBlueprintStatCatalogItem } from '../models/equipment-blueprint-stat-catalog-item';
import { EquipmentBlueprintSummary } from '../models/equipment-blueprint-summary';

@Injectable({
  providedIn: 'root',
})
export class AdminBlueprintsService {
  private readonly apiUrl = `${environment.apiUrl}/equipmentBlueprints`;

  constructor(private readonly http: HttpClient) {}

  getBlueprints(): Observable<EquipmentBlueprintSummary[]> {
    return this.http
      .get<EquipmentBlueprintSummary[]>(this.apiUrl)
      .pipe(catchError(this.handleError));
  }

  getBlueprint(blueprintId: string): Observable<EquipmentBlueprintDetail> {
    return this.http
      .get<EquipmentBlueprintDetail>(`${this.apiUrl}/${encodeURIComponent(blueprintId)}`)
      .pipe(catchError(this.handleError));
  }

  getEquipmentCatalog(): Observable<EquipmentBlueprintEquipmentCatalogItem[]> {
    return this.http
      .get<EquipmentBlueprintEquipmentCatalogItem[]>(`${this.apiUrl}/catalog/equipment`)
      .pipe(catchError(this.handleError));
  }

  getStatCatalog(): Observable<EquipmentBlueprintStatCatalogItem[]> {
    return this.http
      .get<EquipmentBlueprintStatCatalogItem[]>(`${this.apiUrl}/catalog/stats`)
      .pipe(catchError(this.handleError));
  }

  createBlueprint(request: EquipmentBlueprintSaveRequest): Observable<EquipmentBlueprintCreatedResponse> {
    return this.http
      .post<EquipmentBlueprintCreatedResponse>(this.apiUrl, request)
      .pipe(catchError(this.handleError));
  }

  updateBlueprint(blueprintId: string, request: EquipmentBlueprintSaveRequest): Observable<void> {
    return this.http
      .put<void>(`${this.apiUrl}/${encodeURIComponent(blueprintId)}`, request)
      .pipe(catchError(this.handleError));
  }

  deleteBlueprint(blueprintId: string): Observable<void> {
    return this.http
      .delete<void>(`${this.apiUrl}/${encodeURIComponent(blueprintId)}`)
      .pipe(catchError(this.handleError));
  }

  private handleError(error: HttpErrorResponse): Observable<never> {
    const message =
      typeof error.error === 'string' && error.error.trim().length > 0
        ? error.error
        : 'Failed to complete blueprint request.';

    return throwError(() => new Error(message));
  }
}
