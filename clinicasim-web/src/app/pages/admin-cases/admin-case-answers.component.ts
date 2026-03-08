import { Component, Input, OnInit } from '@angular/core';
import { NgFor, NgIf } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AdminQuestionService } from '../../core/services/admin-question.service';
import { AdminCaseService } from '../../core/services/admin-case.service';
import { CaseAnswerMapping, QuestionAdminItem } from '../../models/admin.models';

@Component({
  selector: 'app-admin-case-answers',
  standalone: true,
  imports: [NgFor, NgIf, FormsModule],
  template: `
    <h4>Respuestas del caso</h4>

    <div class="card">
      <h5>{{ editingId ? 'Editar sobrescritura' : 'Crear sobrescritura' }}</h5>
      <label>Pregunta</label>
      <select [(ngModel)]="form.questionId">
        <option [ngValue]="''">Seleccione</option>
        <option *ngFor="let q of questions" [ngValue]="q.id">{{ q.text }} ({{ q.section }} / {{ q.category }})</option>
      </select>
      <label>Respuesta</label>
      <input [(ngModel)]="form.answerText" />
      <label><input type="checkbox" [(ngModel)]="form.isCaseSpecific" /> Específica del caso</label>
      <label><input type="checkbox" [(ngModel)]="form.isHighlighted" /> Destacada</label>
      <div class="row">
        <button (click)="save()">Guardar</button>
        <button type="button" (click)="resetForm()">Cancelar</button>
      </div>
      <p *ngIf="error" class="error">{{ error }}</p>
    </div>

    <h5>Heredadas del sistema</h5>
    <table *ngIf="inheritedItems.length > 0">
      <thead><tr><th>Pregunta</th><th>Sección</th><th>Categoría</th><th>Respuesta</th></tr></thead>
      <tbody>
        <tr *ngFor="let item of inheritedItems">
          <td>{{ item.questionText }}</td><td>{{ item.section }}</td><td>{{ item.category }}</td><td>{{ item.answerText }}</td>
        </tr>
      </tbody>
    </table>

    <h5>Alteradas del caso</h5>
    <table *ngIf="overriddenItems.length > 0">
      <thead><tr><th>Pregunta</th><th>Sección</th><th>Categoría</th><th>Respuesta</th><th>Acciones</th></tr></thead>
      <tbody>
        <tr *ngFor="let item of overriddenItems">
          <td>{{ item.questionText }}</td><td>{{ item.section }}</td><td>{{ item.category }}</td><td>{{ item.answerText }}</td>
          <td>
            <button (click)="edit(item)">Editar</button>
            <button (click)="remove(item.overrideId)">Quitar override</button>
          </td>
        </tr>
      </tbody>
    </table>

    <h5>Destacadas / relevantes</h5>
    <table *ngIf="highlightedItems.length > 0">
      <thead><tr><th>Pregunta</th><th>Respuesta</th></tr></thead>
      <tbody><tr *ngFor="let item of highlightedItems"><td>{{ item.questionText }}</td><td>{{ item.answerText }}</td></tr></tbody>
    </table>
  `
})
export class AdminCaseAnswersComponent implements OnInit {
  @Input({ required: true }) caseId!: string;

  items: CaseAnswerMapping[] = [];
  questions: QuestionAdminItem[] = [];
  editingId: string | null = null;
  error = '';
  form = { questionId: '', answerText: '', isCaseSpecific: true, isHighlighted: false };

  constructor(
    private readonly adminCaseService: AdminCaseService,
    private readonly adminQuestionService: AdminQuestionService
  ) {}

  get inheritedItems(): CaseAnswerMapping[] { return this.items.filter(x => x.isInherited); }
  get overriddenItems(): CaseAnswerMapping[] { return this.items.filter(x => !x.isInherited); }
  get highlightedItems(): CaseAnswerMapping[] { return this.items.filter(x => x.isHighlighted); }

  ngOnInit(): void { this.loadQuestions(); this.load(); }

  load(): void {
    this.adminCaseService.getCaseAnswers(this.caseId).subscribe({ next: x => this.items = x });
  }

  loadQuestions(): void {
    this.adminQuestionService.getQuestions({ active: true }).subscribe({ next: x => this.questions = x });
  }

  save(): void {
    this.error = '';
    if (!this.form.questionId || !this.form.answerText.trim()) {
      this.error = 'Complete los campos obligatorios.';
      return;
    }

    const req$ = this.editingId
      ? this.adminCaseService.updateCaseAnswer(this.caseId, this.editingId, { ...this.form })
      : this.adminCaseService.addCaseAnswer(this.caseId, { ...this.form });

    req$.subscribe({
      next: () => { this.resetForm(); this.load(); },
      error: () => this.error = 'No se pudo guardar la sobrescritura.'
    });
  }

  edit(item: CaseAnswerMapping): void {
    if (!item.overrideId) return;
    this.editingId = item.overrideId;
    this.form = {
      questionId: item.questionId,
      answerText: item.answerText,
      isCaseSpecific: item.isCaseSpecific,
      isHighlighted: item.isHighlighted
    };
  }

  remove(overrideId: string | null): void {
    if (!overrideId) return;
    this.adminCaseService.deleteCaseAnswer(this.caseId, overrideId).subscribe({ next: () => this.load() });
  }

  resetForm(): void {
    this.editingId = null;
    this.form = { questionId: '', answerText: '', isCaseSpecific: true, isHighlighted: false };
  }
}
