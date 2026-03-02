import { Routes } from '@angular/router';
import { HomeComponent } from './pages/home/home.component';
import { CaseListComponent } from './pages/cases/case-list.component';
import { SessionComponent } from './pages/session/session.component';

export const appRoutes: Routes = [
  { path: '', component: HomeComponent },
  { path: 'cases', component: CaseListComponent },
  { path: 'session/:sessionCode', component: SessionComponent },
  { path: '**', redirectTo: '' }
];
