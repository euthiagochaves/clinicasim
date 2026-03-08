import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  CreateQuestionPayload,
  QuestionAdminItem,
  QuestionFilters,
  UpdateQuestionPayload
} from '../../models/admin.models';

@Injectable({ providedIn: 'root' })
export class AdminQuestionService {
  private readonly baseUrl = `${environment.apiBaseUrl}/api/admin/questions`;

  constructor(private readonly http: HttpClient) {}

  getQuestions(filters: QuestionFilters): Observable<QuestionAdminItem[]> {
    let params = new HttpParams();

    if (filters.active !== undefined) params = params.set('active', String(filters.active));
    if (filters.section) params = params.set('section', filters.section);
    if (filters.category) params = params.set('category', filters.category);
    if (filters.search) params = params.set('search', filters.search);

    return this.http.get<QuestionAdminItem[]>(this.baseUrl, { params });
  }

  getQuestionById(id: string): Observable<QuestionAdminItem> {
    return this.http.get<QuestionAdminItem>(`${this.baseUrl}/${id}`);
  }

  createQuestion(payload: CreateQuestionPayload): Observable<QuestionAdminItem> {
    return this.http.post<QuestionAdminItem>(this.baseUrl, payload);
  }

  updateQuestion(id: string, payload: UpdateQuestionPayload): Observable<QuestionAdminItem> {
    return this.http.put<QuestionAdminItem>(`${this.baseUrl}/${id}`, payload);
  }

  activateQuestion(id: string): Observable<QuestionAdminItem> {
    return this.http.patch<QuestionAdminItem>(`${this.baseUrl}/${id}/activate`, {});
  }

  deactivateQuestion(id: string): Observable<QuestionAdminItem> {
    return this.http.patch<QuestionAdminItem>(`${this.baseUrl}/${id}/deactivate`, {});
  }
}
