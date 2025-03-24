import { Component, OnDestroy, OnInit } from '@angular/core';
import { Character } from '../../core/models/player';
import { CharacterService } from '../../shared/character.service';
import { CommonModule } from '@angular/common';
import { PlayerStatsComponent } from '../../components/player-stats/player-stats.component';
import { InventoryComponent } from '../../components/inventory/inventory.component';
import { Subject, takeUntil } from 'rxjs';
@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, PlayerStatsComponent],
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss',
})
export class HomeComponent implements OnInit, OnDestroy {
  private destroy$ = new Subject<void>();

  character: Character | null = null;
  leftEquipmentSlots: string[] = ['Helmet', 'Armor', 'Gloves'];
  rightEquipmentSlots: string[] = ['Weapon', 'Shield', 'Boots'];

  constructor(private characterService: CharacterService) {}

  ngOnInit() {
    this.characterService.character$
      .pipe(takeUntil(this.destroy$))
      .subscribe((character) => {
        if (character) {
          this.character = character;
        }
      });

    this.characterService.getPlayer().subscribe();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  performAction(action: string, itemId: string): void {
    if (!itemId) return;

    this.characterService.makeAction(action, itemId).subscribe({
      error: (error) => console.error('Action failed:', error),
    });
  }
}
