import { Component, Input } from '@angular/core';
import { Character } from '../../core/models/player';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-player-stats',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './player-stats.component.html',
  styleUrl: './player-stats.component.scss',
})
export class PlayerStatsComponent {
  @Input()
  character!: Character;
}
