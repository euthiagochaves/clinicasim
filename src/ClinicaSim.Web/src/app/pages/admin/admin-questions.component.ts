import { Component, OnInit } from '@angular/core';
import { NgFor, NgIf } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AdminQuestionService } from '../../core/services/admin-question.service';
import { QuestionAdminItem } from '../../models/admin.models';

@Component({
  selector: 'app-admin-questions',
  standalone: true,
  imports: [NgIf, NgFor, FormsModule],
  template: `
    <h2>Preguntas globales</h2>

    <div class="toolbar">
      <button (click)="startCreate()">Nueva pregunta</button>
    </div>

    <div class="filters">
      <input [(ngModel)]="filters.search" placeholder="Buscar por texto" />
      <input [(ngModel)]="filters.section" placeholder="Sección" />
      <input [(ngModel)]="filters.category" placeholder="Categoría" />
      <select [(ngModel)]="activeFilter">
        <option value="true">Activas</option>
        <option value="false">Inactivas</option>
        <option value="all">Todas</option>
      </select>
      <button (click)="load()">Filtrar</button>
    </div>

    <p *ngIf="isLoading">Cargando...</p>
    <p *ngIf="errorMessage" class="error">{{ errorMessage }}</p>
    <p *ngIf="successMessage" class="success">{{ successMessage }}</p>

    <div *ngIf="showForm" class="card">
      <h3>{{ editingId ? 'Editar pregunta' : 'Nueva pregunta' }}</h3>
      <label>Texto *</label>
      <input [(ngModel)]="form.text" />

      <label>Sección *</label>
      <input [(ngModel)]="form.section" />

      <label>Categoría *</label>
      <input [(ngModel)]="form.category" />

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
          <th>Texto</th>
          <th>Sección</th>
          <th>Categoría</th>
          <th>Activa</th>
          <th>Acciones</th>
        </tr>
      </thead>
      <tbody>
        <tr *ngFor="let item of items">
          <td>{{ item.text }}</td>
          <td>{{ item.section }}</td>
          <td>{{ item.category }}</td>
          <td>{{ item.active ? 'Sí' : 'No' }}</td>
          <td>
            <button (click)="startEdit(item)">Editar</button>
            <button *ngIf="item.active" (click)="deactivate(item.id)">Desactivar</button>
            <button *ngIf="!item.active" (click)="activate(item.id)">Activar</button>
          </td>
        </tr>
      </tbody>
    </table>

    <p *ngIf="!isLoading && items.length === 0">No hay preguntas para mostrar.</p>
  `
})
export class AdminQuestionsComponent implements OnInit {
  items: QuestionAdminItem[] = [];
  isLoading = false;
  errorMessage = '';
  successMessage = '';

  filters = { search: '', section: '', category: '' };
  activeFilter: 'true' | 'false' | 'all' = 'true';

  showForm = false;
  editingId: string | null = null;
  form = { text: '', section: '', category: '', tags: '' };

  constructor(private readonly adminQuestionService: AdminQuestionService) {}

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.adminQuestionService.getQuestions({
      active: this.activeFilter === 'all' ? undefined : this.activeFilter === 'true',
      search: this.filters.search.trim() || undefined,
      section: this.filters.section.trim() || undefined,
      category: this.filters.category.trim() || undefined
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
    this.form = { text: '', section: '', category: '', tags: '' };
    this.successMessage = '';
    this.errorMessage = '';
  }

  startEdit(item: QuestionAdminItem): void {
    this.showForm = true;
    this.editingId = item.id;
    this.form = {
      text: item.text,
      section: item.section,
      category: item.category,
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

    if (!this.form.text.trim() || !this.form.section.trim() || !this.form.category.trim()) {
      this.errorMessage = 'Complete los campos obligatorios.';
      return;
    }

    const payload = {
      text: this.form.text,
      section: this.form.section,
      category: this.form.category,
      tags: this.form.tags || null
    };

    const request$ = this.editingId
      ? this.adminQuestionService.updateQuestion(this.editingId, { ...payload, active: undefined })
      : this.adminQuestionService.createQuestion(payload);

    request$.subscribe({
      next: () => {
        this.successMessage = this.editingId
          ? 'Pregunta actualizada correctamente.'
          : 'Pregunta creada correctamente.';
        this.showForm = false;
        this.editingId = null;
        this.load();
      },
      error: () => {
        this.errorMessage = 'No se pudo guardar la pregunta.';
      }
    });
  }

  deactivate(id: string): void {
    this.errorMessage = '';
    this.successMessage = '';
    this.adminQuestionService.deactivateQuestion(id).subscribe({
      next: () => {
        this.successMessage = 'Pregunta desactivada correctamente.';
        this.load();
      },
      error: () => {
        this.errorMessage = 'No se pudo desactivar la pregunta.';
      }
    });
  }

  activate(id: string): void {
    this.errorMessage = '';
    this.successMessage = '';
    this.adminQuestionService.activateQuestion(id).subscribe({
      next: () => {
        this.successMessage = 'Pregunta activada correctamente.';
        this.load();
      },
      error: () => {
        this.errorMessage = 'No se pudo activar la pregunta.';
      }
    });
  }
}
