import { Component, OnInit } from "@angular/core";
import { LoggedInUser } from "src/app/core/domain/loggedin.user";
import { AuthenService } from "src/app/core/services/authen.service";
import { UrlConstants } from "src/app/core/common/url.constants";
import { SystemConstants } from "src/app/core/common/system,constants";
import { Router } from "@angular/router";
import { NotificationService } from "src/app/core/services/notification.service";

@Component({
  selector: "app-top-menu",
  templateUrl: "./top-menu.component.html",
  styleUrls: ["./top-menu.component.css"]
})
export class TopMenuComponent implements OnInit {
  public user: LoggedInUser;
  loading = false;
  constructor(
    private _authenService: AuthenService,
    private router: Router,
    private notificationService: NotificationService
  ) {}

  ngOnInit() {
    this.user = this._authenService.getLoggedInUser();
  }

  logout() {
    localStorage.removeItem(SystemConstants.CURRENT_USER);
    localStorage.clear();
    this._authenService.logout();
    this.router.navigate([UrlConstants.LOGIN]);
    this.notificationService.printErrorMessage("Đã logout");
    this.loading = false;
  }
}
