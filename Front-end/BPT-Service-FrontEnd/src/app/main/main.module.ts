import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MainComponent } from './main.component';
import { SidebarMenuComponent } from '../shared/sidebar-menu/sidebar-menu.component';
import { TopMenuComponent } from '../shared/top-menu/top-menu.component';
import { AuthenService } from 'src/app/core/services/authen.service';
import { UtilityService } from 'src/app/core/services/utility.service';
import { RouterModule } from '@angular/router';
import { mainRoutes } from './main.routes';
import { HomeModule } from './home/home.module';
import { TranslationService } from '../core/services/translation.service';
import { TopbarUserComponent } from '../shared/topbar-user/topbar-user.component';
import { UserModule } from './user/user.module';
@NgModule({
  imports: [
    CommonModule,
    UserModule,
    HomeModule,
    RouterModule.forChild(mainRoutes),

  ],
  declarations: [MainComponent, SidebarMenuComponent, TopMenuComponent, TopbarUserComponent],
  providers: [UtilityService, AuthenService, TranslationService]
})
export class MainModule { }
