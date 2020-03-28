import { Routes } from "@angular/router";
import { MyServiceComponent } from './my-service.component';
export const myServiceRoutes: Routes = [
  {
    path: "",
    component: MyServiceComponent,
    children: [
      {
        path: "post", loadChildren: "./post/post.module#PostModule"
      },
      {
        path: "view", loadChildren: "./view/view.module#ViewModule"
      },
    ]
  }
];
