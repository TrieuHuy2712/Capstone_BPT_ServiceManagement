import { AdminRecommendationComponent } from './admin-recommendation.component';
import { CommonModule } from '@angular/common';
import { DataService } from '../../../core/services/data.service';
import { FormsModule } from '@angular/forms';
import { Ng4LoadingSpinnerModule } from 'ng4-loading-spinner';
import { NgModule, CUSTOM_ELEMENTS_SCHEMA, NO_ERRORS_SCHEMA } from '@angular/core';
import { NotificationService } from '../../../core/services/notification.service';
import { PaginationModule, ModalModule, TypeaheadModule } from 'ngx-bootstrap';
import { RouterModule, Routes } from '@angular/router';
import { SharedModule } from '../../../core/common/SharedModule';
import { TranslationService } from '../../../core/services/translation.service';

const roleRoutes: Routes = [
  // localhost:4200/main/user
  { path: '', redirectTo: 'index', pathMatch: 'full' },
  // localhost:4200/main/home/index
  { path: 'index', component: AdminRecommendationComponent }
]

@NgModule({
  declarations: [AdminRecommendationComponent],
  providers: [DataService, NotificationService, TranslationService
  ],
  imports: [
    CommonModule,
    PaginationModule,
    FormsModule,
    ModalModule.forRoot(),
    RouterModule.forChild(roleRoutes),
    SharedModule,
    Ng4LoadingSpinnerModule.forRoot(),
    TypeaheadModule.forRoot(),
  ],
  schemas: [
    CUSTOM_ELEMENTS_SCHEMA,
    NO_ERRORS_SCHEMA
  ]
})
export class AdminRecommendationModule { }
