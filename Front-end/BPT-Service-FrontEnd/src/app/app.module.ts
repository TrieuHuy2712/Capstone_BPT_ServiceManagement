import { BrowserModule } from '@angular/platform-browser';
import { NgModule, NO_ERRORS_SCHEMA } from '@angular/core';
import {PaginationModule} from 'ngx-bootstrap/pagination';
import { ModalModule } from "ngx-bootstrap";
import {
  SocialLoginModule, AuthServiceConfig,
  AuthService,
  FacebookLoginProvider,
  GoogleLoginProvider,
  LinkedinLoginProvider
} from "angular-6-social-login";

import { MatProgressSpinnerModule } from "@angular/material/progress-spinner";
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

import {HttpClientModule, HTTP_INTERCEPTORS} from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { MainComponent } from './component/main/main.component';
import { BaseComponent } from './component/base/base.component';
import { RoleComponent } from './component/main/role/role.component';
import { FunctionComponent } from './component/main/function/function.component';
import { UserComponent } from './component/main/user/user.component';
import { CategoryComponent } from './component/main/category/category.component';
import { ServiceComponent } from './component/main/service/service.component';
import { ServiceTagComponent } from './component/main/service-tag/service-tag.component';
import { LoginComponent } from './login/login.component';
import { AuthGuard } from './core/guards/auth.guard';
import { RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { routes } from './login/login.module';
import { AuthenService } from './core/services/authen.service';
import { SystemConstants } from './core/common/system,constants';
import { LoaderComponent } from './component/loader/loader.component';
import { LoaderInterceptor } from './interceptor/loader.interceptor';
import { AuthInterceptor } from './auth/auth.interceptor';

export function getAuthServiceConfigs() {
  let config = new AuthServiceConfig(
      [
        {
          id: FacebookLoginProvider.PROVIDER_ID,
	      provider: new FacebookLoginProvider("Your-Facebook-app-id")
        },
        {
          id: GoogleLoginProvider.PROVIDER_ID,
	      provider: new GoogleLoginProvider("Your-Google-Client-Id")
        },
          {
            id: LinkedinLoginProvider.PROVIDER_ID,
            provider: new LinkedinLoginProvider("1098828800522-m2ig6bieilc3tpqvmlcpdvrpvn86q4ks.apps.googleusercontent.com")
          },
      ]
  );
  return config;
}

@NgModule({
  declarations: [
    LoaderComponent,
    AppComponent,
    MainComponent,
    BaseComponent,
    RoleComponent,
    FunctionComponent,
    UserComponent,  
    CategoryComponent,
    ServiceComponent,
    ServiceTagComponent,
    LoginComponent,
  ],
  imports: [
    BrowserModule,
    CommonModule,
    FormsModule,
    RouterModule.forChild(routes),
    AppRoutingModule,
    HttpClientModule,
    FormsModule,
    PaginationModule.forRoot(),
    ModalModule.forRoot(),
    SocialLoginModule,
    BrowserAnimationsModule,
    MatProgressSpinnerModule,
  ],
  providers: [AuthGuard, AuthenService, AuthService, SystemConstants,
  {
      provide: AuthServiceConfig,
      useFactory: getAuthServiceConfigs
  },
  { provide: HTTP_INTERCEPTORS, useClass: LoaderInterceptor, multi: true },
  { provide: HTTP_INTERCEPTORS, useClass: AuthInterceptor, multi: true }
  ],
  bootstrap: [AppComponent],
  schemas: [ NO_ERRORS_SCHEMA ]
})
export class AppModule { }
