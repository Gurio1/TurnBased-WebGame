import { CommonModule, NgFor, NgIf } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [CommonModule, NgIf, NgFor, RouterLink, RouterLinkActive],
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.scss',
})
export class NavbarComponent {
  @Input() isAuthenticated = false;
  @Input() userName?: string;

  @Output() loginClick = new EventEmitter<void>();
  @Output() registerClick = new EventEmitter<void>();
  @Output() logoutClick = new EventEmitter<void>();

  isMobileMenuOpen = false;

  readonly navItems = [
    { label: 'Home', route: '/home' },
    { label: 'Map', route: '/map' },
    { label: 'Battle', route: '/battle' },
  ] as const;

  toggleMobileMenu(): void {
    this.isMobileMenuOpen = !this.isMobileMenuOpen;
  }

  closeMobileMenu(): void {
    this.isMobileMenuOpen = false;
  }
}
