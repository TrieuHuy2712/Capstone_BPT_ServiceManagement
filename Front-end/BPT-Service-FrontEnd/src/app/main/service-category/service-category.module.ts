import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ServiceCategoryComponent } from './service-category.component';
import { Routes, RouterModule } from '@angular/router';
import { PaginationModule  } from 'ngx-bootstrap/pagination';
import {FormsModule} from '@angular/forms';
import { ModalModule } from 'ngx-bootstrap/modal';
import { DataService } from 'src/app/core/services/data.service';
import { NotificationService } from 'src/app/core/services/notification.service';

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
    RouterModule.forChild(roleRoutes)
  ]
})
export class ServiceCategoryModule { }
