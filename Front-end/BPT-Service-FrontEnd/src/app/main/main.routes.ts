import { Routes } from "@angular/router";
import { MainComponent } from "./main.component";
export const mainRoutes: Routes = [
  {
    path: "",
    component: MainComponent,
    children: [
      //localhost:4200/main/
      { path: "", redirectTo: "home", pathMatch: "full" },
      //localhost:4200/main/home
      { path: "home", loadChildren: "./home/home.module#HomeModule" },
      { path: "location", loadChildren: "./location/location.module#LocationModule" },
      { path: "user", loadChildren: "./user/user.module#UserModule" },
      {
        path: "listOfItem", loadChildren: "./list-of-item/list-of-item.module#ListOfItemModule"
      },

      {
        path: "detailItem", loadChildren: "./detail-item/detail-item.module#DetailItemModule"
      },
      {
        path: "userManage", loadChildren: "./user-manage/user-manage.module#UserManageModule"
      },
    ]
  }
];
