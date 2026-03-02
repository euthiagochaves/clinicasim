import { Component, OnInit } from '@angular/core';
import { NgFor, NgIf } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { ApiClientService } from '../../core/services/api-client.service';
import { SessionStateService } from '../../core/services/session-state.service';
import { CaseListItemDto } from '../../models/api.models';

@Component({
  selector: 'app-case-list',
  standalone: true,
  imports: [NgFor, NgIf, FormsModule],
  template: `
    <h2>Pacientes</h2>

    <input
      [(ngModel)]="searchTerm"
      (ngModelChange)="applyFilter()"
      placeholder="Buscar por nombre o motivo de consulta"
    />

    <p *ngIf="isLoading">Cargando...</p>
    <p *ngIf="!isLoading && loadError" class="error">Error al cargar pacientes.</p>
    <p *ngIf="actionError" class="error">{{ actionError }}</p>

    <ul *ngIf="!isLoading && !loadError">
      <li *ngFor="let item of filteredCases" style="margin-bottom: 12px;">
        <strong>{{ item.fullName }}</strong>
        <span style="margin-left: 8px; font-size: 12px;">({{ item.triage }})</span>
        <br />
        <small>{{ item.chiefComplaint }}</small>
        <br />
        <button (click)="startSession(item.caseId)" [disabled]="loadingCaseId === item.caseId">
          {{ loadingCaseId === item.caseId ? 'Iniciando...' : 'Atender' }}
        </button>
      </li>
    </ul>

    <p *ngIf="!isLoading && !loadError && filteredCases.length === 0">No se encontraron pacientes.</p>
  `
})
export class CaseListComponent implements OnInit {
  cases: CaseListItemDto[] = [];
  filteredCases: CaseListItemDto[] = [];
  searchTerm = '';

  isLoading = true;
  loadError = false;
  loadingCaseId: string | null = null;
  actionError = '';

  constructor(
    private readonly apiClient: ApiClientService,
    private readonly sessionStateService: SessionStateService,
    private readonly router: Router
  ) {}

  ngOnInit(): void {
    this.apiClient.getCases().subscribe({
      next: (items) => {
        this.cases = items;
        this.applyFilter();
        this.isLoading = false;
      },
      error: () => {
        this.loadError = true;
        this.isLoading = false;
      }
    });
  }

  applyFilter(): void {
    const term = this.searchTerm.trim().toLowerCase();
    if (!term) {
      this.filteredCases = [...this.cases];
      return;
    }

    this.filteredCases = this.cases.filter((item) =>
      item.fullName.toLowerCase().includes(term) || item.chiefComplaint.toLowerCase().includes(term)
    );
  }

  startSession(caseId: string): void {
    this.actionError = '';
    this.loadingCaseId = caseId;

    this.apiClient.startSession({ caseId }).subscribe({
      next: (result) => {
        this.loadingCaseId = null;
        this.sessionStateService.setCurrentSession(result);
        this.router.navigateByUrl(`/session/${result.sessionCode}`);
      },
      error: () => {
        this.loadingCaseId = null;
        this.actionError = 'No se pudo iniciar la sesión.';
      }
    });
  }
}
