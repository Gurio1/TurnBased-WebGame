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
  private readonly baseNavItems = [
    { label: 'Home', route: '/home' },
    { label: 'Map', route: '/map' },
    { label: 'Battle', route: '/battle' },
  ] as const;

  @Input()
  set isAuthenticated(value: boolean) {
    this._isAuthenticated = value;
    this.navItems = value
      ? [...this.baseNavItems, { label: 'Admin', route: '/admin/blueprints' }]
      : [...this.baseNavItems];
  }

  get isAuthenticated(): boolean {
    return this._isAuthenticated;
  }

  @Input() userName?: string;

  @Output() loginClick = new EventEmitter<void>();
  @Output() registerClick = new EventEmitter<void>();
  @Output() logoutClick = new EventEmitter<void>();

  private _isAuthenticated = false;
  isMobileMenuOpen = false;
  navItems: ReadonlyArray<{ label: string; route: string }> = [...this.baseNavItems];

  toggleMobileMenu(): void {
    this.isMobileMenuOpen = !this.isMobileMenuOpen;
  }

  closeMobileMenu(): void {
    this.isMobileMenuOpen = false;
  }
}
