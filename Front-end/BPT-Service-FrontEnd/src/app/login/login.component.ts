import { Component, OnInit } from "@angular/core";
import { AuthenService } from "src/app/core/services/authen.service";
import { NotificationService } from "src/app/core/services/notification.service";
import {
  GoogleLoginProvider,
  FacebookLoginProvider,
  AuthService
} from "angular-6-social-login";
import { Router } from "@angular/router";
import { UrlConstants } from "src/app/core/common/url.constants";
import { from } from "rxjs";
import { Socialusers } from "../core/domain/social.user";
import { DataService } from "../core/services/data.service";
import { SystemConstants } from "../core/common/system,constants";
import { LoggedInUser } from "../core/domain/loggedin.user";
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
    private _dataService: DataService
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

  loginExternal() {
    debugger;
    this._dataService
      .post("/UserManagement/LoginExternal/", this.socialusers)
      .subscribe(
        (response: any) => {
          this.socialusers = response;
          if (this.socialusers && this.socialusers.token) {
            let loggedUsers = new LoggedInUser(
              this.socialusers.token,
              this.socialusers.name,
              this.socialusers.name,
              this.socialusers.email,
              this.socialusers.email,
              "Customer",
              null
            );
            localStorage.removeItem(SystemConstants.CURRENT_USER);
            localStorage.setItem(
              SystemConstants.CURRENT_USER,
              JSON.stringify(loggedUsers)
            );
            localStorage.setItem(
              SystemConstants.const_username,
              this.socialusers.name
            );
          }
          this.router.navigate([UrlConstants.HOME]);
        },
        error => this._dataService.handleError(error)
      );
  }

  login() {
    this.loading = true;
    this.authenService
      .login(this.model.username, this.model.password)
      .subscribe(
        data => {
          console.log(data)
          if(data == null)
            this.notificationService.printErrorMessage("Username or password is incorrect");
          if(data.token=="BPT-Service-Lockedout"){
            this.blocked = true;
            setTimeout(function() {
              this.blocked = false;
              console.log(this.blocked);
          }.bind(this), 300000);
            this.notificationService.printErrorMessage("You was blocked out in 5 minutes");
          }
          else{
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
