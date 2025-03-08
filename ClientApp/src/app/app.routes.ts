import { Routes } from '@angular/router';
import { BattleComponent } from '../features/battle/battle.component';

export const routes: Routes = [
  { path: '', redirectTo: 'battle', pathMatch: 'full' },
  { path: 'battle', component: BattleComponent },
];
