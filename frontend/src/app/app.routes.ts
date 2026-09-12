import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  {
    path: 'login',
    loadComponent: () =>
      import('./pages/login/login').then(m => m.LoginComponent)
  },
  {
    path: 'purchase-bill',
    loadComponent: () =>
      import('./pages/purchase-bill/purchase-bill').then(m => m.PurchaseBillComponent),
    canActivate: [authGuard]
  },
  { path: '**', redirectTo: 'login' }
];

