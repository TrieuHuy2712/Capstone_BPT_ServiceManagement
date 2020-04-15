import { NgModule, CUSTOM_ELEMENTS_SCHEMA, NO_ERRORS_SCHEMA } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AdminEmailComponent } from './admin-email.component';
import { Routes, RouterModule } from '@angular/router';
import { PaginationModule, ModalModule, TypeaheadModule } from 'ngx-bootstrap';
import { FormsModule } from '@angular/forms';
import { SharedModule } from '../../../core/common/SharedModule';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { Ng4LoadingSpinnerModule } from 'ng4-loading-spinner';
import { DataService } from '../../../core/services/data.service';
import { NotificationService } from '../../../core/services/notification.service';
import { TranslationService } from '../../../core/services/translation.service';

const roleRoutes: Routes = [
  //localhost:4200/main/user
  { path: '', redirectTo: 'index', pathMatch: 'full' },
  //localhost:4200/main/home/index
  { path: 'index', component: AdminEmailComponent }
]

@NgModule({
  declarations: [DataService, NotificationService, TranslationService],
  imports: [
    CommonModule,
    PaginationModule,
    FormsModule,
    ModalModule.forRoot(),
    RouterModule.forChild(roleRoutes),
    SharedModule,
    NgbModule,
    Ng4LoadingSpinnerModule.forRoot(),
    TypeaheadModule.forRoot(),
  ],
  schemas: [
    CUSTOM_ELEMENTS_SCHEMA,
    NO_ERRORS_SCHEMA
  ],
})
export class AdminEmailModule { }
