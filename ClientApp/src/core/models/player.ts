import { Ability } from './ability';
import { Equipment } from './equipment';
import { Item } from './item';

export interface Character {
  id: string;
  maxHealth: number;
  currentHealth: number;
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
