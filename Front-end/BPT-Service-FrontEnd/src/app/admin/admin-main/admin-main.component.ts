import { Component, OnInit } from '@angular/core';
import { LoggedInUser } from 'src/app/core/domain/loggedin.user';
import { UtilityService } from 'src/app/core/services/utility.service';
import { AuthenService } from 'src/app/core/services/authen.service';
import { SystemConstants } from 'src/app/core/common/system,constants';
import { UrlConstants } from 'src/app/core/common/url.constants';


@Component({
  selector: 'app-main',
  templateUrl: './admin-main.component.html',
  styleUrls: ['./admin-main.component.css']
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
