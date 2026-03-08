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
    <h4>Hallazgos del caso</h4>

    <div class="card">
      <h5>{{ editingId ? 'Editar sobrescritura' : 'Crear sobrescritura' }}</h5>
      <label>Hallazgo</label>
      <select [(ngModel)]="form.findingId">
        <option [ngValue]="''">Seleccione</option>
        <option *ngFor="let f of findings" [ngValue]="f.id">{{ f.system }} - {{ f.name }}</option>
      </select>
      <label>Presente</label>
      <select [(ngModel)]="form.present"><option [ngValue]="true">Sí</option><option [ngValue]="false">No</option></select>
      <label>Detalle</label>
      <input [(ngModel)]="form.detailText" />
      <label><input type="checkbox" [(ngModel)]="form.isCaseSpecific" /> Específico del caso</label>
      <label><input type="checkbox" [(ngModel)]="form.isHighlighted" /> Destacado</label>
      <div class="row">
        <button (click)="save()">Guardar</button>
        <button type="button" (click)="resetForm()">Cancelar</button>
      </div>
      <p *ngIf="error" class="error">{{ error }}</p>
    </div>

    <h5>Heredados del sistema</h5>
    <table *ngIf="inheritedItems.length > 0">
      <thead><tr><th>Hallazgo</th><th>Sistema</th><th>Presente</th><th>Detalle</th></tr></thead>
      <tbody><tr *ngFor="let item of inheritedItems"><td>{{ item.findingName }}</td><td>{{ item.system }}</td><td>{{ item.present ? 'Sí' : 'No' }}</td><td>{{ item.detailText }}</td></tr></tbody>
    </table>

    <h5>Alterados del caso</h5>
    <table *ngIf="overriddenItems.length > 0">
      <thead><tr><th>Hallazgo</th><th>Sistema</th><th>Presente</th><th>Detalle</th><th>Acciones</th></tr></thead>
      <tbody>
        <tr *ngFor="let item of overriddenItems">
          <td>{{ item.findingName }}</td><td>{{ item.system }}</td><td>{{ item.present ? 'Sí' : 'No' }}</td><td>{{ item.detailText }}</td>
          <td><button (click)="edit(item)">Editar</button><button (click)="remove(item.overrideId)">Quitar override</button></td>
        </tr>
      </tbody>
    </table>

    <h5>Destacados</h5>
    <table *ngIf="highlightedItems.length > 0">
      <thead><tr><th>Hallazgo</th><th>Detalle</th></tr></thead>
      <tbody><tr *ngFor="let item of highlightedItems"><td>{{ item.findingName }}</td><td>{{ item.detailText }}</td></tr></tbody>
    </table>
  `
})
export class AdminCaseFindingsComponent implements OnInit {
  @Input({ required: true }) caseId!: string;

  items: CaseFindingMapping[] = [];
  findings: FindingAdminItem[] = [];
  editingId: string | null = null;
  error = '';
  form = { findingId: '', present: true, detailText: '', isCaseSpecific: true, isHighlighted: false };

  constructor(
    private readonly adminCaseService: AdminCaseService,
    private readonly adminFindingService: AdminFindingService
  ) {}

  get inheritedItems(): CaseFindingMapping[] { return this.items.filter(x => x.isInherited); }
  get overriddenItems(): CaseFindingMapping[] { return this.items.filter(x => !x.isInherited); }
  get highlightedItems(): CaseFindingMapping[] { return this.items.filter(x => x.isHighlighted); }

  ngOnInit(): void { this.loadFindings(); this.load(); }

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

    const payload = {
      findingId: this.form.findingId,
      present: this.form.present,
      detailText: this.form.detailText || null,
      isCaseSpecific: this.form.isCaseSpecific,
      isHighlighted: this.form.isHighlighted
    };

    const req$ = this.editingId
      ? this.adminCaseService.updateCaseFinding(this.caseId, this.editingId, payload)
      : this.adminCaseService.addCaseFinding(this.caseId, payload);

    req$.subscribe({ next: () => { this.resetForm(); this.load(); }, error: () => this.error = 'No se pudo guardar la sobrescritura.' });
  }

  edit(item: CaseFindingMapping): void {
    if (!item.overrideId) return;
    this.editingId = item.overrideId;
    this.form = {
      findingId: item.findingId,
      present: item.present,
      detailText: item.detailText ?? '',
      isCaseSpecific: item.isCaseSpecific,
      isHighlighted: item.isHighlighted
    };
  }

  remove(overrideId: string | null): void {
    if (!overrideId) return;
    this.adminCaseService.deleteCaseFinding(this.caseId, overrideId).subscribe({ next: () => this.load() });
  }

  resetForm(): void {
    this.editingId = null;
    this.form = { findingId: '', present: true, detailText: '', isCaseSpecific: true, isHighlighted: false };
  }
}
