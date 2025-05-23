import { Component, OnDestroy, OnInit } from '@angular/core';
import { CharacterService } from '../../shared/character.service';
import { CommonModule } from '@angular/common';
import { PlayerStatsComponent } from '../../components/player-stats/player-stats.component';
import { InventoryComponent } from '../../components/inventory/inventory.component';
import { Subject, takeUntil } from 'rxjs';
import { PlayerHomeViewModel } from './contracts/playerHomeViewModel';
@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, PlayerStatsComponent, InventoryComponent],
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss',
})
export class HomeComponent implements OnInit, OnDestroy {
  private destroy$ = new Subject<void>();

  character: PlayerHomeViewModel | null = null;
  leftEquipmentSlots: string[] = ['Head', 'Chest', 'Gloves'];
  rightEquipmentSlots: string[] = ['Weapon', 'Shield', 'Boots'];
  statsVisible: boolean = true;
  inventoryVisible: boolean = true;

  constructor(private characterService: CharacterService) {}

  ngOnInit() {
    this.characterService.character$
      .pipe(takeUntil(this.destroy$))
      .subscribe((character) => {
        if (character) {
          console.log(character);
          this.character = character;
        }
      });

    this.characterService.getPlayer().subscribe();
  }

  toggleStats(): void {
    this.statsVisible = !this.statsVisible;
    // You could add animation triggers here if needed
  }

  toggleInventory(): void {
    this.inventoryVisible = !this.inventoryVisible;
    // You could add animation triggers here if needed
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
