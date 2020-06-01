import { Component, OnInit, AfterViewInit } from "@angular/core";
import { LoggedInUser } from "src/app/core/domain/loggedin.user";
import { AuthenService } from "src/app/core/services/authen.service";
import { UrlConstants } from "src/app/core/common/url.constants";
import { SystemConstants } from "src/app/core/common/system,constants";
import { Router } from "@angular/router";
import { NotificationService } from "src/app/core/services/notification.service";
import { LanguageService } from 'src/app/core/services/language.service';
import { DataService } from '../../../core/services/data.service';


@Component({
  selector: "app-top-menu",
  templateUrl: "./top-menu.component.html",
})
export class TopMenuComponent implements OnInit, AfterViewInit {
  public user: LoggedInUser;
  public quantityNotification = 0;
  public indexNotification = 0;
  public listNotification: any[];
  public listNotificationinHtml: any[];
  loading = false;
  constructor(
    private _authenService: AuthenService,
    private router: Router,
    private notificationService: NotificationService,
    private languageService: LanguageService,
    private dataService: DataService

  ) { }

  ngOnInit() {
    this.user = this._authenService.getLoggedInUser();
  }

  ngAfterViewInit(): void {
    if (localStorage.getItem(SystemConstants.CURRENT_USER)) {
      this.GetInitNotification();
    }
  }

  GetInitNotification() {
    this.dataService.get('/Notification/AutoGetNotification').subscribe((res: any) => {
      this.listNotification = res.logs;
      this.quantityNotification = res.numberNotification;
      this.listNotificationinHtml = this.listNotification.splice(0, this.indexNotification + 7);
      setInterval(() => this.AutoGetNotification(), 60000);
    });
  }
  AutoGetNotification() {
    this.dataService.get('/Notification/RealTimeNotification').subscribe((res: any) => {
      if(res.numberNotification>0){
        this.listNotification.push(res.logs);
        this.quantityNotification = res.numberNotification;
        this.listNotificationinHtml = this.listNotification.splice(0, this.indexNotification + 7);
      }
    });
  }

  logout() {
    localStorage.removeItem(SystemConstants.CURRENT_USER);
    localStorage.clear();
    this._authenService.logout();
    this.router.navigate([UrlConstants.LOGIN]);
    this.notificationService.printErrorMessage('Đã logout');
    this.loading = false;
  }

  onChange(deviceValue) {
    this.languageService.setLanguage(deviceValue);
  }

  HasBeenClick() {
    this.dataService.get('/Notification/GetNotificationHasRead').subscribe((res: any) => {
      if (res === true) {
        this.quantityNotification = 0;
      }

    });
  }

  // MoreNotification() {
  //   this.indexNotification = this.indexNotification + 7;
  //   this.listNotificationinHtml = this.listNotification.splice(0, this.indexNotification);
  // }
}
