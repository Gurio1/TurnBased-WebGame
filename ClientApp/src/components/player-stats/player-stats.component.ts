import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PlayerHomeViewModel } from '../../features/home/contracts/playerHomeViewModel';
import { TooltipDirective } from '../../shared/tooltip.directive';

@Component({
  selector: 'app-player-stats',
  standalone: true,
  imports: [CommonModule, TooltipDirective],
  templateUrl: './player-stats.component.html',
  styleUrl: './player-stats.component.scss',
})
export class PlayerStatsComponent {
  @Input()
  character!: PlayerHomeViewModel;
}
