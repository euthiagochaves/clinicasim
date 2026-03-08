import { Component } from '@angular/core';
import { RouterLink, RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, RouterLink],
  template: `
    <div class="container">
      <h1>ClinicaSim Web</h1>
      <nav>
        <a routerLink="/">Inicio</a>
        <a routerLink="/cases">Casos</a>
        <a routerLink="/admin">Admin</a>
      </nav>
      <hr />
      <router-outlet />
    </div>
  `
})
export class AppComponent {}
