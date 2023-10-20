import { Component, OnInit } from '@angular/core';
import { Router, NavigationStart, RouterEvent, NavigationEnd } from '@angular/router';
import { UpdateService } from './update.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {

  loading: boolean;
  title = 'Soficom Q-Sys';

  constructor(private router: Router,
    private updateSvc: UpdateService) {
    this.loading = false;
    router.events.subscribe(
      (event: RouterEvent): void => {
        if (event instanceof NavigationStart) {
          this.loading = true;
        } else if (event instanceof NavigationEnd) {
          this.loading = false;
        }
      }
    );
  }

  ngOnInit() {
    this.updateSvc.checkForUpdates();
  }


}
