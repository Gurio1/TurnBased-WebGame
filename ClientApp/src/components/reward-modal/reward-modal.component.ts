import { Component, Inject, Input } from '@angular/core';
import { Reward } from './models/reward';
import { CommonModule, NgFor, NgIf } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';
import { Item } from '../../core/models/item';
import { Equipment } from '../../core/models/equipment';
import { Router } from '@angular/router';

@Component({
  selector: 'app-reward-modal',
  standalone: true,
  imports: [NgIf, NgFor, CommonModule],
  templateUrl: './reward-modal.component.html',
  styleUrl: './reward-modal.component.scss',
})
export class RewardModalComponent {
  @Input() gold: number = 0;
  @Input() gems: number = 0;
  @Input() items: Equipment[] = [];

  constructor(private router: Router) {
    this.gold = 100;
    this.gems = 50;
    this.items = [
      {
        id: 'eq1',
        equipmentId: 'sword_iron_01',
        name: 'Iron Short Sword',
        slot: 'Weapon',
        itemType: 'Equipment',
        imageUrl: 'wooden-chest.png',
        interactions: 'equip',
        description: 'A sturdy short sword made of iron.',
        attributes: [
          { name: 'Attack', value: 12 },
          { name: 'Speed', value: 3 },
        ],
      },
      {
        id: 'eq1',
        equipmentId: 'sword_iron_01',
        name: 'Iron Short Sword',
        slot: 'Weapon',
        itemType: 'Equipment',
        imageUrl: 'wooden-chest.png',
        interactions: 'equip',
        description: 'A sturdy short sword made of iron.',
        attributes: [
          { name: 'Attack', value: 12 },
          { name: 'Speed', value: 3 },
        ],
      },
      {
        id: 'eq1',
        equipmentId: 'sword_iron_01',
        name: 'Iron Short Sword',
        slot: 'Weapon',
        itemType: 'Equipment',
        imageUrl: 'wooden-chest.png',
        interactions: 'equip',
        description: 'A sturdy short sword made of iron.',
        attributes: [
          { name: 'Attack', value: 12 },
          { name: 'Speed', value: 3 },
        ],
      },
      {
        id: 'eq1',
        equipmentId: 'sword_iron_01',
        name: 'Iron Short Sword',
        slot: 'Weapon',
        itemType: 'Equipment',
        imageUrl: 'wooden-chest.png',
        interactions: 'equip',
        description: 'A sturdy short sword made of iron.',
        attributes: [
          { name: 'Attack', value: 12 },
          { name: 'Speed', value: 3 },
        ],
      },
      {
        id: 'eq1',
        equipmentId: 'sword_iron_01',
        name: 'Iron Short Sword',
        slot: 'Weapon',
        itemType: 'Equipment',
        imageUrl: 'wooden-chest.png',
        interactions: 'equip',
        description: 'A sturdy short sword made of iron.',
        attributes: [
          { name: 'Attack', value: 12 },
          { name: 'Speed', value: 3 },
        ],
      },
      {
        id: 'eq1',
        equipmentId: 'sword_iron_01',
        name: 'Iron Short Sword',
        slot: 'Weapon',
        itemType: 'Equipment',
        imageUrl: 'wooden-chest.png',
        interactions: 'equip',
        description: 'A sturdy short sword made of iron.',
        attributes: [
          { name: 'Attack', value: 12 },
          { name: 'Speed', value: 3 },
        ],
      },
      {
        id: 'eq2',
        equipmentId: 'boots_leather_01',
        name: 'Leather Boots',
        slot: 'Boots',
        itemType: 'Equipment',
        imageUrl: 'wooden-chest.png',
        interactions: 'equip',
        description: 'Light boots made of cured leather.',
        attributes: [
          { name: 'Defense', value: 5 },
          { name: 'Agility', value: 2 },
        ],
      },
      {
        id: 'eq2',
        equipmentId: 'boots_leather_01',
        name: 'Leather Boots',
        slot: 'Boots',
        itemType: 'Equipment',
        imageUrl: 'wooden-chest.png',
        interactions: 'equip',
        description: 'Light boots made of cured leather.',
        attributes: [
          { name: 'Defense', value: 5 },
          { name: 'Agility', value: 2 },
        ],
      },
      {
        id: 'eq3',
        equipmentId: 'amulet_mystic_01',
        name: 'Mystic Amulet',
        slot: 'Neck',
        itemType: 'Equipment',
        imageUrl: 'wooden-chest.png',
        interactions: 'equip',
        description: 'An ancient amulet humming with arcane energy.',
        attributes: [
          { name: 'Magic Power', value: 10 },
          { name: 'Mana Regen', value: 4 },
        ],
      },
    ];
  }

  goHome() {
    this.router.navigate(['/home']);
  }
}
