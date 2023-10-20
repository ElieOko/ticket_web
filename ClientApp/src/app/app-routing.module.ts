import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { LoginComponent } from './pages/login/login.component';
import { AppLayoutComponent } from './shared/app-layout/app-layout.component';
import { TicketListComponent } from './pages/ticket-list/ticket-list.component';
import { SimpleComponent } from './pages/simple/simple.component';
import { CallComponent } from './pages/call/call.component';
import { AuthGuard } from './core/auth.guard';
import { AdminGuard } from './core/admin.guard';
import { OpenTicketComponent } from './pages/open-ticket/open-ticket.component';

const routes: Routes = [
  {
    path: 'app',
    component: AppLayoutComponent,
    canActivate: [AuthGuard],
    canActivateChild: [AuthGuard],
    children: [
      { path: 'tickets', component: TicketListComponent },
      { path: 'simple',  component: SimpleComponent },
      { path: 'calls',   component: CallComponent },
      {
        path: 'users',
        loadChildren: () => import('./user/user.module').then(m => m.UserModule),
        canLoad: [AdminGuard]
      },
      { path: 'settings', loadChildren: () => import('./settings/settings.module').then(m => m.SettingsModule) },
      { path: 'open', component: OpenTicketComponent },
      { path: 'manage-account', loadChildren: () => import('./manage/manage.module').then(m => m.ManageModule) },
      { path: '', redirectTo: 'tickets', pathMatch: 'full' },
    ]
  },
  { path: 'login',  component: LoginComponent },
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  { path: '**', redirectTo: 'app', pathMatch:'full' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
