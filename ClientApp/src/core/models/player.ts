import { Ability } from './ability';

export interface Item {
  id: string;
  name: string;
  interactions: string;
}

export interface Equipment {
  id: string;
  name: string;
  type: string; // e.g., "weapon", "armor"
  stats: { [key: string]: number };
}

export interface Character {
  id: string;
  hp: number;
  armor: number;
  damage: number;
  debuffResistance: number;
  criticalChance: number;
  criticalDamage: number;
  dodgeChance: number;
  abilities: Ability[];
  equipment: { [slot: string]: Equipment | null };
  inventory: Item[];
  characterType: string;
}
