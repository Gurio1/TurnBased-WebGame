import { Component, HostListener, Input } from '@angular/core';
import { Equipment, Item } from '../../core/models/player';
import { CommonModule } from '@angular/common';
import { CharacterService } from '../../shared/character.service';

@Component({
  selector: 'app-inventory',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './inventory.component.html',
  styleUrl: './inventory.component.scss',
})
export class InventoryComponent {
  @Input()
  equipmentInventoryItems!: Equipment[];
  @Input()
  otherInventoryItems!: Item[];

  contextMenuVisible = false;
  contextMenuX = 0;
  contextMenuY = 0;
  selectedItem: Item | null = null;
  hoveredItem: Equipment | null = null;

  constructor(private characterService: CharacterService) {}

  openContextMenu(event: MouseEvent, item: Item) {
    event.preventDefault(); // Prevent default right-click menu
    this.contextMenuVisible = true;
    this.contextMenuX = event.clientX;
    this.contextMenuY = event.clientY;
    this.selectedItem = item;
  }

  performAction(action: string, itemId: string): void {
    if (!itemId) return;

    this.characterService.makeAction(action, itemId).subscribe({
      error: (error) => console.error('Action failed:', error),
    });
    this.contextMenuVisible = false;
  }

  onItemHover(item: Equipment | any): void {
    this.hoveredItem = item;
  }

  onItemLeave(): void {
    this.hoveredItem = null;
  }

  @HostListener('document:click')
  closeContextMenu() {
    this.contextMenuVisible = false;
  }
}
