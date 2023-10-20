import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { Router } from '@angular/router';
import { IBranchModel } from './models/misc';

export interface IUserToken {
  userId: number;
  username: string;
  accessToken: string;
  branchId: number;
  isAdmin: boolean;
  canManage: boolean;
  branch: IBranchModel;
}

const USER_KEY = "auth-user";

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  private authState: BehaviorSubject<IUserToken | null>;
  public currentUserToken: Observable<IUserToken | null>;

  constructor(private http: HttpClient, private router: Router) {
    this.authState = new BehaviorSubject(JSON.parse(sessionStorage.getItem(USER_KEY)));
    this.currentUserToken = this.authState.asObservable();
  }

  public get currentUser(): IUserToken | null {
    return this.authState.value;
  }

  login(credentials) {
    return this.http.post<IUserToken>('auth/token', credentials)
      .pipe(map(res => {
        // login successful if there's a user in the response
        if (res) {
          // store user details in session storage to keep user logged in between page refreshes
          sessionStorage.setItem(USER_KEY, JSON.stringify(res));
          this.authState.next(res);
        }

        return res;
      }));
  }

  logout() {
    // remove user from session storage to log user out
    sessionStorage.removeItem(USER_KEY);
    this.authState.next(null);
    this.router.navigate(['/login']);
  }
}
