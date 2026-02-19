import { Component, Input } from '@angular/core';
import { MapLocation } from '../../core/models/mapLocation';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-world-map',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './world-map.component.html',
  styleUrl: './world-map.component.scss',
})
export class WorldMapComponent {
  @Input({ required: true }) mapImageUrl!: string;
  @Input({ required: true }) locations: MapLocation[] = [];

  selected: MapLocation | null = null;

  select(loc: MapLocation) {
    this.selected = loc;
  }

  closeSidebar() {
    this.selected = null;
  }

  trackById = (_: number, l: MapLocation) => l.id;
}
