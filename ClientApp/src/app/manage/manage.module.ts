import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { ManageRoutingModule } from './manage-routing.module';
import { ManageComponent } from './manage.component';
import { ReactiveFormsModule } from '@angular/forms';


@NgModule({
  declarations: [ManageComponent],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    ManageRoutingModule
  ]
})
export class ManageModule { }
