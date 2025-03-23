import { Injectable } from '@angular/core';
import {
  AsyncValidator,
  AbstractControl,
  ValidationErrors,
} from '@angular/forms';
import { Observable, map, catchError, of } from 'rxjs';
import { IdentityService } from '../../services/identity.service';

@Injectable({ providedIn: 'root' })
export class UniqueEmailValidator implements AsyncValidator {
  constructor(private identityService: IdentityService) {}

  validate(control: AbstractControl): Observable<ValidationErrors | null> {
    return this.identityService.isEmailTaken(control.value).pipe(
      map((isUnique) => (isUnique ? { uniqueEmail: true } : null)),

      //TO DO: Handle error
      catchError(() => of({ uniqueEmail: true }))
    );
  }
}
