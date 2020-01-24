import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";
import { LoginComponent } from "./login.component";
import { AuthenService } from "src/app/core/services/authen.service";
import { NotificationService } from "src/app/core/services/notification.service";
import {
  GoogleLoginProvider,
  FacebookLoginProvider,
  AuthService
} from "angular-6-social-login";
import { SocialLoginModule, AuthServiceConfig } from "angular-6-social-login";
import { Routes, RouterModule } from "@angular/router";
import { FormsModule } from "@angular/forms";
export const routes: Routes = [
  {
    path: "",
    component: LoginComponent
  }
];
export function socialConfigs() {
  const config = new AuthServiceConfig([
    {
      id: FacebookLoginProvider.PROVIDER_ID,
      provider: new FacebookLoginProvider("734931260332638")
    },
    {
      id: GoogleLoginProvider.PROVIDER_ID,
      provider: new GoogleLoginProvider("306109909114-c06j0q9fg0s7ktfbnfe1ibr0vcqschbs.apps.googleusercontent.com")
    }
  ]);
  return config;
}
@NgModule({
  imports: [CommonModule, FormsModule, RouterModule.forChild(routes)],
  providers: [
    AuthenService,
    AuthService, 
    NotificationService,
    {
      provide: AuthServiceConfig,
      useFactory: socialConfigs
    }
  ],
  declarations: [LoginComponent]
})
export class LoginModule {}
