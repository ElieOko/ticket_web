import { UserService } from './user.service';
import { ValidatorFn, AbstractControl, ValidationErrors, FormControl} from '@angular/forms';
import { Observable, of } from 'rxjs';
import { catchError, map } from 'rxjs/operators';

export function uniqueUserName(userSvc: UserService) {
  return (control: FormControl): Observable<ValidationErrors | null> => {
    return userSvc.isUsernameTaken(control.value).pipe(
      map(isTaken => (isTaken ? { uniqueUsername: true } : null)),
      catchError(() => of(null))
    )
  }
}


export const passwordMatchValidator: ValidatorFn = (control: AbstractControl): ValidationErrors | null => {
  const password = control.get('password');
  const confirmPassword = control.get('confirmPassword');

  return password && confirmPassword && password.value !== confirmPassword.value ? { passwordMatch: true } : null;
};
