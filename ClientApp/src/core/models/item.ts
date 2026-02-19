import { ItemAction } from './itemAction';
export interface Item {
  id: string;
  name: string;
  imageUrl: string;
  type: string;
  itemActions: ItemAction[];
}
