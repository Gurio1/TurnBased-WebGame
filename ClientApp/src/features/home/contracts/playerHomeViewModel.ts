import { Equipment } from '../../../core/models/equipment';
import { Stats } from '../../../core/models/stats';
import { AbilityHomeViewModel } from './abilityHomeViewModel';

export interface PlayerHomeViewModel {
  id: string;
  stats: Stats;
  abilities: AbilityHomeViewModel[];
  equipment: { [slot: string]: Equipment | null };
  inventory: any;
  characterType: string;
}
