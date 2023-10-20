import { Component, OnInit } from '@angular/core';
import { AuthService } from '../../core/auth.service';

@Component({
  selector: 'app-app-layout',
  templateUrl: './app-layout.component.html',
  styleUrls: ['./app-layout.component.scss']
})
export class AppLayoutComponent implements OnInit {

  pages = [];

  constructor(public auth: AuthService) { }

  ngOnInit(): void {
    if (this.auth.currentUser.isAdmin) {
      this.pages = [
        { linkName: 'Accueil', url: 'tickets' },
        { linkName: 'Simple', url: 'simple' },
        { linkName: 'Appels', url: 'calls' },
        { linkName: 'Utilisateurs', url: 'users' },
        { linkName: 'Paramètres', url: 'settings' },
        { linkName: 'Ouvrir ticket', url: 'open' }
      ];
    } else {
      this.pages = [
        { linkName: 'Accueil', url: 'tickets' },
        { linkName: 'Simple', url: 'simple' },
        { linkName: 'Appels', url: 'calls' },
        { linkName: 'Paramètres', url: 'settings' },
        { linkName: 'Ouvrir ticket', url: 'open' }
      ];
    }
  }

  logout() {
    this.auth.logout();
  }

}
