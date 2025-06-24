import {
  Component,
  ElementRef,
  OnDestroy,
  OnInit,
  ViewChild,
} from '@angular/core';
import { CharacterService } from '../../shared/character.service';
import { CommonModule } from '@angular/common';
import { PlayerStatsComponent } from '../../components/player-stats/player-stats.component';
import { InventoryComponent } from '../../components/inventory/inventory.component';
import { Subject, takeUntil } from 'rxjs';
import { PlayerHomeViewModel } from './contracts/playerHomeViewModel';
import { Equipment } from '../../core/models/equipment';
import { Item } from '../../core/models/item';
import { InventoryTooltipComponent } from '../../components/inventory-tooltip/inventory-tooltip.component';
@Component({
  selector: 'app-home',
  standalone: true,
  imports: [
    CommonModule,
    PlayerStatsComponent,
    InventoryComponent,
    InventoryTooltipComponent,
  ],
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss',
})
export class HomeComponent implements OnInit, OnDestroy {
  @ViewChild('tooltipAnchor', { static: true }) tooltipAnchor!: ElementRef;
  activeTooltipItem: any = null;
  tooltipPosition = { x: 0, y: 0 };
  private tooltipDimensions = { width: 220, height: 150 };

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

  showTooltip(data: { item: any; element: HTMLElement }) {
    this.activeTooltipItem = data.item;

    // Calculate position immediately using estimated dimensions
    this.updateTooltipPosition(data.element);

    // Update with real dimensions after view updates
    setTimeout(() => this.updateTooltipPosition(data.element));
  }

  private updateTooltipPosition(itemElement: HTMLElement) {
    if (!this.activeTooltipItem) return;

    const itemRect = itemElement.getBoundingClientRect();
    const padding = 10;

    this.tooltipPosition = {
      x: itemRect.left - this.tooltipDimensions.width - padding,
      y: itemRect.top + (itemRect.height - this.tooltipDimensions.height) / 2,
    };

    // Ensure tooltip stays within viewport
    this.tooltipPosition.x = Math.max(padding, this.tooltipPosition.x);
    this.tooltipPosition.y = Math.max(padding, this.tooltipPosition.y);
  }

  // Call this when tooltip dimensions are known
  onTooltipLoaded(dimensions: { width: number; height: number }) {
    this.tooltipDimensions = dimensions;
    // Re-position if active tooltip exists
    if (this.activeTooltipItem) {
      const items = document.querySelectorAll('.inventory-item');
      const lastHovered = Array.from(items).find(
        (el) => el.querySelector(':hover') !== null
      );
      if (lastHovered) {
        this.updateTooltipPosition(lastHovered as HTMLElement);
      }
    }
  }

  hideTooltip() {
    this.activeTooltipItem = null;
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
