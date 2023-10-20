import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { UserData, UserDto } from './user';

const users_baseUrl = "api/users";

const cloneData = (data: any[]) => data.map(item => Object.assign({}, item));

export interface CheckUsernameResponse {
  isTaken: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class UserService {

  public userLoading = false;
  public users: UserDto[] = [];
  
  constructor(private http: HttpClient) { }

  createUser(user) {
    return this.http.post(users_baseUrl, user)
  }

  isUsernameTaken(username: string) {
    const url = `${users_baseUrl}/checkusername`;
    return this.http.post<CheckUsernameResponse>(url, { username: username })
      .pipe(map(res => res && res.isTaken))
  }

  getUsers(searchTerm: string = null, pageNumber = 1, pageSize = 2) {
    let params = new HttpParams() 
      .set('pageNumber', String(pageNumber))
      .set('pageSize', String(pageSize))

    if (searchTerm) params = params.append("query", searchTerm)

    return this.http.get<UserData>(users_baseUrl, { params });
  }

  getUser(userId: number) {
    const url = `${users_baseUrl}/${userId}`;
    return this.http.get<UserDto>(url);
  }

  unLockUser(data) {
    const url = `${users_baseUrl}/unlock`;
    return this.http.put(url, data);
  }

  updateUser(user: UserDto) {
    const url = `${users_baseUrl}/${user.userId}`;
    return this.http.put(url, user);
  }

  deleteUser(userId: number) {
    const url = `${users_baseUrl}/${userId}`;
    return this.http.delete(url);
  }

  resetUserPassword(userId: number, data) {
    const url = `${users_baseUrl}/${userId}/resetpassword`;
    return this.http.post(url, data);
  }

  updateUsers(users) {
    const url = `${users_baseUrl}/update`;
    return this.http.put(url, users);
  }

  deleteUsers(users) {
    const url = `${users_baseUrl}/delete`;
    return this.http.post(url, users);
  }
}
