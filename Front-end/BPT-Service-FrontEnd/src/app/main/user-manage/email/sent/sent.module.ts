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
import { RatingModule } from 'ngx-bootstrap';
import { SentComponent } from './sent.component';

const itemRoutes: Routes = [
  //localhost:4200/main/user
  { path: '', redirectTo: 'index', pathMatch: 'full' },
  //localhost:4200/main/home/index
  { path: 'index', component: SentComponent }
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
    
  ],
  declarations: [SentComponent],
  providers:[DataService,NotificationService, TranslationService]
})
export class SentModule { }