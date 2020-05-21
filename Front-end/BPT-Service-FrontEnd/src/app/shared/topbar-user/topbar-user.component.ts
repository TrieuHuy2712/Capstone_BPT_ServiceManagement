import { Component, OnInit } from '@angular/core';
import { AuthenService } from 'src/app/core/services/authen.service';
import { Router } from '@angular/router';
import { NotificationService } from 'src/app/core/services/notification.service';
import { LanguageService } from 'src/app/core/services/language.service';
import { LoggedInUser } from 'src/app/core/domain/loggedin.user';
import { SystemConstants } from 'src/app/core/common/system,constants';
import { UrlConstants } from 'src/app/core/common/url.constants';

@Component({
  selector: 'app-topbar-user',
  templateUrl: './topbar-user.component.html',
  styleUrls: ['./topbar-user.component.css']
})
export class TopbarUserComponent implements OnInit {
  public user: LoggedInUser;

  constructor(
    private _authenService: AuthenService,
    private router: Router,
    private notificationService: NotificationService,
    private languageService: LanguageService
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

}
