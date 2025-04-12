import { Ability } from './ability';
import { Equipment } from './equipment';
import { Item } from './item';
import { Stats } from './stats';

export interface Player {
  id: string;
  stats: Stats;
  abilities: Ability[];
  equipment: { [slot: string]: Equipment | null };
  inventoryEquipmentItems: Equipment[];
  otherInventoryItems: Item[];
  characterType: string;
}
