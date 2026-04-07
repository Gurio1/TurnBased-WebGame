import { Equipment } from '../../../core/models/equipment';
import { Stats } from '../../../core/models/stats';

export interface PlayerHomeViewModel {
  id: string;
  stats: Stats;
  equipment: { [slot: string]: Equipment | null };
  inventory: any;
  characterType: string;
}
