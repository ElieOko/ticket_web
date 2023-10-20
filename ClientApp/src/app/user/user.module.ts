import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { UserRoutingModule } from './user-routing.module';
import { UserComponent } from './user.component';
import { UserListComponent } from './user-list/user-list.component';
import { ReactiveFormsModule } from '@angular/forms';
import { ComboBoxModule } from '@progress/kendo-angular-dropdowns';
import { UserFormComponent } from './user-form/user-form.component';
import { UserEditComponent } from './user-edit/user-edit.component';
import { ResetPasswordComponent } from './reset-password/reset-password.component';
 

@NgModule({
  declarations: [
    UserComponent,
    UserListComponent,
    UserFormComponent,
    UserEditComponent,
    ResetPasswordComponent
  ],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    ComboBoxModule,
    UserRoutingModule
  ]
})
export class UserModule { }
