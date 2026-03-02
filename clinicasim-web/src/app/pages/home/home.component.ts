import { Component } from '@angular/core';
import { NgIf } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
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
    <input [(ngModel)]="sessionCode" placeholder="Ingrese el código de sesión" />
    <button (click)="downloadPdf()" [disabled]="isDownloading">
      {{ isDownloading ? 'Descargando...' : 'Descargar PDF' }}
    </button>

    <p *ngIf="message" class="error">{{ message }}</p>
  `
})
export class HomeComponent {
  sessionCode = '';
  message = '';
  isDownloading = false;

  constructor(private readonly router: Router, private readonly apiClient: ApiClientService) {}

  goToCases(): void {
    this.router.navigateByUrl('/cases');
  }

  downloadPdf(): void {
    this.message = '';

    const code = this.sessionCode.trim();
    if (!code) {
      this.message = 'Ingrese un código.';
      return;
    }

    this.isDownloading = true;

    this.apiClient.downloadPdf(code).subscribe({
      next: (blob) => {
        const url = window.URL.createObjectURL(blob);
        const anchor = document.createElement('a');
        anchor.href = url;
        anchor.download = `ClinicaSim_${code}.pdf`;
        anchor.click();
        window.URL.revokeObjectURL(url);
        this.isDownloading = false;
      },
      error: (error: HttpErrorResponse) => {
        if (error.status === 404) {
          this.message = 'Código no encontrado.';
        } else if (error.status === 409) {
          this.message = 'La sesión no está finalizada.';
        } else {
          this.message = 'Error al descargar el PDF.';
        }

        this.isDownloading = false;
      }
    });
  }
}
