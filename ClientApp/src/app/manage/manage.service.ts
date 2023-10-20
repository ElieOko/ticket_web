import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { IManageResponse } from './models';

const base_url = "api/manage";

@Injectable({
  providedIn: 'root'
})
export class ManageService {

  constructor(private http: HttpClient) { }

  getCurrentUserInfo() {
    const url = `${base_url}/userinfo`;
    return this.http.get<IManageResponse>(url);
  }

  changeUserPassword(payload) {
    const url = `${base_url}/changepassword`;
    return this.http.post(url, payload);
  }

  updateUserProfile(payload) {
    const url = `${base_url}/profile`;
    return this.http.post(url, payload);
  }


}
