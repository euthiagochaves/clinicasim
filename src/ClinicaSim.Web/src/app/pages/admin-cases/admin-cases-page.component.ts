import { Component, OnInit } from '@angular/core';
import { NgFor, NgIf } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AdminCaseService } from '../../core/services/admin-case.service';
import { AdminCaseItem, CreateCasePayload } from '../../models/admin.models';
import { AdminCaseFormComponent } from './admin-case-form.component';
import { AdminCaseDetailComponent } from './admin-case-detail.component';

@Component({
  selector: 'app-admin-cases-page',
  standalone: true,
  imports: [NgFor, NgIf, FormsModule, AdminCaseFormComponent, AdminCaseDetailComponent],
  template: `
    <h2>Casos clínicos</h2>

    <div class="toolbar">
      <button (click)="startCreate()">Nuevo caso</button>
    </div>

    <div class="filters">
      <input [(ngModel)]="filters.search" placeholder="Buscar" />
      <input [(ngModel)]="filters.triage" placeholder="Triaje" />
      <select [(ngModel)]="activeFilter">
        <option value="true">Activos</option>
        <option value="false">Inactivos</option>
        <option value="all">Todos</option>
      </select>
      <button (click)="load()">Filtrar</button>
    </div>

    <p *ngIf="message" class="success">{{ message }}</p>
    <p *ngIf="error" class="error">{{ error }}</p>

    <app-admin-case-form *ngIf="creating" (save)="create($event)" (cancel)="creating = false" />

    <table>
      <thead><tr><th>Nombre</th><th>Queja principal</th><th>Edad</th><th>Sexo</th><th>Triaje</th><th>Activo</th><th>Acciones</th></tr></thead>
      <tbody>
        <tr *ngFor="let item of items">
          <td>{{ item.fullName }}</td>
          <td>{{ item.chiefComplaint }}</td>
          <td>{{ item.age }}</td>
          <td>{{ item.sex }}</td>
          <td>{{ item.triage }}</td>
          <td>{{ item.active ? 'Sí' : 'No' }}</td>
          <td>
            <button (click)="select(item)">Ver detalle</button>
            <button *ngIf="item.active" (click)="deactivate(item.id)">Desactivar</button>
            <button *ngIf="!item.active" (click)="activate(item.id)">Activar</button>
          </td>
        </tr>
      </tbody>
    </table>

    <app-admin-case-detail *ngIf="selected" [caseItem]="selected" (saveCase)="updateSelected($event)" />
  `
})
export class AdminCasesPageComponent implements OnInit {
  items: AdminCaseItem[] = [];
  selected: AdminCaseItem | null = null;
  creating = false;
  message = '';
  error = '';

  filters = { search: '', triage: '' };
  activeFilter: 'true' | 'false' | 'all' = 'true';

  constructor(private readonly adminCaseService: AdminCaseService) {}

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.adminCaseService.getCases({
      active: this.activeFilter === 'all' ? undefined : this.activeFilter === 'true',
      search: this.filters.search.trim() || undefined,
      triage: this.filters.triage.trim() || undefined
    }).subscribe({
      next: items => this.items = items,
      error: () => this.error = 'No se pudo cargar la información.'
    });
  }

  startCreate(): void {
    this.creating = true;
    this.message = '';
    this.error = '';
  }

  create(payload: CreateCasePayload): void {
    this.adminCaseService.createCase(payload).subscribe({
      next: () => {
        this.creating = false;
        this.message = 'Caso creado correctamente.';
        this.load();
      },
      error: () => this.error = 'No se pudo crear el caso.'
    });
  }

  select(item: AdminCaseItem): void {
    this.selected = item;
  }

  updateSelected(payload: CreateCasePayload): void {
    if (!this.selected) return;

    this.adminCaseService.updateCase(this.selected.id, { ...payload, active: this.selected.active }).subscribe({
      next: updated => {
        this.selected = updated;
        this.message = 'Caso actualizado correctamente.';
        this.load();
      },
      error: () => this.error = 'No se pudo actualizar el caso.'
    });
  }

  deactivate(id: string): void {
    this.adminCaseService.deactivateCase(id).subscribe({
      next: () => {
        this.message = 'Caso desactivado correctamente.';
        this.load();
      }
    });
  }

  activate(id: string): void {
    this.adminCaseService.activateCase(id).subscribe({
      next: () => {
        this.message = 'Caso activado correctamente.';
        this.load();
      }
    });
  }
}
