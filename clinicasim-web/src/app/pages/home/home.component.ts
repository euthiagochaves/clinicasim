import { Component } from '@angular/core';
import { NgIf } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { ApiClientService } from '../../core/services/api-client.service';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [FormsModule, NgIf],
  template: `
    <h2>ClinicaSim</h2>
    <p>Simulación académica de atención clínica.</p>

    <button (click)="goToCases()">Comenzar</button>

    <hr />
    <h3>Exportar por código</h3>
    <input [(ngModel)]="sessionCode" placeholder="Código de sesión" />
    <button (click)="downloadPdf()">Descargar PDF</button>

    <p *ngIf="errorMessage" class="error">{{ errorMessage }}</p>
  `
})
export class HomeComponent {
  sessionCode = '';
  errorMessage = '';

  constructor(private readonly router: Router, private readonly apiClient: ApiClientService) {}

  goToCases(): void {
    this.router.navigateByUrl('/cases');
  }

  downloadPdf(): void {
    this.errorMessage = '';

    if (!this.sessionCode.trim()) {
      this.errorMessage = 'Ingrese un código de sesión.';
      return;
    }

    this.apiClient.downloadPdf(this.sessionCode.trim()).subscribe({
      next: (blob) => {
        const url = window.URL.createObjectURL(blob);
        const anchor = document.createElement('a');
        anchor.href = url;
        anchor.download = `ClinicaSim_${this.sessionCode.trim()}.pdf`;
        anchor.click();
        window.URL.revokeObjectURL(url);
      },
      error: () => {
        this.errorMessage = 'No se pudo descargar el PDF para ese código.';
      }
    });
  }
}
