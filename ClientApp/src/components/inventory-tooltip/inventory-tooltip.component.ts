// inventory-tooltip.component.ts
import {
  Component,
  ElementRef,
  EventEmitter,
  Input,
  Output,
  ViewChild,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { Equipment } from '../../core/models/equipment';
import { Item } from '../../core/models/item';

@Component({
  selector: 'app-inventory-tooltip',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './inventory-tooltip.component.html',
  styleUrls: ['./inventory-tooltip.component.scss'],
})
export class InventoryTooltipComponent {
  @Input() item: any;
  @Input() position = { x: 0, y: 0 };
  @Output() loaded = new EventEmitter<{ width: number; height: number }>();

  @ViewChild('tooltip', { static: true }) tooltip!: ElementRef;

  ngAfterViewInit() {
    const rect = this.tooltip.nativeElement.getBoundingClientRect();
    this.loaded.emit({
      width: rect.width,
      height: rect.height,
    });
  }

  isEquipment(item: any): item is Equipment {
    return (item as Equipment).slot !== undefined;
  }

  isItPercentStat(statName: string): boolean {
    return (
      statName.toLowerCase().includes('percent') ||
      statName.toLowerCase().includes('chance') ||
      statName === 'Critical chance' ||
      statName === 'Critical damage'
    );
  }
}
