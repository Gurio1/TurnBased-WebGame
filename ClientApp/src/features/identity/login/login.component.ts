import { NgIf } from '@angular/common';
import { Component } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { passwordValidator } from '../shared/validators/password-validator';
import { LoginUser } from './models/login-user';
import { IdentityService } from '../services/identity.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [NgIf, RouterLink, ReactiveFormsModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss',
})
export class LoginComponent {
  constructor(
    private readonly identityService: IdentityService,
    private readonly router: Router
  ) {}

  loginForm = new FormGroup({
    email: new FormControl('', [Validators.required, Validators.email]),
    password: new FormControl('', [
      Validators.required,
      Validators.minLength(8),
      passwordValidator(),
    ]),
  });

  submitForm(): void {
    if (!this.loginForm.valid) {
      return;
    }

    const user: LoginUser = {
      email: this.loginForm.controls.email.value ?? '',
      password: this.loginForm.controls.password.value ?? '',
    };

    this.identityService.login(user).subscribe({
      next: () => this.router.navigate(['/home']),
      error: (err) => console.error('Observable emitted an error: ' + err),
    });
  }
}
