export interface MonsterDetail {
  name: string;
  overallDropChance: number;
  stats: MonsterStatValue[];
  abilityIds: string[];
  drops: MonsterDropEntry[];
}

export interface MonsterStatValue {
  key: string;
  name: string;
  valueType: string;
  value: number;
}

export interface MonsterDropEntry {
  itemTypeName: string;
  itemId: string;
  itemName: string;
  itemImageUrl: string;
  category: string;
  quantity: number;
  weight: number;
}
