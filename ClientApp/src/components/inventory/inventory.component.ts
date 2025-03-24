import {
  AfterViewInit,
  Component,
  HostListener,
  Input,
  ViewChild,
  ViewEncapsulation,
} from '@angular/core';
import { MatTooltip, MatTooltipModule } from '@angular/material/tooltip';
import { Equipment, Item } from '../../core/models/player';
import { CommonModule } from '@angular/common';
import { CharacterService } from '../../shared/character.service';

@Component({
  selector: 'app-inventory',
  standalone: true,
  imports: [CommonModule, MatTooltipModule],
  templateUrl: './inventory.component.html',
  styleUrl: './inventory.component.scss',
  encapsulation: ViewEncapsulation.None,
})
export class InventoryComponent implements AfterViewInit {
  @Input()
  equipmentInventoryItems!: Equipment[];
  @Input()
  otherInventoryItems!: Item[];

  @ViewChild(MatTooltip) tooltip: MatTooltip | undefined;

  contextMenuVisible = false;
  contextMenuX = 0;
  contextMenuY = 0;
  selectedItem: Item | null = null;
  hoveredItem: Equipment | null = null;

  disableTooltipInteractivity = true;

  constructor(private characterService: CharacterService) {}

  ngAfterViewInit() {
    // Listen for hover events on the tooltip trigger element
    if (this.tooltip?._tooltipInstance) {
      this.tooltip._tooltipInstance.afterHidden().subscribe(() => {
        // Tooltip hidden, reset any state or actions here if needed
      });
    }
  }

  showTooltip(event: Event) {
    if (this.tooltip) {
      this.tooltip.show();
    }
  }

  hideTooltip(event: Event) {
    if (this.tooltip) {
      this.tooltip.hide();
    }
  }

  openContextMenu(event: MouseEvent, item: Item) {
    event.preventDefault(); // Prevent default right-click menu
    this.contextMenuVisible = true;
    this.contextMenuX = event.clientX;
    this.contextMenuY = event.clientY;
    this.selectedItem = item;
  }

  getEquipmentTooltip(item: Equipment): string {
    return `Name: ${item.name}
            Slot: ${item.slot}
            ${item.attributes
              .map((attr) => `${attr.name}: ${attr.value}`)
              .join('\n')}`;
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
