import { Component } from '@angular/core';
import { NgIf } from '@angular/common';
import { Router, RouterOutlet } from '@angular/router';
import { IdentityService } from '../features/identity/services/identity.service';
import { NavbarComponent } from '../components/navbar/navbar.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, NavbarComponent, NgIf],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss',
})
export class AppComponent {
  private readonly authRoutes = ['/login', '/register'];

  get isAuthenticated(): boolean {
    return !!this.identityService.getToken();
  }

  get showNavbar(): boolean {
    return !this.authRoutes.some((route) => this.router.url.startsWith(route));
  }

  readonly userName = 'Adventurer';

  constructor(
    private readonly identityService: IdentityService,
    private readonly router: Router
  ) {}

  logout(): void {
    this.identityService.logout();
    void this.router.navigate(['/login']);
  }

  onLoginClick(): void {
    void this.router.navigate(['/login']);
  }

  onRegisterClick(): void {
    void this.router.navigate(['/register']);
  }
}
