import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';

export function passwordMatchValidator(): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    const isMatch =
      control.get('password')?.value === control.get('confirmPassword')?.value;
    return isMatch ? null : { passwordMismatch: true };
  };
}
