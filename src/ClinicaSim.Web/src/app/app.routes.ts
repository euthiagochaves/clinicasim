import { Routes } from '@angular/router';
import { HomeComponent } from './pages/home/home.component';
import { CaseListComponent } from './pages/cases/case-list.component';
import { SessionComponent } from './pages/session/session.component';
import { AdminHomeComponent } from './pages/admin/admin-home.component';
import { AdminQuestionsComponent } from './pages/admin/admin-questions.component';
import { AdminFindingsComponent } from './pages/admin/admin-findings.component';
import { AdminCasesPageComponent } from './pages/admin-cases/admin-cases-page.component';

export const appRoutes: Routes = [
  { path: '', component: HomeComponent },
  { path: 'cases', component: CaseListComponent },
  { path: 'session/:sessionCode', component: SessionComponent },
  { path: 'admin', component: AdminHomeComponent },
  { path: 'admin/questions', component: AdminQuestionsComponent },
  { path: 'admin/findings', component: AdminFindingsComponent },
  { path: 'admin/cases', component: AdminCasesPageComponent },
  { path: '**', redirectTo: '' }
];
