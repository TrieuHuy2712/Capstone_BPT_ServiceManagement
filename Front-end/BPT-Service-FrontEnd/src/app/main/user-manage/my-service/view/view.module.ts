import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Routes, RouterModule } from '@angular/router';
import { PaginationModule  } from 'ngx-bootstrap/pagination';
import {FormsModule} from '@angular/forms';
import { ModalModule } from 'ngx-bootstrap/modal';
import { DataService } from 'src/app/core/services/data.service';
import { NotificationService } from 'src/app/core/services/notification.service';
import { TranslationService } from 'src/app/core/services/translation.service';
import { SharedModule } from 'src/app/core/common/SharedModule';
import { SlickModule } from 'ngx-slick';
import { ViewComponent } from './view.component';
import { TypeaheadModule } from 'ngx-bootstrap/typeahead';
import { EditorModule } from '@tinymce/tinymce-angular';
import { Ng4LoadingSpinnerModule } from 'ng4-loading-spinner';

const itemRoutes: Routes = [
  //localhost:4200/main/user
  { path: '', redirectTo: 'index', pathMatch: 'full' },
  //localhost:4200/main/home/index
  { path: 'index', component: ViewComponent }
]
@NgModule({
  imports: [
    CommonModule,
    PaginationModule,
    FormsModule,
    ModalModule.forRoot(),
    RouterModule.forChild(itemRoutes),
    SharedModule,
    SlickModule.forRoot(),
    TypeaheadModule.forRoot(),
    EditorModule,
    Ng4LoadingSpinnerModule.forRoot()


  ],
  declarations: [ViewComponent],
  providers:[DataService,NotificationService, TranslationService]
})
export class ViewModule { }