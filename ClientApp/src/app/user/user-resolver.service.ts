import { Injectable } from '@angular/core';
import { Resolve, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { UserDto } from './user';
import { UserService } from './user.service';
import { catchError } from 'rxjs/operators';
import { Observable, empty } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class UserResolverService implements Resolve<UserDto> {

  constructor(private userSvc: UserService) { }

  resolve(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<any> | Promise<any> | any {
    return this.userSvc.getUser(+route.paramMap.get('id')).pipe(
      catchError(() => {
        return empty()
      })
    )
  }

}
