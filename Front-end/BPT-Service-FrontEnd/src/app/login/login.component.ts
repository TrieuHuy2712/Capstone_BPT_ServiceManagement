import { Component, OnInit } from "@angular/core";
import { AuthenService } from "src/app/core/services/authen.service";
import { NotificationService } from "src/app/core/services/notification.service";
import { GoogleLoginProvider, FacebookLoginProvider, AuthService } from 'angular-6-social-login';
import { Router } from "@angular/router";
import { UrlConstants } from "src/app/core/common/url.constants";
import { from } from "rxjs";
import { Socialusers } from "../core/domain/social.user";
@Component({
  selector: "app-login",
  templateUrl: "./login.component.html",
  styleUrls: ["./login.component.css"]
})
export class LoginComponent implements OnInit {
  loading = false;
  model: any = {};
  returnUrl: string;
  response;
  socialusers = new Socialusers();
  constructor(
    private authenService: AuthenService,
    private notificationService: NotificationService,
    private router: Router,
    private authService: AuthService,
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
      console.log(socialProvider, socialusers);
      console.log(socialusers);
    });
  }
  login() {
    this.loading = true;
    this.authenService
      .login(this.model.username, this.model.password)
      .subscribe(
        data => {
          this.router.navigate([UrlConstants.HOME]);
        },
        error => {
          this.notificationService.printErrorMessage("Có lỗi rồi nhóc");
          this.loading = false;
        }
      );
  }
}
