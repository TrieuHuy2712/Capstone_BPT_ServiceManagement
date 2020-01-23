import { Routes } from "@angular/router";
import { MainComponent } from "./main.component";
import { AuthGuard } from 'src/app/core/guards/auth.guard';
import { LoginComponent } from 'src/app/login/login.component';
export const mainRoutes: Routes = [
  {
    path: "",
    component: MainComponent,
    canActivate: [AuthGuard],
    children: [
      //localhost:4200/main/
      { path: "", redirectTo: "home", pathMatch: "full" },
      //localhost:4200/main/home
      { path: "home", loadChildren: "./home/home.module#HomeModule" },
      { path: "user", loadChildren: "./user/user.module#UserModule" },
      { path: "role", loadChildren: "./role/role.module#RoleModule" },
      { path: "category", loadChildren: "./category/category.module#CategoryModule" },
      { path: "tag", loadChildren: "./service-tag/service-tag.module#ServiceTagModule" },
      {
        path: "function",
        loadChildren: "./function/function.module#FunctionModule"
      },
      {
        path: 'login', redirectTo: 'login', component: LoginComponent
      }
    ]
  }
];
