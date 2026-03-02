import { Component, OnInit } from '@angular/core';
import { NgFor, NgIf } from '@angular/common';
import { Router } from '@angular/router';
import { ApiClientService } from '../../core/services/api-client.service';
import { CaseListItemDto } from '../../models/api.models';

@Component({
  selector: 'app-case-list',
  standalone: true,
  imports: [NgFor, NgIf],
  template: `
    <h2>Lista de pacientes</h2>
    <p>Seleccione un caso para iniciar una sesión.</p>

    <p *ngIf="errorMessage" class="error">{{ errorMessage }}</p>

    <ul>
      <li *ngFor="let item of cases">
        <strong>{{ item.fullName }}</strong> — {{ item.age }} años — {{ item.sex }} — {{ item.triage }}
        <br />
        <small>{{ item.chiefComplaint }}</small>
        <br />
        <button (click)="startSession(item.caseId)">Iniciar sesión</button>
      </li>
    </ul>
  `
})
export class CaseListComponent implements OnInit {
  cases: CaseListItemDto[] = [];
  errorMessage = '';

  constructor(private readonly apiClient: ApiClientService, private readonly router: Router) {}

  ngOnInit(): void {
    this.apiClient.getCases().subscribe({
      next: (items) => (this.cases = items),
      error: () => (this.errorMessage = 'No se pudieron cargar los casos.')
    });
  }

  startSession(caseId: string): void {
    this.errorMessage = '';
    this.apiClient.startSession({ caseId }).subscribe({
      next: (result) => this.router.navigateByUrl(`/session/${result.sessionCode}`),
      error: () => (this.errorMessage = 'No se pudo iniciar la sesión.')
    });
  }
}
