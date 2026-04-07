export interface EquipmentBlueprintSaveRequest {
  equipmentId: string;
  stats: EquipmentBlueprintStatRangeRequest[];
  countWeights: EquipmentBlueprintCountWeightRequest[];
}

export interface EquipmentBlueprintStatRangeRequest {
  statKey: string;
  minValue: number;
  maxValue: number;
}

export interface EquipmentBlueprintCountWeightRequest {
  count: number;
  weight: number;
}
