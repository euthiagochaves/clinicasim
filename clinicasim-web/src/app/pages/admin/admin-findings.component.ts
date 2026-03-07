import { Component, OnInit } from '@angular/core';
import { NgFor, NgIf } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AdminFindingService } from '../../core/services/admin-finding.service';
import { FindingAdminItem } from '../../models/admin.models';

@Component({
  selector: 'app-admin-findings',
  standalone: true,
  imports: [NgIf, NgFor, FormsModule],
  template: `
    <h2>Hallazgos globales</h2>

    <div class="toolbar">
      <button (click)="startCreate()">Nuevo hallazgo</button>
    </div>

    <div class="filters">
      <input [(ngModel)]="filters.search" placeholder="Buscar por nombre" />
      <input [(ngModel)]="filters.system" placeholder="Sistema" />
      <select [(ngModel)]="activeFilter">
        <option value="true">Activos</option>
        <option value="false">Inactivos</option>
        <option value="all">Todos</option>
      </select>
      <button (click)="load()">Filtrar</button>
    </div>

    <p *ngIf="isLoading">Cargando...</p>
    <p *ngIf="errorMessage" class="error">{{ errorMessage }}</p>
    <p *ngIf="successMessage" class="success">{{ successMessage }}</p>

    <div *ngIf="showForm" class="card">
      <h3>{{ editingId ? 'Editar hallazgo' : 'Nuevo hallazgo' }}</h3>
      <label>Nombre *</label>
      <input [(ngModel)]="form.name" />

      <label>Sistema *</label>
      <input [(ngModel)]="form.system" />

      <label>Tags</label>
      <input [(ngModel)]="form.tags" />

      <div class="row">
        <button (click)="save()">Guardar</button>
        <button type="button" (click)="cancelForm()">Cancelar</button>
      </div>
    </div>

    <table *ngIf="!isLoading && items.length > 0">
      <thead>
        <tr>
          <th>Nombre</th>
          <th>Sistema</th>
          <th>Activo</th>
          <th>Acciones</th>
        </tr>
      </thead>
      <tbody>
        <tr *ngFor="let item of items">
          <td>{{ item.name }}</td>
          <td>{{ item.system }}</td>
          <td>{{ item.active ? 'Sí' : 'No' }}</td>
          <td>
            <button (click)="startEdit(item)">Editar</button>
            <button *ngIf="item.active" (click)="deactivate(item.id)">Desactivar</button>
            <button *ngIf="!item.active" (click)="activate(item.id)">Activar</button>
          </td>
        </tr>
      </tbody>
    </table>

    <p *ngIf="!isLoading && items.length === 0">No hay hallazgos para mostrar.</p>
  `
})
export class AdminFindingsComponent implements OnInit {
  items: FindingAdminItem[] = [];
  isLoading = false;
  errorMessage = '';
  successMessage = '';

  filters = { search: '', system: '' };
  activeFilter: 'true' | 'false' | 'all' = 'true';

  showForm = false;
  editingId: string | null = null;
  form = { name: '', system: '', tags: '' };

  constructor(private readonly adminFindingService: AdminFindingService) {}

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.adminFindingService.getFindings({
      active: this.activeFilter === 'all' ? undefined : this.activeFilter === 'true',
      search: this.filters.search.trim() || undefined,
      system: this.filters.system.trim() || undefined
    }).subscribe({
      next: (items) => {
        this.items = items;
        this.isLoading = false;
      },
      error: () => {
        this.errorMessage = 'No se pudo cargar la información.';
        this.isLoading = false;
      }
    });
  }

  startCreate(): void {
    this.showForm = true;
    this.editingId = null;
    this.form = { name: '', system: '', tags: '' };
    this.successMessage = '';
    this.errorMessage = '';
  }

  startEdit(item: FindingAdminItem): void {
    this.showForm = true;
    this.editingId = item.id;
    this.form = {
      name: item.name,
      system: item.system,
      tags: item.tags ?? ''
    };
    this.successMessage = '';
    this.errorMessage = '';
  }

  cancelForm(): void {
    this.showForm = false;
    this.editingId = null;
  }

  save(): void {
    this.errorMessage = '';
    this.successMessage = '';

    if (!this.form.name.trim() || !this.form.system.trim()) {
      this.errorMessage = 'Complete los campos obligatorios.';
      return;
    }

    const payload = {
      name: this.form.name,
      system: this.form.system,
      tags: this.form.tags || null
    };

    const request$ = this.editingId
      ? this.adminFindingService.updateFinding(this.editingId, { ...payload, active: undefined })
      : this.adminFindingService.createFinding(payload);

    request$.subscribe({
      next: () => {
        this.successMessage = this.editingId
          ? 'Hallazgo actualizado correctamente.'
          : 'Hallazgo creado correctamente.';
        this.showForm = false;
        this.editingId = null;
        this.load();
      },
      error: () => {
        this.errorMessage = 'No se pudo guardar el hallazgo.';
      }
    });
  }

  deactivate(id: string): void {
    this.errorMessage = '';
    this.successMessage = '';
    this.adminFindingService.deactivateFinding(id).subscribe({
      next: () => {
        this.successMessage = 'Hallazgo desactivado correctamente.';
        this.load();
      },
      error: () => {
        this.errorMessage = 'No se pudo desactivar el hallazgo.';
      }
    });
  }

  activate(id: string): void {
    this.errorMessage = '';
    this.successMessage = '';
    this.adminFindingService.activateFinding(id).subscribe({
      next: () => {
        this.successMessage = 'Hallazgo activado correctamente.';
        this.load();
      },
      error: () => {
        this.errorMessage = 'No se pudo activar el hallazgo.';
      }
    });
  }
}
