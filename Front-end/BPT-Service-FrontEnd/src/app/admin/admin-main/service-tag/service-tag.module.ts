import { CommonModule } from '@angular/common';
import { DataService } from 'src/app/core/services/data.service';
import { ModalModule } from 'ngx-bootstrap/modal';
import { NgModule, NO_ERRORS_SCHEMA, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { NotificationService } from 'src/app/core/services/notification.service';
import { PaginationModule } from 'ngx-bootstrap/pagination';
import { Routes, RouterModule } from '@angular/router';
import { ServiceTagComponent } from './service-tag.component';
import { SharedModule } from 'src/app/core/common/SharedModule';
import { TranslationService } from 'src/app/core/services/translation.service';
import { FormsModule } from '@angular/forms';
import { Ng4LoadingSpinnerModule } from 'ng4-loading-spinner';

const roleRoutes: Routes = [
  //localhost:4200/main/user
  { path: '', redirectTo: 'index', pathMatch: 'full' },
  //localhost:4200/main/home/index
  { path: 'index', component: ServiceTagComponent }
]
@NgModule({
  declarations: [ServiceTagComponent],

  imports: [
    CommonModule,
    PaginationModule,
    FormsModule,
    ModalModule.forRoot(),
    RouterModule.forChild(roleRoutes),
    SharedModule,
    Ng4LoadingSpinnerModule.forRoot()
  ],
  schemas: [
    CUSTOM_ELEMENTS_SCHEMA,
    NO_ERRORS_SCHEMA
  ],
  providers: [DataService, NotificationService, TranslationService
  ],
  
})
export class ServiceTagModule { }
