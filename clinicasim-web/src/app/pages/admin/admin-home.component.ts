import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-admin-home',
  standalone: true,
  imports: [RouterLink],
  template: `
    <h2>Panel Admin</h2>
    <p>Gestione el banco global de preguntas y hallazgos.</p>
    <div class="admin-menu">
      <a routerLink="/admin/questions">Preguntas</a>
      <a routerLink="/admin/findings">Hallazgos</a>
    </div>
  `
})
export class AdminHomeComponent {}
