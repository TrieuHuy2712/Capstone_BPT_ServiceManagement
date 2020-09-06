import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AuthenService } from 'src/app/core/services/authen.service';
import { UtilityService } from 'src/app/core/services/utility.service';
import { RouterModule } from '@angular/router';
import { TranslationService } from 'src/app/core/services/translation.service';
import { emailRoutes } from './email.routes';
import { EmailComponent } from './email.component';



@NgModule({
  imports: [
    CommonModule,
    RouterModule.forChild(emailRoutes)
  ],
  declarations: [EmailComponent],
  providers:[UtilityService,AuthenService,TranslationService]
})
export class EmailModule { }
