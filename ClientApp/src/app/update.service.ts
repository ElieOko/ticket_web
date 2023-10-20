import { Injectable } from '@angular/core';
import { environment } from '.././environments/environment';

const appVersionKey = "App.Version";

@Injectable({
  providedIn: 'root'
})
export class UpdateService {

  currentAppVersion = environment.appVersion;

  constructor() { }


  checkForUpdates() {
    const previousVersion = this.storedAppVersion;

    if (previousVersion === null) {
      this.storedAppVersion = this.currentAppVersion;
      document.location.reload();
      return;
    }

    if (previousVersion !== this.currentAppVersion) {
      console.log("updating...");
      this.storedAppVersion = this.currentAppVersion;
      document.location.reload();
    }
  }

  get storedAppVersion() {
    return window.localStorage.getItem(appVersionKey);
  }

  set storedAppVersion(value) {
    window.localStorage.setItem(appVersionKey, value);
  }
}
