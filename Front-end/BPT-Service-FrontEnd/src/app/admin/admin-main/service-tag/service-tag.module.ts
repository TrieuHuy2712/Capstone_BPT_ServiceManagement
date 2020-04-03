import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Routes, RouterModule } from '@angular/router';
import { PaginationModule  } from 'ngx-bootstrap/pagination';
import {FormsModule} from '@angular/forms';
import { ModalModule } from 'ngx-bootstrap/modal';
import { DataService } from 'src/app/core/services/data.service';
import { NotificationService } from 'src/app/core/services/notification.service';
import { ServiceTagComponent } from './service-tag.component';
import { LanguageService } from 'src/app/core/services/language.service';
import { TranslationService } from 'src/app/core/services/translation.service';
import { SharedModule } from 'src/app/core/common/SharedModule';

const roleRoutes: Routes = [
  //localhost:4200/main/user
  { path: '', redirectTo: 'index', pathMatch: 'full' },
  //localhost:4200/main/home/index
  { path: 'index', component: ServiceTagComponent }
]
@NgModule({
  declarations: [ServiceTagComponent],
  providers:[DataService,NotificationService, TranslationService
  ],
  imports: [
    CommonModule,
    PaginationModule,
    FormsModule,
    ModalModule.forRoot(),
    RouterModule.forChild(roleRoutes),
    SharedModule
  ]
})
export class ServiceTagModule { }
