import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  CreateFindingPayload,
  FindingAdminItem,
  FindingFilters,
  UpdateFindingPayload
} from '../../models/admin.models';

@Injectable({ providedIn: 'root' })
export class AdminFindingService {
  private readonly baseUrl = `${environment.apiBaseUrl}/api/admin/findings`;

  constructor(private readonly http: HttpClient) {}

  getFindings(filters: FindingFilters): Observable<FindingAdminItem[]> {
    let params = new HttpParams();

    if (filters.active !== undefined) params = params.set('active', String(filters.active));
    if (filters.system) params = params.set('system', filters.system);
    if (filters.search) params = params.set('search', filters.search);

    return this.http.get<FindingAdminItem[]>(this.baseUrl, { params });
  }

  getFindingById(id: string): Observable<FindingAdminItem> {
    return this.http.get<FindingAdminItem>(`${this.baseUrl}/${id}`);
  }

  createFinding(payload: CreateFindingPayload): Observable<FindingAdminItem> {
    return this.http.post<FindingAdminItem>(this.baseUrl, payload);
  }

  updateFinding(id: string, payload: UpdateFindingPayload): Observable<FindingAdminItem> {
    return this.http.put<FindingAdminItem>(`${this.baseUrl}/${id}`, payload);
  }

  activateFinding(id: string): Observable<FindingAdminItem> {
    return this.http.patch<FindingAdminItem>(`${this.baseUrl}/${id}/activate`, {});
  }

  deactivateFinding(id: string): Observable<FindingAdminItem> {
    return this.http.patch<FindingAdminItem>(`${this.baseUrl}/${id}/deactivate`, {});
  }
}
