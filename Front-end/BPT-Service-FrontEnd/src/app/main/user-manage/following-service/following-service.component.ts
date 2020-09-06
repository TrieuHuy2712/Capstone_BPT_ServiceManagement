import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { DataService } from 'src/app/core/services/data.service';
import { NotificationService } from 'src/app/core/services/notification.service';
import { LoggedInUser } from 'src/app/core/domain/loggedin.user';
import { SystemConstants } from 'src/app/core/common/system,constants';

@Component({
  selector: 'app-following-service',
  templateUrl: './following-service.component.html',
  styleUrls: ['./following-service.component.css']
})
export class FollowingServiceComponent implements OnInit {

  public UIS: any[];
  public usId: any;
  public user: LoggedInUser;
  public followingService:any[];



  constructor(
    private route: ActivatedRoute,
    private _dataService: DataService,
    private _notificationService: NotificationService,
  ) { }

  ngOnInit() {
    this.user = JSON.parse(localStorage.getItem(SystemConstants.CURRENT_USER));

    this.followAService();
  }

  followAService() {
    debugger;
    this._dataService.get("/UserManagement/GetAllUser").subscribe((response: any) => {
      this.UIS = response;
      this.usId = this.UIS.find(x => x.fullName == this.user.fullName).id;
      
      this._dataService
        .get(
          "/ServiceFollowing/GetUserFollow?idUser="+ this.usId
        )
        .subscribe((response: any) => {
          this.followingService = response;
        });
    });

  }

}
