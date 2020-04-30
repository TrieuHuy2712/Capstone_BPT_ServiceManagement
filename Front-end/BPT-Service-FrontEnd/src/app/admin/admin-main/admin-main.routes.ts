import { Routes } from "@angular/router";
import { MainComponent } from "./admin-main.component";
export const mainRoutes: Routes = [
  {
    path: "",
    component: MainComponent,
    children: [
      //localhost:4200/main/
      { path: "", redirectTo: "adminHome", pathMatch: "full" },
      //localhost:4200/main/home
      { path: "home", loadChildren: "./admin-home/admin-home.module#AdminHomeModule" },
      { path: "location", loadChildren: "./location/location.module#LocationModule" },
      { path: "user", loadChildren: "./admin-user/admin-user.module#AdminUserModule" },
      { path: "role", loadChildren: "./role/role.module#RoleModule" },
      { path: "category", loadChildren: "./service-category/service-category.module#ServiceCategoryModule" },
      { path: "tag", loadChildren: "./service-tag/service-tag.module#ServiceTagModule" },
      { path: "function", loadChildren: "./function/function.module#FunctionModule" },
      { path: "provider", loadChildren: "./provider/provider.module#ProviderModule" },
      { path: "news", loadChildren: "./news/news.module#NewsModule" },
      { path: "service", loadChildren: "./post-service/post-service.module#PostServiceModule" },
      { path: "email", loadChildren: "./admin-email/admin-email.module#AdminEmailModule" },
      { path: "logging", loadChildren: "./logging/logging.module#LoggingModule" },
    ]
  }
];
