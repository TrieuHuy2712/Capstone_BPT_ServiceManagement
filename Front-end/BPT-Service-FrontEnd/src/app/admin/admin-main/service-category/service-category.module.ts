import { NgModule, NO_ERRORS_SCHEMA, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ServiceCategoryComponent } from './service-category.component';
import { Routes, RouterModule } from '@angular/router';
import { PaginationModule  } from 'ngx-bootstrap/pagination';
import {FormsModule} from '@angular/forms';
import { ModalModule } from 'ngx-bootstrap/modal';
import { DataService } from 'src/app/core/services/data.service';
import { NotificationService } from 'src/app/core/services/notification.service';
import { Ng4LoadingSpinnerModule } from 'ng4-loading-spinner';

const roleRoutes: Routes = [
  //localhost:4200/main/user
  { path: '', redirectTo: 'index', pathMatch: 'full' },
  //localhost:4200/main/home/index
  { path: 'index', component: ServiceCategoryComponent }
]
@NgModule({
  declarations: [ServiceCategoryComponent],
  providers:[DataService,NotificationService],
  imports: [
    CommonModule,
    PaginationModule,
    FormsModule,
    ModalModule.forRoot(),
    Ng4LoadingSpinnerModule.forRoot(),
    RouterModule.forChild(roleRoutes)
  ],
  schemas: [
    CUSTOM_ELEMENTS_SCHEMA,
    NO_ERRORS_SCHEMA
  ]
})
export class ServiceCategoryModule { }
