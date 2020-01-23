import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { MainComponent } from './component/main/main.component';
import { RoleComponent } from './component/main/role/role.component';
import { AuthGuard } from './core/guards/auth.guard';
import { FunctionComponent } from './component/main/function/function.component';
import { UserComponent } from './component/main/user/user.component';
import { CategoryComponent } from './component/main/category/category.component';
import { ServiceComponent } from './component/main/service/service.component';
import { ServiceTagComponent } from './component/main/service-tag/service-tag.component';
import { LoginComponent } from './login/login.component';


const routes: Routes = [
  {
    path: '',
    redirectTo: 'main',
    pathMatch: 'full'
  },
  {
    path: 'main',
    component: MainComponent,
    children:[
      {
        path: 'role',
        component: RoleComponent
      },
      {
        path: 'function',
        component: FunctionComponent
      },
      {
        path: 'user',
        component: UserComponent
      },
      {
        path: 'category',
        component: CategoryComponent
      },
      {
        path: 'service',
        component: ServiceComponent
      },
      {
        path: 'service_tag',
        component: ServiceTagComponent
      }
    ]
  },
  {
    path: 'login',
    component: LoginComponent
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
