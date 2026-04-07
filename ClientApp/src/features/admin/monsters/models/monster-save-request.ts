export interface MonsterSaveRequest {
  name: string;
  overallDropChance: number;
  stats: MonsterStatValueRequest[];
  drops: MonsterDropEntryRequest[];
  abilityIds: string[];
}

export interface MonsterStatValueRequest {
  key: string;
  value: number;
}

export interface MonsterDropEntryRequest {
  itemTypeName: string;
  itemId: string;
  quantity: number;
  weight: number;
}
