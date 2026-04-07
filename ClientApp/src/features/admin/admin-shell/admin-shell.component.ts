import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-admin-shell',
  standalone: true,
  imports: [CommonModule, RouterLink, RouterLinkActive, RouterOutlet],
  templateUrl: './admin-shell.component.html',
  styleUrl: './admin-shell.component.scss',
})
export class AdminShellComponent {
  readonly navItems = [
    {
      label: 'Blueprints',
      route: '/admin/blueprints',
      description: 'Shape loot-ready equipment templates and their stat pools.',
    },
    {
      label: 'Monsters',
      route: '/admin/monsters',
      description: 'Tune encounters, assign abilities, and build weighted drop tables.',
    },
  ] as const;
}
