import { Ability } from '../../../../core/models/ability';
import { Stats } from '../../../../core/models/stats';

export interface Monster {
  name: string;
  stats: Stats;
  abilities: Ability[];
}
