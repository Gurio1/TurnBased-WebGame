import {
  Component,
  ElementRef,
  EventEmitter,
  HostListener,
  Input,
  Output,
  ViewChild,
  ViewEncapsulation,
} from '@angular/core';
import { MatTooltipModule } from '@angular/material/tooltip';
import { CommonModule } from '@angular/common';
import { CharacterService } from '../../shared/character.service';
import { Equipment } from '../../core/models/equipment';
import { Item } from '../../core/models/item';

@Component({
  selector: 'app-inventory',
  standalone: true,
  imports: [CommonModule, MatTooltipModule],
  templateUrl: './inventory.component.html',
  styleUrl: './inventory.component.scss',
  encapsulation: ViewEncapsulation.None,
})
export class InventoryComponent {
  @Input() equipmentInventoryItems!: Equipment[];
  @Input() otherInventoryItems!: Item[];

  @Output() itemHover = new EventEmitter<{
    item: Equipment | Item;
    element: HTMLElement;
  }>();
  @Output() itemLeave = new EventEmitter<void>();

  get allInventoryItems(): (Equipment | Item)[] {
    return [...this.equipmentInventoryItems, ...this.otherInventoryItems];
  }

  contextMenuVisible = false;
  contextMenuX = 0;
  contextMenuY = 0;
  selectedItem: Item | Equipment | null = null;
  hoveredItem: Item | Equipment | null = null;

  disableTooltipInteractivity = true;

  constructor(private characterService: CharacterService) {}

  openContextMenu(event: MouseEvent, item: any) {
    event.preventDefault();
    this.contextMenuVisible = true;
    this.selectedItem = item;

    // Get viewport dimensions
    const viewportWidth = window.innerWidth;
    const menuWidth = 120; // Should match your CSS width

    // Position horizontally
    if (event.clientX + menuWidth > viewportWidth) {
      // Show menu to the left of cursor if near right edge
      this.contextMenuX = event.clientX - menuWidth;
    } else {
      // Default: show to the right of cursor
      this.contextMenuX = event.clientX;
    }

    // Position vertically (similar logic for bottom edge)
    const viewportHeight = window.innerHeight;
    const menuHeight = item.interactions?.split(', ').length * 40 || 100; // Approx menu height

    if (event.clientY + menuHeight > viewportHeight) {
      this.contextMenuY = event.clientY - menuHeight;
    } else {
      this.contextMenuY = event.clientY;
    }
  }

  getEquipmentTooltip(item: Equipment): string {
    return `Name: ${item.name}
            Slot: ${item.slot}
            Id: ${item.id}
            ${item.attributes
              .map(
                (attr) =>
                  `${attr.name}: ${
                    this.isItPercentStat(attr.name)
                      ? attr.value * 100 + '%'
                      : attr.value
                  }`
              )
              .join('\n')}`;
  }
  getItemAttributes(item: Item | Equipment) {
    if (this.isEquipment(item)) {
      return item.attributes || [];
    }
    // Handle regular item attributes if they exist
    return (item as any).attributes || [];
  }

  isItPercentStat(statName: string) {
    return statName == 'Critical chance' || statName == 'Critical damage';
  }

  highlightItem(event: MouseEvent) {
    (event.currentTarget as HTMLElement).classList.add('active');
  }

  unhighlightItem(event: MouseEvent) {
    (event.currentTarget as HTMLElement).classList.remove('active');
  }

  performAction(action: string, itemId: string): void {
    if (!itemId) return;

    this.characterService.makeAction(action, itemId).subscribe({
      error: (error) => console.error('Action failed:', error),
    });
    this.contextMenuVisible = false;
  }

  // Add this to your component class
  isEquipment(item: any): item is Equipment {
    return (item as Equipment).slot !== undefined;
  }

  onItemHover(item: any, event: MouseEvent) {
    this.itemHover.emit({
      item: item,
      element: event.currentTarget as HTMLElement,
    });
  }

  onItemLeave(): void {
    this.itemLeave.emit();
  }

  @HostListener('document:click')
  closeContextMenu() {
    // Remove active class from all items
    document
      .querySelectorAll('.inventory-item')
      .forEach((el) => el.classList.remove('context-menu-active'));
    this.contextMenuVisible = false;
  }
}
