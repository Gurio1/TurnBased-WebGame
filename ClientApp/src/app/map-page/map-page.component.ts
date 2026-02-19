import { Component } from '@angular/core';
import { MapLocation } from '../../core/models/mapLocation';
import { CommonModule } from '@angular/common';
import { WorldMapComponent } from '../world-map/world-map.component';

@Component({
  selector: 'app-map-page',
  standalone: true,
  imports: [CommonModule, WorldMapComponent],
  templateUrl: './map-page.component.html',
  styleUrl: './map-page.component.scss',
})
export class MapPageComponent {
  map = 'map.png';

  locations: MapLocation[] = [
    {
      id: 'newcomers-village',
      name: "Newcomers' Village",
      description:
        'A quiet place where weak monsters roam and basic materials can be gathered.',
      imageUrl: '/assets/locations/newcomers-village.jpg',
      monsters: ['Rat', 'Wild Dog', 'Lost Bandit'],
      loot: [
        { name: 'Gold', chance: 40 },
        { name: 'Iron', chance: 20 },
        { name: 'Leather', chance: 20 },
        { name: 'Stick', chance: 20 },
      ],
      x: 22,
      y: 68,
      difficulty: 1,
    },
    {
      id: 'foggy-woods',
      name: 'Foggy Woods',
      description: 'Thick fog, eerie silence, and hidden threats.',
      imageUrl: '/assets/locations/foggy-woods.jpg',
      monsters: ['Wolf', 'Wisp'],
      loot: [
        { name: 'Herbs', chance: 50 },
        { name: 'Leather', chance: 30 },
        { name: 'Rare Seed', chance: 20 },
      ],
      x: 58,
      y: 34,
      difficulty: 2,
    },
  ];
}
