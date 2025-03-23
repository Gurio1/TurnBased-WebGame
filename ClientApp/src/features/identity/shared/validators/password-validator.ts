import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';

export function passwordValidator(): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    const value = control.value;
    const hasNumber = /[0-9]/.test(value);
    const hasSymbol = /[!@#$%^&*()_+{}\[\]:;<>,.?/~\-]/.test(value);

    const valid = hasNumber && hasSymbol;

    return valid ? null : { invalidPassword: true };
  };
}
