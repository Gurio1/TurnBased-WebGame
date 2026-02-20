import { NgIf } from '@angular/common';
import { Component } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { Router, RouterLink } from '@angular/router';
import { UniqueEmailValidator } from '../shared/validators/unique-email-validator';
import { IdentityService } from '../services/identity.service';
import { passwordValidator } from '../shared/validators/password-validator';
import { RegisterUser } from './models/register-user';
import { passwordMatchValidator } from './validators/password-match-validator';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [RouterLink, ReactiveFormsModule, NgIf, MatProgressBarModule],
  templateUrl: './register.component.html',
  styleUrl: './register.component.scss',
})
export class RegisterComponent {
  registerForm!: FormGroup;

  constructor(
    private readonly identityService: IdentityService,
    private readonly uniqueEmailValidator: UniqueEmailValidator,
    private readonly router: Router
  ) {
    this.registerForm = new FormGroup(
      {
        userName: new FormControl('', Validators.pattern(/^[a-zA-Z0-9]*$/)),
        email: new FormControl('', {
          asyncValidators: [this.uniqueEmailValidator.validate.bind(this.uniqueEmailValidator)],
          validators: [Validators.required, Validators.email],
          updateOn: 'blur',
        }),
        password: new FormControl('', [
          Validators.required,
          Validators.minLength(8),
          passwordValidator(),
        ]),
        confirmPassword: new FormControl('', Validators.required),
      },
      { validators: passwordMatchValidator() }
    );
  }

  submitForm(): void {
    if (!this.registerForm.valid) {
      return;
    }

    const formControls = this.registerForm.controls;
    const user: RegisterUser = {
      email: formControls['email'].value ?? '',
      password: formControls['password'].value ?? '',
      confirmedPassword: formControls['confirmPassword'].value ?? '',
    };

    this.identityService.registerUser(user).subscribe({
      next: () => this.router.navigate(['/battle']),
      error: (err) => console.error('Observable emitted an error: ' + err),
    });
  }
}
