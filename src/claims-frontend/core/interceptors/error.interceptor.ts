import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';
import { AuthService } from '../auth/auth.service';
import { NotificationService } from '../services/notification.service';
import { ProblemDetails } from '../../shared/models/problem-details.model';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const notification = inject(NotificationService);
  const authService = inject(AuthService);
  const router = inject(Router);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      const problem = error.error as ProblemDetails | undefined;

      switch (error.status) {
        case 401:
          authService.logout();
          router.navigate(['/login']);
          notification.error('Your session has expired. Please log in again.');
          break;

        case 400:
          notification.error(formatValidationErrors(problem));
          break;

        case 403:
          notification.error('You do not have permission to perform this action.');
          break;

        case 404:
          notification.error(problem?.detail ?? 'The requested resource was not found.');
          break;

        case 409:
          notification.error(
            problem?.detail ?? 'This record was modified by another user. Please refresh and try again.'
          );
          break;

        case 422:
          notification.error(problem?.detail ?? 'This action violates a business rule.');
          break;

        default:
          notification.error(problem?.detail ?? 'An unexpected error occurred. Please try again.');
      }

      return throwError(() => error);
    })
  );
};

function formatValidationErrors(problem: ProblemDetails | undefined): string {
  if (!problem?.errors) {
    return problem?.detail ?? 'Validation failed.';
  }

  const messages = Object.values(problem.errors).flat();
  return messages.length > 0 ? messages.join(' ') : 'Validation failed.';
}
