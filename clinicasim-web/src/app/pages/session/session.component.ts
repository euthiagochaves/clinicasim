import { Component, OnInit } from '@angular/core';
import { NgIf } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { ApiClientService } from '../../core/services/api-client.service';

@Component({
  selector: 'app-session',
  standalone: true,
  imports: [NgIf, RouterLink],
  template: `
    <h2>Atención clínica</h2>
    <p><strong>Sesión:</strong> {{ sessionCode }}</p>

    <p *ngIf="isLoading">Cargando sesión...</p>
    <p *ngIf="!isLoading && statusText">Estado: {{ statusText }}</p>
    <p *ngIf="notFound" class="error">Sesión no encontrada.</p>
    <a *ngIf="notFound" routerLink="/cases">Volver a pacientes</a>

    <p>Pantalla de atención (se implementa en Tareas 7/8).</p>
  `
})
export class SessionComponent implements OnInit {
  sessionCode = '';
  statusText = '';
  isLoading = true;
  notFound = false;

  constructor(private readonly route: ActivatedRoute, private readonly apiClient: ApiClientService) {}

  ngOnInit(): void {
    this.route.paramMap.subscribe((params) => {
      this.sessionCode = params.get('sessionCode') ?? '';
      if (!this.sessionCode) {
        this.isLoading = false;
        this.notFound = true;
        return;
      }

      this.loadSession();
    });
  }

  private loadSession(): void {
    this.isLoading = true;
    this.notFound = false;

    this.apiClient.getSession(this.sessionCode).subscribe({
      next: (data) => {
        this.statusText = data.status === 'Finalized' ? 'Finalizada' : 'Activa';
        this.isLoading = false;
      },
      error: (error) => {
        this.isLoading = false;
        this.notFound = error?.status === 404;
      }
    });
  }
}
