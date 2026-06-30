import { Routes } from '@angular/router';
import { authGuard } from './core/auth/auth.guard';

export const routes: Routes = [
  {
    path: 'login',
    loadComponent: () => import('./features/auth/login/login.component').then((m) => m.LoginComponent)
  },
  {
    path: 'claims',
    canActivate: [authGuard],
    loadChildren: () => import('./features/claims/claims.routes').then((m) => m.CLAIMS_ROUTES)
  },
  {
    path: '',
    redirectTo: 'claims',
    pathMatch: 'full'
  },
  {
    path: '**',
    redirectTo: 'claims'
  }
];
