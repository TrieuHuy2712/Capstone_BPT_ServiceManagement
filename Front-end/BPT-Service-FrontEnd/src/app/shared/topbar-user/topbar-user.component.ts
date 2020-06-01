import { Component, OnInit, AfterViewInit } from '@angular/core';
import { AuthenService } from 'src/app/core/services/authen.service';
import { Router } from '@angular/router';
import { NotificationService } from 'src/app/core/services/notification.service';
import { LanguageService } from 'src/app/core/services/language.service';
import { LoggedInUser } from 'src/app/core/domain/loggedin.user';
import { SystemConstants } from 'src/app/core/common/system,constants';
import { UrlConstants } from 'src/app/core/common/url.constants';
import { DataService } from '../../core/services/data.service';

@Component({
  selector: 'app-topbar-user',
  templateUrl: './topbar-user.component.html',
  styleUrls: ['./topbar-user.component.css']
})
export class TopbarUserComponent implements OnInit, AfterViewInit{
  public user: LoggedInUser;
  public quantityNotification = 0;
  public indexNotification = 0;
  public listNotification: any;
  public listNotificationinHtml: any;

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

  logout() {
    localStorage.removeItem(SystemConstants.CURRENT_USER);
    localStorage.clear();
    this._authenService.logout();
    this.router.navigate([UrlConstants.LOGIN]);
    this.notificationService.printErrorMessage("Đã logout");
  }
  ngAfterViewInit(): void {
    if (localStorage.getItem(SystemConstants.CURRENT_USER)) {
      this.GetInitNotification();
    }
  }

  GetInitNotification() {
    this.dataService.get('/Notification/AutoGetUserNotification').subscribe((res: any) => {
      this.listNotification = res.logs;
      this.quantityNotification = res.numberNotification;
      this.listNotificationinHtml = this.listNotification.splice(0, this.indexNotification + 7);
      setInterval(() => this.AutoGetNotification(), 60000);
    });
  }
  AutoGetNotification() {
    this.dataService.get('/Notification/RealTimeUserNotification').subscribe((res: any) => {
      if(res.numberNotification > 0){
        this.listNotification.push(res.logs);
        this.quantityNotification = res.numberNotification;
        this.listNotificationinHtml = this.listNotification.splice(0, this.indexNotification + 7);
      }
    });
  }

  HasBeenClick() {
    this.dataService.get('/Notification/GetUserNotificationHasRead').subscribe((res: any) => {
      this.quantityNotification = 0;
    });
  }

  // MoreNotification() {
  //   this.indexNotification = this.indexNotification + 7;
  //   this.listNotificationinHtml = this.listNotification.splice(0, this.indexNotification);
  // }

}
