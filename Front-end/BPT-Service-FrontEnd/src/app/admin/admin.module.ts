import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AuthenService } from 'src/app/core/services/authen.service';
import { UtilityService } from 'src/app/core/services/utility.service';
import { RouterModule } from '@angular/router';


import { TranslationService } from 'src/app/core/services/translation.service';
import { AdminComponent } from './admin.component';
import { adminRoutes } from './admin.routes';
import { ProviderComponent } from './provider/provider.component';



@NgModule({
  imports: [
    CommonModule,
    RouterModule.forChild(adminRoutes)
  ],
  declarations: [AdminComponent, ProviderComponent],
  providers:[UtilityService,AuthenService,TranslationService]
})
export class AdminModule { }
