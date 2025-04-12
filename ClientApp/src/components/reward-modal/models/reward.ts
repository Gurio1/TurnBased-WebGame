import { Equipment } from '../../../core/models/equipment';
import { Item } from '../../../core/models/item';

export interface Reward {
  gold: number;
  experience: number;
  drop?: Item[] | null;
  equipmentDrop?: Equipment[] | null;
}
