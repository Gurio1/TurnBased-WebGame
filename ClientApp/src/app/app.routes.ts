import { Routes } from '@angular/router';
import { BattleComponent } from '../features/battle/battle.component';
import { LoginComponent } from '../features/identity/login/login.component';
import { RegisterComponent } from '../features/identity/register/register.component';
import { HomeComponent } from '../features/home/home.component';

export const routes: Routes = [
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'battle', component: BattleComponent },
  { path: 'home', component: HomeComponent },
];
