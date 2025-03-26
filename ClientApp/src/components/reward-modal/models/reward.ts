import { Item } from '../../../core/models/item';

export interface Reward {
  gold: number;
  experience: number;
  drop?: Item | null;
}
