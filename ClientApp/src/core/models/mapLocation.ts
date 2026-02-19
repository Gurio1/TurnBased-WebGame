export type LootEntry = { name: string; chance: number };

export type MapLocation = {
  id: string;
  name: string;
  description: string;
  imageUrl: string; // location preview image
  monsters: string[];
  loot: LootEntry[];
  // Marker position on the map container (percentages)
  x: number; // 0..100
  y: number; // 0..100
  difficulty?: 1 | 2 | 3 | 4 | 5;
};
