import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MainComponent } from './main.component';
import { SidebarMenuComponent } from '../shared/sidebar-menu/sidebar-menu.component';
import { TopMenuComponent } from '../shared/top-menu/top-menu.component';
import { AuthenService } from 'src/app/core/services/authen.service';
import { UtilityService } from 'src/app/core/services/utility.service';
import { RouterModule } from '@angular/router';
import { mainRoutes } from './main.routes';
import { HomeComponent } from './home/home.component';
import { FunctionComponent } from './function/function.component';
import { RoleComponent } from './role/role.component';
import { HomeModule } from './home/home.module';
import { UserComponent } from './user/user.component';
import { UserModule } from './user/user.module';
import { ServiceCategoryComponent } from './service-category/service-category.component';
import { ServiceTagComponent } from './service-tag/service-tag.component';


@NgModule({
  imports: [
    CommonModule,
    UserModule,
    HomeModule,
    RouterModule.forChild(mainRoutes)
  ],
  declarations: [MainComponent,SidebarMenuComponent,TopMenuComponent],
  providers:[UtilityService,AuthenService]
})
export class MainModule { }
