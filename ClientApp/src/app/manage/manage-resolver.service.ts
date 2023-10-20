import { Injectable } from '@angular/core';
import { Resolve, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { catchError } from 'rxjs/operators';
import { Observable, EMPTY } from 'rxjs';
import { ManageService } from './manage.service';
import { IManageResponse } from './models';

@Injectable({
  providedIn: 'root'
})
export class ManageResolverService implements Resolve<IManageResponse> {

  constructor(private manageSvc: ManageService) { }
  resolve(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<any> | Promise<any> | any {
    return this.manageSvc.getCurrentUserInfo().pipe(
      catchError(() => {
        return EMPTY
      })
    )
  }
}
