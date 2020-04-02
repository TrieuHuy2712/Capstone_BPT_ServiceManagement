import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MainComponent } from './admin-main.component';
import { SidebarMenuComponent } from '../shared/sidebar-menu/sidebar-menu.component';
import { TopMenuComponent } from '../shared/top-menu/top-menu.component';
import { AuthenService } from 'src/app/core/services/authen.service';
import { UtilityService } from 'src/app/core/services/utility.service';
import { RouterModule } from '@angular/router';
import { mainRoutes } from './admin-main.routes';
import { HomeComponent } from './admin-home/admin-home.component';
import { FunctionComponent } from './function/function.component';
import { RoleComponent } from './role/role.component';
import { AdminHomeModule } from './admin-home/admin-home.module';
import { UserComponent } from './admin-user/admin-user.component';
import { AdminUserModule } from './admin-user/admin-user.module';
import { ServiceCategoryComponent } from './service-category/service-category.component';
import { ServiceTagComponent } from './service-tag/service-tag.component';
import { TranslationService } from 'src/app/core/services/translation.service';



@NgModule({
  imports: [
    CommonModule,
    AdminUserModule,
    AdminHomeModule,
    RouterModule.forChild(mainRoutes)
  ],
  declarations: [MainComponent,SidebarMenuComponent,TopMenuComponent],
  providers:[UtilityService,AuthenService,TranslationService]
})
export class AdminMainModule { }
