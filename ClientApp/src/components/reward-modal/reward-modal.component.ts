import { Component, Inject } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import {
  MAT_DIALOG_DATA,
  MatDialogRef,
  MatDialogModule,
} from '@angular/material/dialog';
import { Reward } from './models/reward';
import { NgFor, NgIf } from '@angular/common';

@Component({
  selector: 'app-reward-modal',
  standalone: true,
  imports: [NgIf, NgFor, MatDialogModule, MatButtonModule],
  templateUrl: './reward-modal.component.html',
  styleUrl: './reward-modal.component.scss',
})
export class RewardModalComponent {
  objectKeys = Object.keys; // Allows us to iterate over stats dynamically

  constructor(
    public dialogRef: MatDialogRef<RewardModalComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) {
    console.log('data');
    console.log(data);
  }

  closeModal(): void {
    this.dialogRef.close();
  }

  openItemStatsDialog(): void {
    // Call the method passed from the parent (BattleResultComponent)
    if (this.data.openItemStatsDialog) {
      this.data.openItemStatsDialog(this.data.reward.drop); // Pass the drop data to the method
    }
  }
}
