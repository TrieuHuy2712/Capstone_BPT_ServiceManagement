import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Routes, RouterModule } from '@angular/router';
import { ProviderComponent } from './provider.component';
import { PaginationModule, ModalModule } from 'ngx-bootstrap';
import { FormsModule } from '@angular/forms';
import { TypeaheadModule } from 'ngx-bootstrap/typeahead';

const roleRoutes: Routes = [
  //localhost:4200/main/user
  { path: '', redirectTo: 'index', pathMatch: 'full' },
  //localhost:4200/main/home/index
  { path: 'index', component: ProviderComponent }
]

@NgModule({
  declarations: [ProviderComponent],
  imports: [
    CommonModule,
    PaginationModule,
    FormsModule,
    ModalModule.forRoot(),
    TypeaheadModule.forRoot(),
    RouterModule.forChild(roleRoutes)
  ]
})
export class ProviderModule { }
