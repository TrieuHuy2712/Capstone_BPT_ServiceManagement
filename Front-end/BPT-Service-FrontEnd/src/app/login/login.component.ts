import {
  AuthService,
  FacebookLoginProvider,
  GoogleLoginProvider
} from "angular-6-social-login";
import { Component, OnInit } from "@angular/core";
import { HttpClient, HttpHeaders } from "@angular/common/http";

import { AuthenService } from "src/app/core/services/authen.service";
import { DataService } from "../core/services/data.service";
import { LoggedInUser } from "../core/domain/loggedin.user";
import { NotificationService } from "src/app/core/services/notification.service";
import { Router } from "@angular/router";
import { Socialusers } from "../core/domain/social.user";
import { SystemConstants } from "../core/common/system,constants";
import { UrlConstants } from "src/app/core/common/url.constants";
import { from } from "rxjs";
import { map } from "rxjs/operators";

@Component({
  selector: "app-login",
  templateUrl: "./login.component.html",
  styleUrls: ["./login.component.css"]
})
export class LoginComponent implements OnInit {
  blocked = false;
  loading = false;
  model: any = {};
  returnUrl: string;
  pass: boolean = true;
  passVal: string = "";
  email: boolean =true;
  emailVal: string = "";
  registerBtn: boolean =false;
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
      let loggedUsers = {
        Token :this.socialusers.idToken,
        FullName : this.socialusers.name,
        UserName : this.socialusers.name,
        Email : this.socialusers.email,
        Avatar : this.socialusers.image
      };
      this.authenService.loginExternal(loggedUsers).subscribe(
        data => {
          this.router.navigate([UrlConstants.HOME]);
        },
        error => {
          this.notificationService.printErrorMessage("No Authenticated");
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
          }
          else if(data.fullName == "Administrator"){
            this.router.navigate([UrlConstants.ADMINHOME]);
          }
          else {
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

  emailValid(event){
    this.emailVal = event.target.value;
    if(event.target.value.length < 4){
      this.email = false;
    }
    else{
      this.email = true;
    }
  }

  passValid(event){
    if(event.target.value.length < 4){
      this.pass = false;
    }
    else{
      this.pass = true;
    }
    // if(event.target.value.length > 4 && this.emailVal.length > 4){
    //   this.registerBtn = false;
    // }
    // else{
    //   this.registerBtn = true;
    // }
  }


}
