import { Routes } from '@angular/router';

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
    loadComponent: () =>
      import('../features/battle/battle.component').then((m) => m.BattleComponent),
  },
  {
    path: 'home',
    loadComponent: () => import('../features/home/home.component').then((m) => m.HomeComponent),
  },
  {
    path: 'map',
    loadComponent: () => import('../features/map/pages/map-page/map-page.component').then((m) => m.MapPageComponent),
  },
];
