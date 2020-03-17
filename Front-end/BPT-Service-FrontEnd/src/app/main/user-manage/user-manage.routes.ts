import { Routes } from "@angular/router";
import { UserManageComponent } from './user-manage.component';
export const userManageRoutes: Routes = [
  {
    path: "",
    component: UserManageComponent,
    children: [
      {
        path: "followingProvider", loadChildren: "./following-provider/following-provider.module#FollowingProviderModule"
      },
      {
        path: "followingService", loadChildren: "./following-service/following-service.module#FollowingServiceModule"
      }
    ]
  }
];
