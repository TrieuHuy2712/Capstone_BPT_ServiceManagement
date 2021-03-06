import { AdminComponent } from './admin.component';
import { AuthenService } from 'src/app/core/services/authen.service';
import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { TranslationService } from 'src/app/core/services/translation.service';
import { UtilityService } from 'src/app/core/services/utility.service';
import { adminRoutes } from './admin.routes';



@NgModule({
  imports: [
    CommonModule,
    RouterModule.forChild(adminRoutes)
  ],
  declarations: [AdminComponent],
  providers: [UtilityService, AuthenService, TranslationService]
})
export class AdminModule { }
