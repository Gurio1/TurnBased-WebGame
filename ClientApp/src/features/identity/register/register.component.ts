import { Component } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import {
  FormGroup,
  FormControl,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { IdentityService } from '../services/identity.service';
import { NgIf } from '@angular/common';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { UniqueEmailValidator } from '../shared/validators/unique-email-validator';
import { passwordValidator } from '../shared/validators/password-validator';
import { passwordMatchValidator } from './validators/password-match-validator';
import { registerUser } from './models/register-user';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [RouterLink, ReactiveFormsModule, NgIf, MatProgressBarModule],
  templateUrl: './register.component.html',
  styleUrl: './register.component.scss',
})
export class RegisterComponent {
  email: string = '';
  registerForm!: FormGroup;

  constructor(
    private identityService: IdentityService,
    private uniqueEmailValidator: UniqueEmailValidator,
    private router: Router
  ) {
    this.registerForm = new FormGroup(
      {
        userName: new FormControl('', Validators.pattern(/^[a-zA-Z0-9]*$/)),
        email: new FormControl('', {
          asyncValidators: [
            this.uniqueEmailValidator.validate.bind(this.uniqueEmailValidator),
          ],
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

  submitForm() {
    if (this.registerForm.valid) {
      let formControls = this.registerForm.controls;

      let user = new registerUser(
        formControls['email'].value!,
        formControls['password'].value!,
        formControls['confirmPassword'].value!
      );

      var result = this.identityService.registerUser(user);

      result.subscribe({
        next: (value) => this.router.navigate(['/battle']),
        error: (err) => console.error('Observable emitted an error: ' + err),
      });
    }
  }
}
