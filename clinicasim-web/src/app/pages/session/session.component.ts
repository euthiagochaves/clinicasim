import { Component, OnDestroy, OnInit } from '@angular/core';
import { NgFor, NgIf } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { Subscription } from 'rxjs';
import { ApiClientService } from '../../core/services/api-client.service';
import { SessionStateService } from '../../core/services/session-state.service';
import {
  SessionCategoryDto,
  SessionQuestionDto,
  SessionSectionDto,
  StartSessionResponse
} from '../../models/api.models';

type SectionTab = 'anamnesis' | 'examen' | 'historia';

interface FeedItem {
  questionText: string;
  answerText: string;
}

@Component({
  selector: 'app-session',
  standalone: true,
  imports: [NgIf, NgFor, RouterLink],
  styles: [
    `
      .layout { display: flex; gap: 12px; }
      .left { flex: 0 0 25%; }
      .center { flex: 0 0 50%; }
      .right { flex: 0 0 25%; }
      .panel { border: 1px solid #ddd; border-radius: 6px; padding: 10px; }
      .tabs button { margin-right: 6px; margin-bottom: 8px; }
      .category { margin: 10px 0; }
      .question { display: block; margin: 4px 0; }
      .feed-item { margin-bottom: 10px; border-bottom: 1px dashed #ddd; padding-bottom: 8px; }
      .warn { color: #b00020; font-size: 14px; }
    `
  ],
  template: `
    <h2>Atención clínica</h2>
    <p><strong>Sesión:</strong> {{ sessionCode }}</p>
    <p *ngIf="statusText"><strong>Estado:</strong> {{ statusText }}</p>
    <p *ngIf="isFinalized" class="warn">Sesión finalizada. Solo lectura.</p>

    <p *ngIf="missingSessionData" class="warn">
      Datos de la sesión no cargados. Vuelva a la lista de pacientes e inicie la sesión nuevamente.
    </p>
    <a *ngIf="missingSessionData" routerLink="/cases">Volver a pacientes</a>

    <p *ngIf="loadError" class="warn">Sesión no encontrada.</p>

    <div class="layout" *ngIf="sessionData && !loadError">
      <div class="left panel">
        <div class="tabs">
          <button (click)="activeTab = 'anamnesis'">Anamnesis</button>
          <button (click)="activeTab = 'examen'">Examen Físico</button>
          <button (click)="activeTab = 'historia'">Historia Clínica</button>
        </div>

        <ng-container *ngIf="activeTab !== 'historia'; else historiaTpl">
          <ng-container *ngIf="activeSection as section; else noSectionTpl">
            <div class="category" *ngFor="let category of section.categories">
              <strong>{{ category.name }}</strong>
              <button
                class="question"
                *ngFor="let question of category.questions"
                (click)="onQuestionClick(question)"
                [disabled]="isFinalized || loadingQuestionId === question.questionId"
              >
                {{ loadingQuestionId === question.questionId ? 'Enviando...' : question.text }}
              </button>
            </div>
          </ng-container>
        </ng-container>

        <ng-template #historiaTpl>
          <p>Se implementa en Tarea 8.</p>
        </ng-template>

        <ng-template #noSectionTpl>
          <p>No hay datos para esta sección.</p>
        </ng-template>
      </div>

      <div class="center panel">
        <h3>Feed de interacción</h3>
        <p *ngIf="feedItems.length === 0">Sin interacciones recientes.</p>

        <div class="feed-item" *ngFor="let item of feedItems">
          <div><strong>Médico:</strong> {{ item.questionText }}</div>
          <div><strong>Paciente:</strong> {{ item.answerText }}</div>
        </div>
      </div>

      <div class="right panel">
        <h3>Datos del paciente</h3>
        <ng-container *ngIf="sessionData.case as c">
          <div><strong>Nombre:</strong> {{ c.fullName }}</div>
          <div><strong>Edad:</strong> {{ c.age }}</div>
          <div><strong>Sexo:</strong> {{ c.sex }}</div>
          <div><strong>Motivo:</strong> {{ c.chiefComplaint }}</div>
          <div><strong>Triaje:</strong> {{ c.triage }}</div>
        </ng-container>
      </div>
    </div>

    <p style="margin-top: 12px;">Pantalla de atención se implementa en Tarea 7/8.</p>
  `
})
export class SessionComponent implements OnInit, OnDestroy {
  sessionCode = '';
  statusText = '';
  isFinalized = false;
  loadError = false;

  sessionData: StartSessionResponse | null = null;
  missingSessionData = false;
  activeTab: SectionTab = 'anamnesis';
  feedItems: FeedItem[] = [];
  loadingQuestionId: string | null = null;

  private routeSub?: Subscription;
  private clearFeedTimeoutId: ReturnType<typeof setTimeout> | null = null;

  constructor(
    private readonly route: ActivatedRoute,
    private readonly apiClient: ApiClientService,
    private readonly sessionStateService: SessionStateService
  ) {}

  get activeSection(): SessionSectionDto | null {
    if (!this.sessionData) {
      return null;
    }

    const target = this.activeTab === 'anamnesis' ? 'anamnesis' : this.activeTab === 'examen' ? 'examen' : '';
    if (!target) {
      return null;
    }

    return this.sessionData.sections.find((s) => s.name.toLowerCase().includes(target)) ?? null;
  }

  ngOnInit(): void {
    this.routeSub = this.route.paramMap.subscribe((params) => {
      this.sessionCode = params.get('sessionCode') ?? '';
      this.feedItems = [];
      this.clearTimer();

      if (!this.sessionCode) {
        this.loadError = true;
        return;
      }

      this.loadFromState();
      this.loadSessionStatus();
    });
  }

  ngOnDestroy(): void {
    this.routeSub?.unsubscribe();
    this.clearTimer();
  }

  onQuestionClick(question: SessionQuestionDto): void {
    if (this.isFinalized || !this.sessionCode) {
      return;
    }

    this.loadingQuestionId = question.questionId;

    this.apiClient.postEvent(this.sessionCode, { questionId: question.questionId }).subscribe({
      next: (response) => {
        this.feedItems.push({
          questionText: response.questionText,
          answerText: response.answerText
        });
        this.loadingQuestionId = null;
        this.resetClearTimer();
      },
      error: (error) => {
        this.loadingQuestionId = null;
        if (error?.status === 409) {
          this.isFinalized = true;
          this.statusText = 'Finalizada';
        }
      }
    });
  }

  private loadFromState(): void {
    const current = this.sessionStateService.getCurrentSession();
    if (current && current.sessionCode === this.sessionCode) {
      this.sessionData = current;
      this.missingSessionData = false;
      return;
    }

    this.sessionData = null;
    this.missingSessionData = true;
  }

  private loadSessionStatus(): void {
    this.loadError = false;

    this.apiClient.getSession(this.sessionCode).subscribe({
      next: (data) => {
        this.statusText = data.status === 'Finalized' ? 'Finalizada' : 'Activa';
        this.isFinalized = data.status === 'Finalized';
      },
      error: (error) => {
        this.loadError = error?.status === 404;
      }
    });
  }

  private resetClearTimer(): void {
    this.clearTimer();
    this.clearFeedTimeoutId = setTimeout(() => {
      this.feedItems = [];
    }, 60000);
  }

  private clearTimer(): void {
    if (this.clearFeedTimeoutId) {
      clearTimeout(this.clearFeedTimeoutId);
      this.clearFeedTimeoutId = null;
    }
  }
}
