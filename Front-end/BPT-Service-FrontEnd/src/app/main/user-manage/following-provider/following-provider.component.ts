import { Component, OnInit } from '@angular/core';
import { DataService } from 'src/app/core/services/data.service';
import { NotificationService } from 'src/app/core/services/notification.service';
import { UploadService } from 'src/app/core/services/upload.service';
import { LoggedInUser } from 'src/app/core/domain/loggedin.user';
import { SystemConstants } from 'src/app/core/common/system,constants';
@Component({
  selector: 'app-following-provider',
  templateUrl: './following-provider.component.html',
  styleUrls: ['./following-provider.component.css']
})
export class FollowingProviderComponent implements OnInit {

  // Provider params
  public currentProvider: any[];
  public currentProviderId: any;
  public listFollower: any[];

  // User params
  public currentUser: LoggedInUser;
  public allUser: any[];
  public currentUserId: any;


  constructor(
    private _dataService: DataService,
    private _notificationService: NotificationService,
    private _uploadService: UploadService
  ) { }

  ngOnInit() {
    this.currentUser = JSON.parse(localStorage.getItem(SystemConstants.CURRENT_USER));
    console.log(this.currentUser.fullName);

    this.getCurrentProvider();
    // this.getAllUser();
  }

  ngAfterViewInit() {
    this.getAllUserFollowedProvider();
  }

  // get current provider loggin system
  getCurrentProvider() {
    this._dataService.get("/UserManagement/GetAllUser").subscribe((response: any) => {
      this.allUser = response;
      this.currentUserId = this.allUser.find(x => x.fullName == this.currentUser.fullName).id;
      console.log("current user id  = " + this.currentUserId);
      this._dataService.get("/Provider/GetAllPaging?filter=1").subscribe((responses: any) => {
        this.currentProvider = responses.results;
        this.currentProviderId = this.currentProvider.find(x => x.userId == this.currentUserId).id;
        if (this.currentProviderId == undefined) {
          this._notificationService.printErrorMessage("Bạn chưa phải là nhà cung cấp");
        }
        else {
          console.log("current provider id= " + this.currentProviderId);
          this._dataService.get("/ProviderFollowing/GetListProviderFollow?idProvider=" + this.currentProviderId).subscribe((response: any) => {
            this.listFollower = response;
            console.log("List of follower : " + this.listFollower);

          });
        }

      });
    });
  }

  //  get all user who followed u
  getAllUserFollowedProvider() {

  }

  // get all user
  // getAllUser() {
  //   this._dataService.get("/UserManagement/GetAllUser").subscribe((response: any) => {
  //     this.allUser = response;
  //     this.currentUserId = this.allUser.find(x => x.fullName == this.currentUser.fullName).id;
  //     console.log("current user id  = "+this.currentUserId);


  //   });
  // }
}
