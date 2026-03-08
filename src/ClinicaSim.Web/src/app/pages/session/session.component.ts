import { Component, OnDestroy, OnInit } from '@angular/core';
import { NgFor, NgIf } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { Subscription } from 'rxjs';
import { HttpErrorResponse } from '@angular/common/http';
import { FormArray, FormBuilder, FormControl, ReactiveFormsModule, Validators } from '@angular/forms';
import { CdkDragDrop, DragDropModule, moveItemInArray } from '@angular/cdk/drag-drop';
import { ApiClientService } from '../../core/services/api-client.service';
import { SessionStateService } from '../../core/services/session-state.service';
import {
  ClinicalNoteRequest,
  DifferentialItemDto,
  SaveDifferentialsRequest,
  SessionInfoResponse,
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
  imports: [NgIf, NgFor, RouterLink, ReactiveFormsModule, DragDropModule],
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
      .field { margin-bottom: 10px; }
      .field textarea { width: 100%; min-height: 72px; }
      .ddx-row { display: flex; gap: 8px; align-items: center; margin-bottom: 6px; }
      .ddx-row input { flex: 1; }
      .drag-handle { cursor: move; padding: 0 6px; border: 1px solid #ccc; border-radius: 4px; }
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

    <div class="layout" *ngIf="!loadError">
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
          <div [formGroup]="historyForm">
            <div class="field">
              <label>Resumen</label>
              <textarea formControlName="summaryText" [readonly]="isFinalized"></textarea>
            </div>

            <div class="field">
              <label>Diagnósticos diferenciales (mínimo 5)</label>
              <div cdkDropList (cdkDropListDropped)="dropDdx($event)">
                <div class="ddx-row" *ngFor="let ctrl of differentials.controls; let i = index" cdkDrag>
                  <span class="drag-handle" cdkDragHandle>☰</span>
                  <input [formControl]="ctrl" [readonly]="isFinalized" placeholder="Diagnóstico diferencial" />
                  <button type="button" (click)="removeDdx(i)" [disabled]="isFinalized">Eliminar</button>
                </div>
              </div>
              <button type="button" (click)="addDdx()" [disabled]="isFinalized">Agregar diagnóstico</button>
              <p class="warn" *ngIf="showLocalErrors && differentialsNonEmptyCount < 5">
                Debe ingresar al menos 5 diagnósticos diferenciales.
              </p>
            </div>

            <div class="field">
              <label>Diagnóstico probable</label>
              <textarea formControlName="probableDiagnosisText" [readonly]="isFinalized"></textarea>
            </div>

            <div class="field">
              <label>Estudios sugeridos para confirmar</label>
              <textarea formControlName="conductStudiesText" [readonly]="isFinalized"></textarea>
            </div>

            <div class="field">
              <label>Tratamiento si se confirma</label>
              <textarea formControlName="conductTreatmentText" [readonly]="isFinalized"></textarea>
            </div>
          </div>

          <button type="button" (click)="saveDraft()" [disabled]="isFinalized || isSavingDraft">
            {{ isSavingDraft ? 'Guardando...' : 'Guardar borrador' }}
          </button>
          <button type="button" (click)="finalizeSession()" [disabled]="isFinalized || isFinalizing">
            {{ isFinalizing ? 'Finalizando...' : 'Finalizar consulta' }}
          </button>
          <button type="button" *ngIf="isFinalized" (click)="downloadPdf()" [disabled]="isDownloadingPdf">
            {{ isDownloadingPdf ? 'Descargando...' : 'Descargar PDF' }}
          </button>

          <p *ngIf="saveMessage">{{ saveMessage }}</p>
          <ul class="warn" *ngIf="localErrors.length > 0">
            <li *ngFor="let err of localErrors">{{ err }}</li>
          </ul>
          <ul class="warn" *ngIf="backendErrors.length > 0">
            <li *ngFor="let err of backendErrors">{{ err }}</li>
          </ul>
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
        <ng-container *ngIf="sessionInfo?.case as c">
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
  sessionInfo: SessionInfoResponse | null = null;
  missingSessionData = false;
  activeTab: SectionTab = 'anamnesis';
  feedItems: FeedItem[] = [];
  loadingQuestionId: string | null = null;

  isSavingDraft = false;
  isFinalizing = false;
  isDownloadingPdf = false;
  saveMessage = '';
  localErrors: string[] = [];
  backendErrors: string[] = [];
  showLocalErrors = false;

  historyForm = this.fb.group({
    summaryText: this.fb.nonNullable.control('', [Validators.required]),
    probableDiagnosisText: this.fb.nonNullable.control('', [Validators.required]),
    conductStudiesText: this.fb.nonNullable.control('', [Validators.required]),
    conductTreatmentText: this.fb.nonNullable.control('', [Validators.required]),
    differentials: this.fb.array<FormControl<string>>([])
  });

  private routeSub?: Subscription;
  private clearFeedTimeoutId: ReturnType<typeof setTimeout> | null = null;

  constructor(
    private readonly fb: FormBuilder,
    private readonly route: ActivatedRoute,
    private readonly apiClient: ApiClientService,
    private readonly sessionStateService: SessionStateService
  ) {}

  get differentials(): FormArray<FormControl<string>> {
    return this.historyForm.controls.differentials;
  }

  get differentialsNonEmptyCount(): number {
    return this.differentials.controls.filter((c) => c.value.trim().length > 0).length;
  }

  get activeSection(): SessionSectionDto | null {
    if (!this.sessionData) return null;
    const target = this.activeTab === 'anamnesis' ? 'anamnesis' : this.activeTab === 'examen' ? 'examen' : '';
    if (!target) return null;
    return this.sessionData.sections.find((s) => s.name.toLowerCase().includes(target)) ?? null;
  }

  ngOnInit(): void {
    this.routeSub = this.route.paramMap.subscribe((params) => {
      this.sessionCode = params.get('sessionCode') ?? '';
      this.feedItems = [];
      this.clearTimer();
      this.backendErrors = [];
      this.localErrors = [];
      this.saveMessage = '';

      if (!this.sessionCode) {
        this.loadError = true;
        return;
      }

      this.loadFromState();
      this.loadSessionStatus();
      this.loadHistoryData();
    });
  }

  ngOnDestroy(): void {
    this.routeSub?.unsubscribe();
    this.clearTimer();
  }

  onQuestionClick(question: SessionQuestionDto): void {
    if (this.isFinalized || !this.sessionCode) return;

    this.loadingQuestionId = question.questionId;
    this.apiClient.postEvent(this.sessionCode, { questionId: question.questionId }).subscribe({
      next: (response) => {
        this.feedItems.push({ questionText: response.questionText, answerText: response.answerText });
        this.loadingQuestionId = null;
        this.resetClearTimer();
      },
      error: (error) => {
        this.loadingQuestionId = null;
        if (error?.status === 409) {
          this.setFinalizedState();
        }
      }
    });
  }

  addDdx(): void {
    this.differentials.push(this.fb.nonNullable.control(''));
  }

  removeDdx(index: number): void {
    this.differentials.removeAt(index);
  }

  dropDdx(event: CdkDragDrop<FormControl<string>[]>): void {
    if (this.isFinalized || event.previousIndex === event.currentIndex) return;

    moveItemInArray(this.differentials.controls, event.previousIndex, event.currentIndex);
    this.differentials.updateValueAndValidity();
  }

  saveDraft(): void {
    if (this.isFinalized) return;

    this.isSavingDraft = true;
    this.backendErrors = [];
    this.localErrors = [];
    this.saveMessage = '';

    const noteRequest = this.buildNoteRequest();
    const ddxRequest = this.buildDifferentialsRequest();

    this.apiClient.saveNote(this.sessionCode, noteRequest).subscribe({
      next: () => {
        this.apiClient.saveDifferentials(this.sessionCode, ddxRequest).subscribe({
          next: () => {
            this.isSavingDraft = false;
            this.saveMessage = 'Guardado.';
          },
          error: () => {
            this.isSavingDraft = false;
            this.backendErrors = ['No se pudo guardar los diagnósticos diferenciales.'];
          }
        });
      },
      error: () => {
        this.isSavingDraft = false;
        this.backendErrors = ['No se pudo guardar la historia clínica.'];
      }
    });
  }

  finalizeSession(): void {
    if (this.isFinalized) return;

    this.showLocalErrors = true;
    this.localErrors = this.validateLocalBeforeFinalize();
    this.backendErrors = [];
    this.saveMessage = '';

    if (this.localErrors.length > 0) {
      return;
    }

    this.isFinalizing = true;
    const noteRequest = this.buildNoteRequest();
    const ddxRequest = this.buildDifferentialsRequest();

    this.apiClient.saveNote(this.sessionCode, noteRequest).subscribe({
      next: () => {
        this.apiClient.saveDifferentials(this.sessionCode, ddxRequest).subscribe({
          next: () => {
            this.apiClient.finalizeSession(this.sessionCode).subscribe({
              next: () => {
                this.isFinalizing = false;
                this.setFinalizedState();
              },
              error: (error: HttpErrorResponse) => {
                this.isFinalizing = false;
                if (error.status === 400 && error.error?.errors) {
                  this.backendErrors = error.error.errors;
                } else if (error.status === 409) {
                  this.setFinalizedState();
                  this.backendErrors = ['Sesión finalizada. Solo lectura.'];
                } else {
                  this.backendErrors = ['No se pudo finalizar la consulta.'];
                }
              }
            });
          },
          error: () => {
            this.isFinalizing = false;
            this.backendErrors = ['No se pudo guardar los diagnósticos diferenciales.'];
          }
        });
      },
      error: () => {
        this.isFinalizing = false;
        this.backendErrors = ['No se pudo guardar la historia clínica.'];
      }
    });
  }

  downloadPdf(): void {
    this.backendErrors = [];
    this.isDownloadingPdf = true;

    this.apiClient.downloadPdf(this.sessionCode).subscribe({
      next: (blob) => {
        const url = window.URL.createObjectURL(blob);
        const anchor = document.createElement('a');
        anchor.href = url;
        anchor.download = `ClinicaSim_${this.sessionCode}.pdf`;
        anchor.click();
        window.URL.revokeObjectURL(url);
        this.isDownloadingPdf = false;
      },
      error: (error: HttpErrorResponse) => {
        this.isDownloadingPdf = false;
        if (error.status === 404) this.backendErrors = ['Código no encontrado.'];
        else if (error.status === 409) this.backendErrors = ['La sesión no está finalizada.'];
        else this.backendErrors = ['Error al descargar el PDF.'];
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
        this.sessionInfo = data;
        this.statusText = data.status === 'Finalized' ? 'Finalizada' : 'Activa';
        if (data.status === 'Finalized') this.setFinalizedState();
      },
      error: (error) => {
        this.loadError = error?.status === 404;
      }
    });
  }

  private loadHistoryData(): void {
    this.apiClient.getNote(this.sessionCode).subscribe({
      next: (note) => {
        this.historyForm.patchValue({
          summaryText: note.summaryText ?? '',
          probableDiagnosisText: note.probableDiagnosisText ?? '',
          conductStudiesText: note.conductStudiesText ?? '',
          conductTreatmentText: note.conductTreatmentText ?? ''
        });
      }
    });

    this.apiClient.getDifferentials(this.sessionCode).subscribe({
      next: (items) => this.buildDdxControls(items),
      error: () => this.buildDdxControls([])
    });
  }

  private buildDdxControls(items: DifferentialItemDto[]): void {
    this.differentials.clear();

    const ordered = [...items].sort((a, b) => a.rank - b.rank);
    if (ordered.length === 0) {
      for (let i = 0; i < 5; i++) this.addDdx();
      return;
    }

    ordered.forEach((item) => this.differentials.push(this.fb.nonNullable.control(item.text ?? '')));
    while (this.differentials.length < 5) this.addDdx();
  }

  private buildNoteRequest(): ClinicalNoteRequest {
    return {
      summaryText: this.historyForm.controls.summaryText.value.trim(),
      probableDiagnosisText: this.historyForm.controls.probableDiagnosisText.value.trim(),
      conductStudiesText: this.historyForm.controls.conductStudiesText.value.trim(),
      conductTreatmentText: this.historyForm.controls.conductTreatmentText.value.trim()
    };
  }

  private buildDifferentialsRequest(): SaveDifferentialsRequest {
    const items = this.differentials.controls
      .map((c) => c.value.trim())
      .filter((value) => value.length > 0)
      .map((text, index) => ({ rank: index + 1, text }));

    return { items };
  }

  private validateLocalBeforeFinalize(): string[] {
    const errors: string[] = [];

    if (!this.historyForm.controls.summaryText.value.trim()) errors.push('El resumen es obligatorio.');
    if (!this.historyForm.controls.probableDiagnosisText.value.trim()) errors.push('El diagnóstico probable es obligatorio.');
    if (!this.historyForm.controls.conductStudiesText.value.trim()) errors.push('La conducta de estudios es obligatoria.');
    if (!this.historyForm.controls.conductTreatmentText.value.trim()) errors.push('La conducta de tratamiento es obligatoria.');
    if (this.differentialsNonEmptyCount < 5) errors.push('Debe ingresar al menos 5 diagnósticos diferenciales.');

    return errors;
  }

  private setFinalizedState(): void {
    this.isFinalized = true;
    this.statusText = 'Finalizada';
    this.historyForm.disable();
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
