import {
  AuthService,
  FacebookLoginProvider,
  GoogleLoginProvider
} from "angular-6-social-login";
import { Component, OnInit } from "@angular/core";
import { HttpClient, HttpHeaders } from "@angular/common/http";

import { AuthenService } from "src/app/core/services/authen.service";

import { NotificationService } from "src/app/core/services/notification.service";
import { Router } from "@angular/router";

import { UrlConstants } from "src/app/core/common/url.constants";
import { from } from "rxjs";
import { map } from "rxjs/operators";
import { Socialusers } from 'src/app/core/domain/social.user';
import { DataService } from 'src/app/core/services/data.service';

@Component({
  selector: "app-login",
  templateUrl: "./admin-login.component.html",
  styleUrls: ["./admin-login.component.css"]
})
export class LoginComponent implements OnInit {
  blocked = false;
  loading = false;
  model: any = {};
  returnUrl: string;
  socialusers = new Socialusers();

  constructor(
    private authenService: AuthenService,
    private notificationService: NotificationService,
    private router: Router,
    private authService: AuthService,
    private _dataService: DataService,
    private _http: HttpClient
  ) {}

  ngOnInit() {}

  public socialSignIn(socialProvider: string) {
    let socialPlatformProvider;
    if (socialProvider === "facebook") {
      socialPlatformProvider = FacebookLoginProvider.PROVIDER_ID;
    } else if (socialProvider === "google") {
      socialPlatformProvider = GoogleLoginProvider.PROVIDER_ID;
    }
    this.authService.signIn(socialPlatformProvider).then(socialusers => {
      this.socialusers = socialusers;
      console.log(this.socialusers);
      let loggedUsers = {
        Token :this.socialusers.idToken,
        FullName : this.socialusers.name,
        UserName : this.socialusers.name,
        Email : this.socialusers.email,
        Avatar : this.socialusers.image
      };
      console.log(loggedUsers);
      this.authenService.loginExternal(loggedUsers).subscribe(
        data => {
          this.router.navigate([UrlConstants.HOME]);
        },
        error => {
          this.notificationService.printErrorMessage("Có lỗi rồi nhóc");
          this.loading = false;
        }
      );
    });
  }

  login() {
    this.loading = true;
    this.authenService
      .login(this.model.username, this.model.password)
      .subscribe(
        data => {
          console.log(data);
          if (data == null)
            this.notificationService.printErrorMessage(
              "Username or password is incorrect"
            );
          if (data.token == "BPT-Service-Lockedout") {
            this.blocked = true;
            setTimeout(
              function() {
                this.blocked = false;
                console.log(this.blocked);
              }.bind(this),
              300000
            );
            this.notificationService.printErrorMessage(
              "You was blocked out in 5 minutes"
            );
          } else {
            this.router.navigate([UrlConstants.HOME]);
          }
          // if(data == null)
          //   this.notificationService.printErrorMessage("Username or password is incorrect");
        },
        error => {
          this.notificationService.printErrorMessage("Có lỗi rồi nhóc");
          this.loading = false;
        }
      );
  }
}
