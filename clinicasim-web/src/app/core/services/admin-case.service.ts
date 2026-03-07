import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  AdminCaseItem,
  CaseAnswerMapping,
  CaseFilters,
  CaseFindingMapping,
  CreateCaseAnswerPayload,
  CreateCaseFindingPayload,
  CreateCasePayload,
  UpdateCaseAnswerPayload,
  UpdateCaseFindingPayload,
  UpdateCasePayload
} from '../../models/admin.models';

@Injectable({ providedIn: 'root' })
export class AdminCaseService {
  private readonly baseUrl = `${environment.apiBaseUrl}/api/admin/cases`;

  constructor(private readonly http: HttpClient) {}

  getCases(filters: CaseFilters): Observable<AdminCaseItem[]> {
    let params = new HttpParams();
    if (filters.active !== undefined) params = params.set('active', String(filters.active));
    if (filters.search) params = params.set('search', filters.search);
    if (filters.triage) params = params.set('triage', filters.triage);
    return this.http.get<AdminCaseItem[]>(this.baseUrl, { params });
  }

  getCaseById(id: string): Observable<AdminCaseItem> {
    return this.http.get<AdminCaseItem>(`${this.baseUrl}/${id}`);
  }

  createCase(payload: CreateCasePayload): Observable<AdminCaseItem> {
    return this.http.post<AdminCaseItem>(this.baseUrl, payload);
  }

  updateCase(id: string, payload: UpdateCasePayload): Observable<AdminCaseItem> {
    return this.http.put<AdminCaseItem>(`${this.baseUrl}/${id}`, payload);
  }

  activateCase(id: string): Observable<AdminCaseItem> {
    return this.http.patch<AdminCaseItem>(`${this.baseUrl}/${id}/activate`, {});
  }

  deactivateCase(id: string): Observable<AdminCaseItem> {
    return this.http.patch<AdminCaseItem>(`${this.baseUrl}/${id}/deactivate`, {});
  }

  getCaseAnswers(caseId: string): Observable<CaseAnswerMapping[]> {
    return this.http.get<CaseAnswerMapping[]>(`${this.baseUrl}/${caseId}/answers`);
  }

  addCaseAnswer(caseId: string, payload: CreateCaseAnswerPayload): Observable<CaseAnswerMapping> {
    return this.http.post<CaseAnswerMapping>(`${this.baseUrl}/${caseId}/answers`, payload);
  }

  updateCaseAnswer(caseId: string, mappingId: string, payload: UpdateCaseAnswerPayload): Observable<CaseAnswerMapping> {
    return this.http.put<CaseAnswerMapping>(`${this.baseUrl}/${caseId}/answers/${mappingId}`, payload);
  }

  deleteCaseAnswer(caseId: string, mappingId: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${caseId}/answers/${mappingId}`);
  }

  getCaseFindings(caseId: string): Observable<CaseFindingMapping[]> {
    return this.http.get<CaseFindingMapping[]>(`${this.baseUrl}/${caseId}/findings`);
  }

  addCaseFinding(caseId: string, payload: CreateCaseFindingPayload): Observable<CaseFindingMapping> {
    return this.http.post<CaseFindingMapping>(`${this.baseUrl}/${caseId}/findings`, payload);
  }

  updateCaseFinding(caseId: string, mappingId: string, payload: UpdateCaseFindingPayload): Observable<CaseFindingMapping> {
    return this.http.put<CaseFindingMapping>(`${this.baseUrl}/${caseId}/findings/${mappingId}`, payload);
  }

  deleteCaseFinding(caseId: string, mappingId: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${caseId}/findings/${mappingId}`);
  }
}
