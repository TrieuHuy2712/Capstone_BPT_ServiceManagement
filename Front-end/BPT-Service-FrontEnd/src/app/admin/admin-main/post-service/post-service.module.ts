import { NgModule, NO_ERRORS_SCHEMA, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PostServiceComponent } from './post-service.component';
import { FormsModule } from '@angular/forms';
import { TypeaheadModule } from 'ngx-bootstrap/typeahead';
import { PaginationModule } from 'ngx-bootstrap/pagination';
import { ModalModule } from 'ngx-bootstrap/modal';
import { RouterModule, Routes } from '@angular/router';
import { EditorModule } from '@tinymce/tinymce-angular';
import { Ng4LoadingSpinnerModule } from 'ng4-loading-spinner';
import { UploadService } from '../../../core/services/upload.service';

const roleRoutes: Routes = [
  //localhost:4200/main/user
  { path: '', redirectTo: 'index', pathMatch: 'full' },
  //localhost:4200/main/home/index
  { path: 'index', component: PostServiceComponent }
]


@NgModule({
  declarations: [PostServiceComponent],
  imports: [
    CommonModule,
    FormsModule,
    ModalModule.forRoot(),
    PaginationModule,
    RouterModule.forChild(roleRoutes),
    TypeaheadModule.forRoot(),
    EditorModule,
    Ng4LoadingSpinnerModule.forRoot()
  ],
  providers:[
    UploadService
  ],
  schemas: [
    CUSTOM_ELEMENTS_SCHEMA,
    NO_ERRORS_SCHEMA
  ]

})
export class PostServiceModule { }
