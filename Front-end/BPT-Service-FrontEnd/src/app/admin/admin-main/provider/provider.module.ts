import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Routes, RouterModule } from '@angular/router';
import { ProviderComponent } from './provider.component';
import { PaginationModule } from 'ngx-bootstrap/pagination';
import { ModalModule } from 'ngx-bootstrap/modal';
import { FormsModule } from '@angular/forms';
import { TypeaheadModule } from 'ngx-bootstrap/typeahead';
import { EditorModule } from '@tinymce/tinymce-angular';
import { Ng4LoadingSpinnerModule } from 'ng4-loading-spinner';

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
    EditorModule,
    Ng4LoadingSpinnerModule.forRoot(),
    RouterModule.forChild(roleRoutes)
  ]
})
export class ProviderModule { }
