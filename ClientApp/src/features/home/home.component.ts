import { Component, HostListener, OnInit } from '@angular/core';
import { Character, Item } from '../../core/models/player';
import { CharacterService } from '../../shared/character.service';
import { CommonModule } from '@angular/common';
import { Observable } from 'rxjs';
@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss',
})
export class HomeComponent implements OnInit {
  character!: Character;
  leftEquipmentSlots: string[] = ['Helmet', 'Armor', 'Gloves'];
  rightEquipmentSlots: string[] = ['Weapon', 'Shield', 'Boots'];
  contextMenuVisible = false;
  contextMenuX = 0;
  contextMenuY = 0;
  selectedItem: Item | null = null;

  ngOnInit() {
    this.characterService.getPlayer().subscribe((char) => {
      if (!this.character) {
        console.log('initialize');
        this.character = { ...char }; // First initialization
      } else {
        console.log('update');
        Object.assign(this.character, char); // Update properties without replacing object
      }
    });
  }

  constructor(private characterService: CharacterService) {}

  openContextMenu(event: MouseEvent, item: Item) {
    event.preventDefault(); // Prevent default right-click menu
    this.contextMenuVisible = true;
    this.contextMenuX = event.clientX;
    this.contextMenuY = event.clientY;
    this.selectedItem = item;
  }

  performAction(action: string, itemId: string) {
    console.log(`Performing: ${action} on ${this.selectedItem?.name}`);
    this.characterService.makeAction(action, itemId).subscribe({
      next: (char) => Object.assign(this.character, char),
      error: (e) => console.error(e),
    });

    this.contextMenuVisible = false; // Hide menu after selection
  }

  @HostListener('document:click')
  closeContextMenu() {
    this.contextMenuVisible = false;
  }
}
