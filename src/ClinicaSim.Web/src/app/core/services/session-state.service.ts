import { Injectable } from '@angular/core';
import { StartSessionResponse } from '../../models/api.models';

@Injectable({ providedIn: 'root' })
export class SessionStateService {
  private currentSession: StartSessionResponse | null = null;

  setCurrentSession(data: StartSessionResponse): void {
    this.currentSession = data;
  }

  getCurrentSession(): StartSessionResponse | null {
    return this.currentSession;
  }

  clear(): void {
    this.currentSession = null;
  }
}
