import { Routes } from "@angular/router";
import { EmailComponent } from './email.component';
export const emailRoutes: Routes = [
  {
    path: "",
    component: EmailComponent,
    children: [
      {
        path: "inbox", loadChildren: "./inbox/inbox.module#InboxModule"
      },
      {
        path: "read", loadChildren: "./read/read.module#ReadModule"
      },
      {
        path: "sent", loadChildren: "./sent/sent.module#SentModule"
      }
      
    ]
  }
];
