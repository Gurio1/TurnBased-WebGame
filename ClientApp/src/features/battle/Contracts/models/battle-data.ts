import { Player } from '../../../../core/models/player';
import { Monster } from './monster';

export interface BattleData {
  combatPlayer: Player;
  monster: Monster;
}
