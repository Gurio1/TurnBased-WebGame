import { Component } from '@angular/core';
import { MapLocation } from '../../../core/models/map-location';
import { CommonModule } from '@angular/common';
import { WorldMapComponent } from '../components/world-map/world-map.component';
import { LocationService } from '../services/location.service';
import {
  RewardItemVisual,
  RewardModalComponent,
} from '../../../components/reward-modal/reward-modal.component';

@Component({
  selector: 'app-map-page',
  standalone: true,
  imports: [CommonModule, WorldMapComponent, RewardModalComponent],
  templateUrl: './map-page.component.html',
  styleUrl: './map-page.component.scss',
})
export class MapPageComponent {
  map = '/map.png';
  isExploring = false;
  exploreStatusMessage: string | null = null;
  exploreErrorMessage: string | null = null;
  exploreReward: { subtitle: string | null; items: RewardItemVisual[] } | null = null;

  constructor(private readonly locationService: LocationService) {}

  locations: MapLocation[] = [
    {
      id: 'newcomers-village',
      name: "Newcomers' Village",
      description: 'A quiet place where weak monsters roam and basic materials can be gathered.',
      imageUrl: '/background.png',
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
      imageUrl: '/logs.png',
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

  onExplore(location: MapLocation): void {
    if (this.isExploring) {
      return;
    }

    this.isExploring = true;
    this.exploreErrorMessage = null;
    this.exploreStatusMessage = null;

    this.locationService.explore(location.id).subscribe({
      next: (response) => {
        this.exploreStatusMessage = null;
        const items: RewardItemVisual[] =
          response.itemName && response.itemImageUrl
            ? [
                {
                  name: response.itemName,
                  imageUrl: response.itemImageUrl,
                  quantity: response.quantity,
                },
              ]
            : [];

        this.exploreReward = {
          subtitle: null,
          items,
        };
      },
      error: (err: Error) => {
        this.exploreErrorMessage = err.message;
      },
      complete: () => {
        this.isExploring = false;
      },
    });
  }

  closeExploreReward(): void {
    this.exploreReward = null;
  }
}
