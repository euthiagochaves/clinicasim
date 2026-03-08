import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  CaseListItemDto,
  ClinicalNoteRequest,
  ClinicalNoteResponse,
  DifferentialItemDto,
  FinalizeResponse,
  PostEventRequest,
  PostEventResponse,
  SaveDifferentialsRequest,
  SessionInfoResponse,
  StartSessionRequest,
  StartSessionResponse
} from '../../models/api.models';

@Injectable({ providedIn: 'root' })
export class ApiClientService {
  private readonly baseUrl = environment.apiBaseUrl;

  constructor(private readonly http: HttpClient) {}

  getCases(): Observable<CaseListItemDto[]> {
    return this.http.get<CaseListItemDto[]>(`${this.baseUrl}/api/cases`);
  }

  startSession(req: StartSessionRequest): Observable<StartSessionResponse> {
    return this.http.post<StartSessionResponse>(`${this.baseUrl}/api/sessions/start`, req);
  }

  getSession(sessionCode: string): Observable<SessionInfoResponse> {
    return this.http.get<SessionInfoResponse>(`${this.baseUrl}/api/sessions/${sessionCode}`);
  }

  postEvent(sessionCode: string, req: PostEventRequest): Observable<PostEventResponse> {
    return this.http.post<PostEventResponse>(`${this.baseUrl}/api/sessions/${sessionCode}/events`, req);
  }

  getNote(sessionCode: string): Observable<ClinicalNoteResponse> {
    return this.http.get<ClinicalNoteResponse>(`${this.baseUrl}/api/sessions/${sessionCode}/note`);
  }

  saveNote(sessionCode: string, req: ClinicalNoteRequest): Observable<ClinicalNoteResponse> {
    return this.http.post<ClinicalNoteResponse>(`${this.baseUrl}/api/sessions/${sessionCode}/note`, req);
  }

  getDifferentials(sessionCode: string): Observable<DifferentialItemDto[]> {
    return this.http.get<DifferentialItemDto[]>(`${this.baseUrl}/api/sessions/${sessionCode}/differentials`);
  }

  saveDifferentials(sessionCode: string, req: SaveDifferentialsRequest): Observable<DifferentialItemDto[]> {
    return this.http.post<DifferentialItemDto[]>(`${this.baseUrl}/api/sessions/${sessionCode}/differentials`, req);
  }

  finalizeSession(sessionCode: string): Observable<FinalizeResponse> {
    return this.http.post<FinalizeResponse>(`${this.baseUrl}/api/sessions/${sessionCode}/finalize`, {});
  }

  downloadPdf(sessionCode: string): Observable<Blob> {
    return this.http.get(`${this.baseUrl}/api/sessions/${sessionCode}/pdf`, {
      responseType: 'blob'
    });
  }
}
