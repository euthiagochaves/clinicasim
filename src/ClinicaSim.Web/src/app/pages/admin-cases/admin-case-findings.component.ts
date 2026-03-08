import { Component, Input, OnInit } from '@angular/core';
import { NgFor, NgIf } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AdminFindingService } from '../../core/services/admin-finding.service';
import { AdminCaseService } from '../../core/services/admin-case.service';
import { CaseFindingMapping, FindingAdminItem } from '../../models/admin.models';

@Component({
  selector: 'app-admin-case-findings',
  standalone: true,
  imports: [NgFor, NgIf, FormsModule],
  template: `
    <h4>Hallazgos por caso</h4>

    <div class="card">
      <h5>{{ editingId ? 'Editar hallazgo' : 'Agregar hallazgo' }}</h5>
      <label>Hallazgo</label>
      <select [(ngModel)]="form.findingId">
        <option [ngValue]="''">Seleccione</option>
        <option *ngFor="let f of findings" [ngValue]="f.id">{{ f.system }} - {{ f.name }}</option>
      </select>
      <label>Presente</label>
      <select [(ngModel)]="form.present">
        <option [ngValue]="true">Sí</option>
        <option [ngValue]="false">No</option>
      </select>
      <label>Detalle</label>
      <input [(ngModel)]="form.detailText" />
      <div class="row">
        <button (click)="save()">Guardar</button>
        <button type="button" (click)="resetForm()">Cancelar</button>
      </div>
      <p *ngIf="error" class="error">{{ error }}</p>
    </div>

    <table *ngIf="items.length > 0">
      <thead><tr><th>Hallazgo</th><th>Sistema</th><th>Presente</th><th>Detalle</th><th>Acciones</th></tr></thead>
      <tbody>
        <tr *ngFor="let item of items">
          <td>{{ item.findingName }}</td>
          <td>{{ item.system }}</td>
          <td>{{ item.present ? 'Sí' : 'No' }}</td>
          <td>{{ item.detailText }}</td>
          <td>
            <button (click)="edit(item)">Editar</button>
            <button (click)="remove(item.id)">Eliminar</button>
          </td>
        </tr>
      </tbody>
    </table>
  `
})
export class AdminCaseFindingsComponent implements OnInit {
  @Input({ required: true }) caseId!: string;

  items: CaseFindingMapping[] = [];
  findings: FindingAdminItem[] = [];
  editingId: string | null = null;
  error = '';
  form = { findingId: '', present: true, detailText: '' };

  constructor(
    private readonly adminCaseService: AdminCaseService,
    private readonly adminFindingService: AdminFindingService
  ) {}

  ngOnInit(): void {
    this.loadFindings();
    this.load();
  }

  load(): void {
    this.adminCaseService.getCaseFindings(this.caseId).subscribe({ next: x => this.items = x });
  }

  loadFindings(): void {
    this.adminFindingService.getFindings({ active: true }).subscribe({ next: x => this.findings = x });
  }

  save(): void {
    this.error = '';
    if (!this.form.findingId) {
      this.error = 'Complete los campos obligatorios.';
      return;
    }

    const req$ = this.editingId
      ? this.adminCaseService.updateCaseFinding(this.caseId, this.editingId, {
          findingId: this.form.findingId,
          present: this.form.present,
          detailText: this.form.detailText || null
        })
      : this.adminCaseService.addCaseFinding(this.caseId, {
          findingId: this.form.findingId,
          present: this.form.present,
          detailText: this.form.detailText || null
        });

    req$.subscribe({
      next: () => {
        this.resetForm();
        this.load();
      },
      error: () => this.error = 'No se pudo guardar el mapeo.'
    });
  }

  edit(item: CaseFindingMapping): void {
    this.editingId = item.id;
    this.form = { findingId: item.findingId, present: item.present, detailText: item.detailText ?? '' };
  }

  remove(mappingId: string): void {
    this.adminCaseService.deleteCaseFinding(this.caseId, mappingId).subscribe({ next: () => this.load() });
  }

  resetForm(): void {
    this.editingId = null;
    this.form = { findingId: '', present: true, detailText: '' };
  }
}
