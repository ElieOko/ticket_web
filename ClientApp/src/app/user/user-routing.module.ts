import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AdminGuard } from '../core/admin.guard';
import { ResetPasswordComponent } from './reset-password/reset-password.component';
import { UserEditComponent } from './user-edit/user-edit.component';
import { UserFormComponent } from './user-form/user-form.component';
import { UserListComponent } from './user-list/user-list.component';
import { UserResolverService } from './user-resolver.service';
import { UserComponent } from './user.component';

const routes: Routes = [
  {
    path: '',
    component: UserComponent,
    canActivate: [AdminGuard],
    canActivateChild: [AdminGuard],
    children: [
      { path: 'create', component: UserFormComponent },
      {
        path: ':id/edit',
        component: UserEditComponent,
        resolve: {
          user: UserResolverService
        }
      },
      {
        path: ':id/reset-password',
        component: ResetPasswordComponent,
        resolve: {
          user: UserResolverService
        }
      },
      { path: '', component: UserListComponent },
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class UserRoutingModule { }
