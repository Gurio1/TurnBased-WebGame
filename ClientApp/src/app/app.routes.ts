import { Routes } from '@angular/router';
import { authGuard } from '../features/identity/auth.guard';

export const routes: Routes = [
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  {
    path: 'login',
    loadComponent: () =>
      import('../features/identity/login/login.component').then((m) => m.LoginComponent),
  },
  {
    path: 'register',
    loadComponent: () =>
      import('../features/identity/register/register.component').then((m) => m.RegisterComponent),
  },
  {
    path: 'battle',
    canActivate: [authGuard],
    loadComponent: () =>
      import('../features/battle/battle.component').then((m) => m.BattleComponent),
  },
  {
    path: 'home',
    canActivate: [authGuard],
    loadComponent: () => import('../features/home/home.component').then((m) => m.HomeComponent),
  },
  {
    path: 'map',
    canActivate: [authGuard],
    loadComponent: () =>
      import('../features/map/map-page/map-page.component').then((m) => m.MapPageComponent),
  },
];
