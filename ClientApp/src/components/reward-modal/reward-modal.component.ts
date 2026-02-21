import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule, NgFor, NgIf } from '@angular/common';
import { Router } from '@angular/router';

export type RewardItemVisual = {
  name: string;
  imageUrl: string;
  quantity?: number;
};

@Component({
  selector: 'app-reward-modal',
  standalone: true,
  imports: [NgFor, CommonModule],
  templateUrl: './reward-modal.component.html',
  styleUrl: './reward-modal.component.scss',
})
export class RewardModalComponent {
  @Input() title = 'REWARD';
  @Input() subtitle: string | null = null;
  @Input() buttonLabel = 'CONTINUE';
  @Input() continueRoute: string | null = '/home';
  @Input() showCurrencies = true;
  @Input() gold: number = 0;
  @Input() gems: number = 0;
  @Input() items: RewardItemVisual[] = [];
  @Output() closed = new EventEmitter<void>();

  constructor(private router: Router) {
    this.gold = 100;
    this.gems = 50;
  }

  onContinue(): void {
    if (this.continueRoute) {
      void this.router.navigate([this.continueRoute]);
      return;
    }

    this.closed.emit();
  }

  getItemImage(imageUrl: string): string {
    if (!imageUrl) {
      return '';
    }

    return /\.(png|jpg|jpeg|webp|gif|svg)$/i.test(imageUrl) ? imageUrl : `${imageUrl}.png`;
  }
}
