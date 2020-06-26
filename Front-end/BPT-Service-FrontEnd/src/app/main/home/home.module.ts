import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HomeComponent } from './home.component';
import { Routes, RouterModule } from '@angular/router';
import { SlickModule } from 'ngx-slick';
import { ModalModule, PaginationModule, TypeaheadModule } from 'ngx-bootstrap';
import { FormsModule } from '@angular/forms';
import { SharedModule } from 'src/app/core/common/SharedModule';
import { EditorModule } from '@tinymce/tinymce-angular';

const homeRoutes: Routes=[
  {path:'', redirectTo:'index',pathMatch:'full'},
  {path:'index', component: HomeComponent}
]

@NgModule({
  declarations: [HomeComponent],
  imports: [
    CommonModule,
    PaginationModule,
    FormsModule,
    ModalModule.forRoot(),
    RouterModule.forChild(homeRoutes),
    SharedModule,
    SlickModule.forRoot(),
    TypeaheadModule.forRoot(),
    EditorModule,

  ]
})
export class HomeModule { }