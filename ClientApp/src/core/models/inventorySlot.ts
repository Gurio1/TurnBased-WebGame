import { Item } from './item';

export interface InventorySlot {
  item: Item;
  quantity: number;
}
