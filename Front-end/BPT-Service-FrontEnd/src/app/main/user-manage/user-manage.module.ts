import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AuthenService } from 'src/app/core/services/authen.service';
import { UtilityService } from 'src/app/core/services/utility.service';
import { RouterModule } from '@angular/router';
import { UserManageComponent } from './user-manage.component';
import { userManageRoutes } from './user-manage.routes';
import { SidebarMenuComponent } from 'src/app/shared/sidebar-menu/sidebar-menu.component';
import { TopMenuComponent } from 'src/app/shared/top-menu/top-menu.component';
import { TranslationService } from 'src/app/core/services/translation.service';


@NgModule({
  imports: [
    CommonModule,
    RouterModule.forChild(userManageRoutes)
  ],
  declarations: [UserManageComponent],
  providers:[UtilityService,AuthenService,TranslationService]
})
export class UserManageModule { }
