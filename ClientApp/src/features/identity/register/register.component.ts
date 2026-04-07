import { NgIf } from '@angular/common';
import { AfterViewInit, Component, ElementRef, ViewChild } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { IdentityService } from '../services/identity.service';
import { passwordValidator } from '../shared/validators/password-validator';
import { RegisterUser } from './models/register-user';
import { passwordMatchValidator } from './validators/password-match-validator';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [RouterLink, ReactiveFormsModule, NgIf],
  templateUrl: './register.component.html',
  styleUrl: './register.component.scss',
})
export class RegisterComponent {
  @ViewChild('registerFormElement')
  private readonly registerFormElement?: ElementRef<HTMLFormElement>;

  registerForm!: FormGroup;

  constructor(
    private readonly identityService: IdentityService,
    private readonly router: Router
  ) {
    this.registerForm = new FormGroup(
      {
        userName: new FormControl('', Validators.pattern(/^[a-zA-Z0-9]*$/)),
        email: new FormControl('', [Validators.required, Validators.email]),
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

  ngAfterViewInit(): void {
    queueMicrotask(() => this.syncDomValuesToForm());
  }

  submitForm(): void {
    this.syncDomValuesToForm();

    if (!this.registerForm.valid) {
      this.registerForm.markAllAsTouched();
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

  private syncDomValuesToForm(): void {
    const formElement = this.registerFormElement?.nativeElement;
    if (!formElement) {
      return;
    }

    const email = formElement.querySelector<HTMLInputElement>('#email')?.value ?? '';
    const password = formElement.querySelector<HTMLInputElement>('#password')?.value ?? '';
    const confirmPassword =
      formElement.querySelector<HTMLInputElement>('#confirmPassword')?.value ?? '';

    this.registerForm.patchValue(
      {
        email,
        password,
        confirmPassword,
      },
      { emitEvent: false }
    );

    this.registerForm.controls['email'].updateValueAndValidity({ emitEvent: false });
    this.registerForm.controls['password'].updateValueAndValidity({ emitEvent: false });
    this.registerForm.controls['confirmPassword'].updateValueAndValidity({ emitEvent: false });
    this.registerForm.updateValueAndValidity({ emitEvent: false });
  }
}
