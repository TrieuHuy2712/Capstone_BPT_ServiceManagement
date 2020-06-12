import { HttpClient, HttpHeaders, HttpResponse } from "@angular/common/http";
import { catchError, filter, map, mergeMap } from "rxjs/operators";

import { Injectable } from "@angular/core";
import { LoggedInUser } from "../domain/loggedin.user";
import { Observable } from "rxjs";
import { Socialusers } from "../domain/social.user";
import { SystemConstants } from "../common/system,constants";
import { UrlConstants } from "../common/url.constants";

@Injectable({
  providedIn: "root"
})
export class AuthenService {
  constructor(private _http: HttpClient) {}
  login(username: string, password: string) {
    let body = {
      userName: encodeURIComponent(username),
      password: password
    };
    let headers = new HttpHeaders().set(
      "Authorization",
      "Bearer " + localStorage.getItem("access_token")
    );
    headers.append("Content-Type", "application/x-www-form-urlencoded");
    let options = { headers: headers };

    return this._http
      .post(SystemConstants.BASE_API + "/Authenticate/authenticate", body)
      .pipe(
        map((response: any) => {
          const user = response;
          if (user && user.token) {
            localStorage.removeItem(SystemConstants.CURRENT_USER);
            localStorage.setItem(
              SystemConstants.CURRENT_USER,
              JSON.stringify(user)
            );
            localStorage.setItem(SystemConstants.const_username, user.userName);
          }
          return user;
        })
      );
  }
  logout() {
    localStorage.removeItem(SystemConstants.CURRENT_USER);
  }
  isUserAuthenticated(): boolean {
    let user = localStorage.getItem(SystemConstants.CURRENT_USER);
    if (user != null) {
      return true;
    } else {
      return false;
    }
  }
  loginExternal(socialusers: any) {
    debugger
    return this._http
      .post(
        SystemConstants.BASE_API + "/UserManagement/LoginExternal/",
        socialusers
      )
      .pipe(
        map(
          (response: any) => {
            socialusers = response;
            if (socialusers && socialusers.token) {
              localStorage.removeItem(SystemConstants.CURRENT_USER);
              localStorage.setItem(SystemConstants.CURRENT_USER , JSON.stringify(socialusers));
              localStorage.setItem(SystemConstants.const_username,socialusers.userName);
            }
            return socialusers;
            //this.router.navigate([UrlConstants.HOME]);
          },
        )
      );
  }
  getLoggedInUser(): LoggedInUser {
    let user: LoggedInUser;
    if (this.isUserAuthenticated()) {
      var userData = JSON.parse(
        localStorage.getItem(SystemConstants.CURRENT_USER)
      );
      user = new LoggedInUser(
        userData.token,
        userData.username,
        userData.fullName,
        userData.Email,
        userData.avatar,
        userData.roles,
        userData.permissions,
        userData.isProvider
      );
    } else {
      user = null;
    }
    return user;
  }
  checkAccess(functionId: string) {
    var user = this.getLoggedInUser();
    var result: boolean = false;
    var permission: any[] = JSON.parse(user.permissions);
    var roles: any[] = JSON.parse(user.roles);
    var hasPermission: number = permission.findIndex(
      x => x.FunctionId == functionId && x.CanRead == true
    );
    if (hasPermission != -1 || roles.findIndex(x => x == "admin") != -1) {
      return true;
    } else return false;
  }
  hasPermission(functionId: string, action: string): boolean {
    var user = this.getLoggedInUser();
    var result: boolean = false;
    var permission: any[] = JSON.parse(user.permissions);
    var roles: any[] = JSON.parse(user.roles);
    switch (action) {
      case "create":
        var hasPermission: number = permission.findIndex(
          x => x.FunctionId == functionId && x.CanCreate == true
        );
        if (hasPermission != -1 || roles.findIndex(x => x == "Admin") != -1)
          result = true;
        break;
      case "update":
        var hasPermission: number = permission.findIndex(
          x => x.FunctionId == functionId && x.CanUpdate == true
        );
        if (hasPermission != -1 || roles.findIndex(x => x == "Admin") != -1)
          result = true;
        break;
      case "delete":
        var hasPermission: number = permission.findIndex(
          x => x.FunctionId == functionId && x.CanDelete == true
        );
        if (hasPermission != -1 || roles.findIndex(x => x == "Admin") != -1)
          result = true;
        break;
    }
    return result;
  }
}
