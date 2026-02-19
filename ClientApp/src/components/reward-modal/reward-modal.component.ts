import { Component, Inject, Input } from '@angular/core';
import { Reward } from './models/reward';
import { CommonModule, NgFor, NgIf } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';
import { Item } from '../../core/models/item';
import { Equipment } from '../../core/models/equipment';
import { Router } from '@angular/router';

@Component({
  selector: 'app-reward-modal',
  standalone: true,
  imports: [NgFor, CommonModule],
  templateUrl: './reward-modal.component.html',
  styleUrl: './reward-modal.component.scss',
})
export class RewardModalComponent {
  @Input() gold: number = 0;
  @Input() gems: number = 0;
  @Input() items: Equipment[] = [];

  constructor(private router: Router) {
    this.gold = 100;
    this.gems = 50;
  }

  goHome() {
    this.router.navigate(['/home']);
  }
}
