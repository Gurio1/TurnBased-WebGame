import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { IdentityService } from './services/identity.service';

export const authGuard: CanActivateFn = () => {
  const identityService = inject(IdentityService);
  const router = inject(Router);

  return identityService.getToken() ? true : router.createUrlTree(['/login']);
};
