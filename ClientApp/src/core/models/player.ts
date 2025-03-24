import { Ability } from './ability';

export interface Item {
  id: string;
  itemType: string;
  name: string;
  interactions: string;
  description: string;
}

export interface EquipmentAttribute {
  name: string;
  value: number;
}

export interface Equipment extends Item {
  id: string;
  name: string;
  slot: string;
  attributes: EquipmentAttribute[];
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
  inventoryEquipmentItems: Equipment[];
  otherInventoryItems: Item[];
  characterType: string;
}
