export interface DropItem {
  dropChance: number;
  countOfOneStack: number;
  name: string;
  value: number;
  stats: { [key: string]: string }; // Stats object with key-value pairs
}
