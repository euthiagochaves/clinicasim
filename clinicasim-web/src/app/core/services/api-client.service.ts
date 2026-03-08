import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  CaseListItemDto,
  ClinicalNoteRequest,
  ClinicalNoteResponse,
  DifferentialItemDto,
  FindingDto,
  FinalizeResponse,
  PostEventRequest,
  PostEventResponse,
  QuestionDto,
  ResolvedFindingDto,
  ResolvedQuestionAnswerDto,
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

  getQuestions(section?: string): Observable<QuestionDto[]> {
    const qs = section ? `?section=${encodeURIComponent(section)}` : '';
    return this.http.get<QuestionDto[]>(`${this.baseUrl}/api/questions${qs}`);
  }

  getFindings(system?: string): Observable<FindingDto[]> {
    const qs = system ? `?system=${encodeURIComponent(system)}` : '';
    return this.http.get<FindingDto[]>(`${this.baseUrl}/api/findings${qs}`);
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

  resolveQuestion(caseId: string, questionId: string): Observable<ResolvedQuestionAnswerDto> {
    return this.http.get<ResolvedQuestionAnswerDto>(`${this.baseUrl}/api/cases/${caseId}/resolve-question/${questionId}`);
  }

  resolveFinding(caseId: string, findingId: string): Observable<ResolvedFindingDto> {
    return this.http.get<ResolvedFindingDto>(`${this.baseUrl}/api/cases/${caseId}/resolve-finding/${findingId}`);
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
