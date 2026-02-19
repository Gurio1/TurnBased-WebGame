import { Ability } from './ability';
import { Equipment } from './equipment';
import { InventorySlot } from './inventorySlot';
import { Item } from './item';
import { Stats } from './stats';

export interface Player {
  id: string;
  stats: Stats;
  abilities: Ability[];
  equipment: { [slot: string]: Equipment | null };
  inventory: any;
  characterType: string;
}
