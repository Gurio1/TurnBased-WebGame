import { Ability } from './ability';

export interface Character {
  id?: string; // Only Hero has an ID
  characterType: 'Hero' | 'Enemy';
  abilities: Ability[];
  equipment: any[]; // Define equipment type if available
  debuffs: any[]; // Define debuff type if available
  hp: number;
  armor: number;
  damage: number;
  debuffResistance: number;
  critChance: number;
  critDamage: number;
  dodgeChance: number;
}
