import { EquipmentBlueprintCountWeight } from './equipment-blueprint-count-weight';
import { EquipmentBlueprintStatRange } from './equipment-blueprint-stat-range';

export interface EquipmentBlueprintDetail {
  id: string;
  equipmentId: string;
  equipmentName: string;
  equipmentSlot: string;
  equipmentImageUrl: string;
  stats: EquipmentBlueprintStatRange[];
  countWeights: EquipmentBlueprintCountWeight[];
}
