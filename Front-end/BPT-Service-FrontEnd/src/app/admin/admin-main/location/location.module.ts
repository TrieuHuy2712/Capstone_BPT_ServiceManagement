import { NgModule, CUSTOM_ELEMENTS_SCHEMA, NO_ERRORS_SCHEMA } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Routes, RouterModule } from '@angular/router';
import { LocationComponent } from './location.component';
import { DataService } from 'src/app/core/services/data.service';
import { NotificationService } from 'src/app/core/services/notification.service';
import { TranslationService } from 'src/app/core/services/translation.service';
import { PaginationModule, ModalModule } from 'ngx-bootstrap';
import { FormsModule } from '@angular/forms';
import { SharedModule } from 'src/app/core/common/SharedModule';
import { Ng4LoadingSpinnerModule } from 'ng4-loading-spinner';


const roleRoutes: Routes = [
  // localhost:4200/main/user
  { path: '', redirectTo: 'index', pathMatch: 'full' },
  // localhost:4200/main/home/index
  { path: 'index', component: LocationComponent }
]
@NgModule({
  declarations: [LocationComponent],
  providers: [DataService, NotificationService, TranslationService
  ],
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
  ]
})
export class LocationModule { }
