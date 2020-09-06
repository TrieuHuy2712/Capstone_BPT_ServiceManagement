import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AuthenService } from 'src/app/core/services/authen.service';
import { UtilityService } from 'src/app/core/services/utility.service';
import { RouterModule } from '@angular/router';
import { SidebarMenuComponent } from 'src/app/shared/sidebar-menu/sidebar-menu.component';
import { TopMenuComponent } from 'src/app/shared/top-menu/top-menu.component';
import { TranslationService } from 'src/app/core/services/translation.service';
import { MyServiceComponent } from './my-service.component';
import { myServiceRoutes } from './my-service.routes';


@NgModule({
  imports: [
    CommonModule,
    RouterModule.forChild(myServiceRoutes)
  ],
  declarations: [MyServiceComponent],
  providers:[UtilityService,AuthenService,TranslationService]
})
export class MyServiceModule { }
