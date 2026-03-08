import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { NgIf } from '@angular/common';
import { AdminCaseItem, CreateCasePayload } from '../../models/admin.models';

@Component({
  selector: 'app-admin-case-form',
  standalone: true,
  imports: [FormsModule, NgIf],
  template: `
    <div class="card">
      <h3>{{ model ? 'Editar caso' : 'Nuevo caso' }}</h3>
      <label>Nombre *</label>
      <input [(ngModel)]="form.fullName" />
      <label>Queja principal *</label>
      <input [(ngModel)]="form.chiefComplaint" />
      <label>Edad *</label>
      <input type="number" [(ngModel)]="form.age" />
      <label>Sexo *</label>
      <input [(ngModel)]="form.sex" />
      <label>Triaje *</label>
      <input [(ngModel)]="form.triage" />

      <p *ngIf="error" class="error">{{ error }}</p>
      <div class="row">
        <button (click)="submit()">Guardar</button>
        <button type="button" (click)="cancel.emit()">Cancelar</button>
      </div>
    </div>
  `
})
export class AdminCaseFormComponent {
  @Input() model: AdminCaseItem | null = null;
  @Output() save = new EventEmitter<CreateCasePayload>();
  @Output() cancel = new EventEmitter<void>();

  error = '';
  form: CreateCasePayload = { fullName: '', age: 0, sex: '', chiefComplaint: '', triage: '' };

  ngOnChanges(): void {
    if (this.model) {
      this.form = {
        fullName: this.model.fullName,
        age: this.model.age,
        sex: this.model.sex,
        chiefComplaint: this.model.chiefComplaint,
        triage: this.model.triage
      };
    } else {
      this.form = { fullName: '', age: 0, sex: '', chiefComplaint: '', triage: '' };
    }
  }

  submit(): void {
    this.error = '';
    if (!this.form.fullName.trim() || !this.form.chiefComplaint.trim() || !this.form.sex.trim() || !this.form.triage.trim() || this.form.age < 0) {
      this.error = 'Complete los campos obligatorios.';
      return;
    }

    this.save.emit({ ...this.form });
  }
}
