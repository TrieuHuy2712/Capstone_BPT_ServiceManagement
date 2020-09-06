import { CommonModule } from '@angular/common';
import { DataService } from '../../../core/services/data.service';
import { FormsModule } from '@angular/forms';
import { Ng4LoadingSpinnerModule } from 'ng4-loading-spinner';
import { NgModule, CUSTOM_ELEMENTS_SCHEMA, NO_ERRORS_SCHEMA } from '@angular/core';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { NotificationService } from '../../../core/services/notification.service';
import { PaginationModule } from 'ngx-bootstrap/pagination';
import { ModalModule } from 'ngx-bootstrap/modal';
import { RoleComponent } from './role.component';
import { Routes, RouterModule } from '@angular/router';
import { SharedModule } from '../../../core/common/SharedModule';
import { TranslationService } from '../../../core/services/translation.service';


const roleRoutes: Routes = [
  //localhost:4200/main/user
  { path: '', redirectTo: 'index', pathMatch: 'full' },
  //localhost:4200/main/home/index
  { path: 'index', component: RoleComponent }
]
@NgModule({
  imports: [
    CommonModule,
    PaginationModule,
    FormsModule,
    ModalModule.forRoot(),
    RouterModule.forChild(roleRoutes),
    SharedModule,
    NgbModule,
    Ng4LoadingSpinnerModule.forRoot()
  ],
  declarations: [RoleComponent],
  schemas: [
    CUSTOM_ELEMENTS_SCHEMA,
    NO_ERRORS_SCHEMA
  ],
  providers: [DataService, NotificationService, TranslationService]
})
export class RoleModule { }