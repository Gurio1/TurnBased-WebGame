import { BattleData } from '../../models/battle-data';

export interface BattleResponse {
  type: number;
  target: string;
  arguments: BattleData;
}
