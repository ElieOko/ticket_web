import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { ManageComponent } from './manage.component';
import { ManageResolverService } from './manage-resolver.service';

const routes: Routes = [
  {
    path: '',
    component: ManageComponent,
    resolve: {
      user: ManageResolverService
    }
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ManageRoutingModule { }
