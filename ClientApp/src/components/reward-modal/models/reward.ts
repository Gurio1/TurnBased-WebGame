import { DropItem } from "../models/drop-item";

export interface Reward {
  gold: number;
  experience: number;
  drop?: DropItem | null;
}
