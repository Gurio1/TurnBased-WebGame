import { Equipment } from '../../../core/models/equipment';
import { Item } from '../../../core/models/item';
import { Stats } from '../../../core/models/stats';
import { AbilityHomeViewModel } from './abilityHomeViewModel';

export interface PlayerHomeViewModel {
  id: string;
  stats: Stats;
  abilities: AbilityHomeViewModel[];
  equipment: { [slot: string]: Equipment | null };
  inventoryEquipmentItems: Equipment[];
  otherInventoryItems: Item[];
  characterType: string;
}
