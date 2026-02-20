import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';
import { MapLocation } from '../../../../core/models/map-location';

@Component({
  selector: 'app-world-map',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './world-map.component.html',
  styleUrl: './world-map.component.scss',
})
export class WorldMapComponent {
  readonly difficultyLevels = [1, 2, 3, 4, 5] as const;

  @Input({ required: true }) mapImageUrl!: string;
  @Input({ required: true }) locations: MapLocation[] = [];

  selected: MapLocation | null = null;

  select(loc: MapLocation): void {
    this.selected = loc;
  }

  closeSidebar(): void {
    this.selected = null;
  }

  trackById = (_: number, l: MapLocation) => l.id;
}
