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
  link: string;
  isSidebar: boolean = false;
  currentURL = '';
  public user: LoggedInUser;
  public constant: SystemConstants;
  constructor(private utilityService: UtilityService, private authenService: AuthenService) {
    this.currentURL = window.location.href;
  }

  ngOnInit() {
    this.user = JSON.parse(localStorage.getItem(SystemConstants.CURRENT_USER));
    console.log(this.user);
    SystemConstants.const_permission = this.user.username;
    console.log(this.currentURL);
    if (this.currentURL == SystemConstants.BASE_API+"main/userManage/followingProvider/index" 
    || this.currentURL == SystemConstants.BASE_API+"main/userManage/userProfile/index" 
    || this.currentURL == SystemConstants.BASE_API+"main/userManage/followingService/index" 
    || this.currentURL == SystemConstants.BASE_API+"main/userManage/email" 
    || this.currentURL == SystemConstants.BASE_API+"main/userManage/email/inbox/index" 
    || this.currentURL == SystemConstants.BASE_API+"main/userManage/email/read/index" 
    || this.currentURL == SystemConstants.BASE_API+"main/userManage/email/sent/index" 
    || this.currentURL == SystemConstants.BASE_API+"main/userManage/myService/index"
    || this.currentURL == SystemConstants.BASE_API+"main/userManage/myService/post/index"
    || this.currentURL == SystemConstants.BASE_API+"main/userManage/myService/view/index") {
      this.isSidebar = true;
    }
    // else if(this.currentURL.includes("/main/detailItem")){
    //   this.isSidebar = false;
    // }
    
  }
  logout() {
    localStorage.removeItem(SystemConstants.CURRENT_USER);
    this.utilityService.navigate(UrlConstants.LOGIN);
  }

}
