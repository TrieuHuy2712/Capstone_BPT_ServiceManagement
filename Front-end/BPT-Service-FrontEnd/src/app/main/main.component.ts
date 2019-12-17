import { Component, OnInit } from '@angular/core';
import { LoggedInUser } from '../core/domain/loggedin.user';
import { UtilityService } from '../core/services/utility.service';
import { AuthenService } from '../core/services/authen.service';
import { SystemConstants } from '../core/common/system,constants';
import { UrlConstants } from '../core/common/url.constants';

@Component({
  selector: 'app-main',
  templateUrl: './main.component.html',
  styleUrls: ['./main.component.css']
})
export class MainComponent implements OnInit {

  public user: LoggedInUser;
  constructor(private utilityService: UtilityService, private authenService: AuthenService) { }

  ngOnInit() {
    this.user = JSON.parse(localStorage.getItem(SystemConstants.CURRENT_USER));
    console.log(this.user);
    SystemConstants.const_permission = this.user.username;
  }
  logout() {
    localStorage.removeItem(SystemConstants.CURRENT_USER);
    this.utilityService.navigate(UrlConstants.LOGIN);
  }
}
