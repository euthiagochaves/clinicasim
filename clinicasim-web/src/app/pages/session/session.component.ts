import { Component } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-session',
  standalone: true,
  template: `
    <h2>Atención clínica</h2>
    <p><strong>Sesión:</strong> {{ sessionCode }}</p>
    <p>Pantalla de atención (se implementa en Tareas 7/8).</p>
  `
})
export class SessionComponent {
  sessionCode = '';

  constructor(private readonly route: ActivatedRoute) {
    this.route.paramMap.subscribe((params) => {
      this.sessionCode = params.get('sessionCode') ?? '';
    });
  }
}
