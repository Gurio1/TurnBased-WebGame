import { Component } from '@angular/core';
import {
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { IdentityService } from '../services/identity.service';
import { Router, RouterLink } from '@angular/router';
import { loginUser } from './models/login-user';
import { passwordValidator } from '../shared/validators/password-validator';
import { NgIf } from '@angular/common';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [NgIf, RouterLink, ReactiveFormsModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss',
})
export class LoginComponent {
  constructor(
    private identityService: IdentityService,
    private router: Router
  ) {}

  loginForm = new FormGroup({
    email: new FormControl('', [Validators.required, Validators.email]),
    password: new FormControl('', [
      Validators.required,
      Validators.minLength(8),
      passwordValidator(),
    ]),
  });

  submitForm() {
    if (this.loginForm.valid) {
      let formControls = this.loginForm.controls;
      let user = new loginUser(
        this.loginForm.controls.email.value!,
        this.loginForm.controls.password.value!
      );

      var result = this.identityService.login(user);

      result.subscribe({
        next: (value) => this.router.navigate(['/home']),
        error: (err) => console.error('Observable emitted an error: ' + err),
      });
    }
  }
}
