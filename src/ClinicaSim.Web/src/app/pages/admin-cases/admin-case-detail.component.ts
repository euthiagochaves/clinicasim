import { Component, EventEmitter, Input, Output } from '@angular/core';
import { NgIf } from '@angular/common';
import { AdminCaseItem, CreateCasePayload } from '../../models/admin.models';
import { AdminCaseFormComponent } from './admin-case-form.component';
import { AdminCaseAnswersComponent } from './admin-case-answers.component';
import { AdminCaseFindingsComponent } from './admin-case-findings.component';

@Component({
  selector: 'app-admin-case-detail',
  standalone: true,
  imports: [NgIf, AdminCaseFormComponent, AdminCaseAnswersComponent, AdminCaseFindingsComponent],
  template: `
    <div *ngIf="caseItem" class="card">
      <h3>Detalle del caso</h3>
      <p><strong>{{ caseItem.fullName }}</strong> - {{ caseItem.triage }}</p>
      <button (click)="editing = !editing">{{ editing ? 'Cerrar edición' : 'Editar caso' }}</button>

      <app-admin-case-form *ngIf="editing" [model]="caseItem" (save)="saveCase.emit($event)" (cancel)="editing = false" />

      <app-admin-case-answers [caseId]="caseItem.id" />
      <app-admin-case-findings [caseId]="caseItem.id" />
    </div>
  `
})
export class AdminCaseDetailComponent {
  @Input() caseItem: AdminCaseItem | null = null;
  @Output() saveCase = new EventEmitter<CreateCasePayload>();
  editing = false;
}
