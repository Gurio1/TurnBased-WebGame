import { EquipmentStat } from './equipment-attribute';
import { Item } from './item';

export interface Equipment extends Item {
  id: string;
  equipmentId: string;
  name: string;
  slot: string;
  attributes: EquipmentStat[];
}
