import { AdminHomeModule } from './admin-home/admin-home.module';
import { AdminUserModule } from './admin-user/admin-user.module';
import { AuthenService } from 'src/app/core/services/authen.service';
import { CommonModule } from '@angular/common';
import { MainComponent } from './admin-main.component';
import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { SidebarMenuComponent } from '../shared/sidebar-menu/sidebar-menu.component';
import { TopMenuComponent } from '../shared/top-menu/top-menu.component';
import { TranslationService } from 'src/app/core/services/translation.service';
import { UtilityService } from 'src/app/core/services/utility.service';
import { mainRoutes } from './admin-main.routes';

@NgModule({
  imports: [
    CommonModule,
    AdminUserModule,
    AdminHomeModule,
    RouterModule.forChild(mainRoutes)
  ],
  declarations: [MainComponent, SidebarMenuComponent, TopMenuComponent],
  providers: [UtilityService, AuthenService, TranslationService]
})
export class AdminMainModule { }
