import { NgIf } from '@angular/common';
import { AfterViewInit, Component, ElementRef, ViewChild } from '@angular/core';
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
export class LoginComponent implements AfterViewInit {
  @ViewChild('loginFormElement')
  private readonly loginFormElement?: ElementRef<HTMLFormElement>;

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

  ngAfterViewInit(): void {
    queueMicrotask(() => this.syncDomValuesToForm());
  }

  submitForm(): void {
    this.syncDomValuesToForm();

    if (!this.loginForm.valid) {
      this.loginForm.markAllAsTouched();
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

  private syncDomValuesToForm(): void {
    const formElement = this.loginFormElement?.nativeElement;
    if (!formElement) {
      return;
    }

    const email = formElement.querySelector<HTMLInputElement>('#email')?.value ?? '';
    const password = formElement.querySelector<HTMLInputElement>('#password')?.value ?? '';

    this.loginForm.patchValue(
      {
        email,
        password,
      },
      { emitEvent: false }
    );

    this.loginForm.controls['email'].updateValueAndValidity({ emitEvent: false });
    this.loginForm.controls['password'].updateValueAndValidity({ emitEvent: false });
    this.loginForm.updateValueAndValidity({ emitEvent: false });
  }
}
