import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { BattleComponent } from '../features/battle/battle.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, BattleComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss',
})
export class AppComponent {
  title = 'ClientApp';
}
