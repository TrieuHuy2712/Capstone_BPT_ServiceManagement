import { Component, OnInit } from "@angular/core";
import { LoggedInUser } from "src/app/core/domain/loggedin.user";
import { AuthenService } from "src/app/core/services/authen.service";
import { UrlConstants } from "src/app/core/common/url.constants";
import { SystemConstants } from "src/app/core/common/system,constants";
import { Router } from "@angular/router";
import { NotificationService } from "src/app/core/services/notification.service";
import { LanguageService } from 'src/app/core/services/language.service';


@Component({
  selector: "app-top-menu",
  templateUrl: "./top-menu.component.html",
  styleUrls: ["./top-menu.component.css"]
})
export class TopMenuComponent implements OnInit {
  public user: LoggedInUser;
  loading = false;
  searchData: string = "";
  public isAdministrator = false;
  public guestName: string;
  public isLogin: boolean = true;
  constructor(
    private _authenService: AuthenService,
    private router: Router,
    private notificationService: NotificationService,
    private languageService: LanguageService

  ) {}

  ngOnInit() {
    this.user = this._authenService.getLoggedInUser();
    console.log("this session have user login? "+this.user);
    if(this.user == null){
      this.guestName = "Khách";
      this.isLogin != this.isLogin;
    }
    if(this.user !== null){
      if(this.user.fullName == "Administrator"){
        this.isAdministrator = true;
      }
       else if(this.user.fullName == null){
        this.isAdministrator = false;
      }
    }
  }

  logout() {
    localStorage.removeItem(SystemConstants.CURRENT_USER);
    localStorage.clear();
    this._authenService.logout();
    this.router.navigate([UrlConstants.LOGIN]);
    this.notificationService.printErrorMessage("Đã đăng xuất");
    this.loading = false;
  }

  onChange(deviceValue) {
    console.log(deviceValue);
    this.languageService.setLanguage(deviceValue);
  }
  dataSearchBinding(val: any){
    this.searchData = val;
    console.log(this.searchData);
  }
}
