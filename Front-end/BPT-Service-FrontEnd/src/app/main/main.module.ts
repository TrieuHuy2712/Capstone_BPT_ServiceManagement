
import { AuthenService } from 'src/app/core/services/authen.service';
import { CommonModule } from '@angular/common';
import { HomeModule } from './home/home.module';
import { LocationComponent } from './location/location.component';
import { MainComponent } from './main.component';
import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { SidebarMenuComponent } from '../shared/sidebar-menu/sidebar-menu.component';
import { TopMenuComponent } from '../shared/top-menu/top-menu.component';
import { TranslationService } from '../core/services/translation.service';
import { UserModule } from './user/user.module';
import { UtilityService } from 'src/app/core/services/utility.service';
import { mainRoutes } from './main.routes';

@NgModule({
  imports: [
    CommonModule,
    UserModule,
    HomeModule,
    RouterModule.forChild(mainRoutes)
  ],
  declarations: [MainComponent, SidebarMenuComponent, TopMenuComponent, LocationComponent],
  providers: [UtilityService, AuthenService, TranslationService]
})
export class MainModule { }
