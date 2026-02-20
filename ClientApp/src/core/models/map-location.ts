export type LootEntry = { name: string; chance: number };

export type MapLocation = {
  id: string;
  name: string;
  description: string;
  imageUrl: string;
  monsters: string[];
  loot: LootEntry[];
  x: number;
  y: number;
  difficulty?: 1 | 2 | 3 | 4 | 5;
};
