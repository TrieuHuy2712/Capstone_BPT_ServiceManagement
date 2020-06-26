import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { DataService } from 'src/app/core/services/data.service';
import { NotificationService } from 'src/app/core/services/notification.service';
import { SystemConstants } from 'src/app/core/common/system,constants';
import { MessageConstants } from 'src/app/core/common/message.constants';

@Component({
  selector: 'app-view-about-provider',
  templateUrl: './view-about-provider.component.html',
  styleUrls: ['./view-about-provider.component.css']
})
export class ViewAboutProviderComponent implements OnInit {
  userId: string;
  newId: string;
  public user: any;
  public isFollowedProvider: boolean;
  public provider: any;
  public UIS: any[];
  public viewOfService: any[];
  public id: any;
  avaPath: any[];
  provName: any;
  

  // follow param
  public followAProviderEntity: any;
  public checkId: any;
  public checkArr: any[];
  public userIdInCheck: any;



  constructor(
    private route: ActivatedRoute,
    private _dataService: DataService,
    private _notificationService: NotificationService,
  ) { }

  ngOnInit() {
    this.newId = this.route.snapshot.paramMap.get("id");
    console.log("ket qua ne " + this.userId);
    this.loadData();
    this.loadServiceOfProvice();
    this.checkFollowProvider();
  }

  loadData() {
    this._dataService.get("/Provider/GetProviderById/" + this.newId)
      .subscribe((response: any) => {
        this.provider = response.myModel;
        this.avaPath = response.myModel.avatarPath;
        this.provName = response.myModel.providerName;
        // console.log("provider " + this.provider);

      });
  }

  // follow a service
  followAProvider() {
    this.followAProviderEntity = {};
    this._dataService.get("/UserManagement/GetAllUser").subscribe((response: any) => {
      this.UIS = response;
      this.user = JSON.parse(localStorage.getItem(SystemConstants.CURRENT_USER));
      this.userId = this.UIS.find(x => x.fullName == this.user.fullName).id;
      this.followAProviderEntity.userId = this.userId;
      this.followAProviderEntity.providerId = this.newId;
      this.followAProviderEntity.isReceiveEmail = true;
      this._dataService
        .post(
          "/ProviderFollowing/FollowProviders",this.followAProviderEntity
        )
        .subscribe((response: any) => {
          this.viewOfService = response;
          console.log(this.viewOfService);
          if(response.isValid == true){
            this._notificationService.printSuccessMessage(MessageConstants.FOLLOW_PROVIDER_OK_MSG);
            this.checkFollowProvider();
          }
          else{
            this._notificationService.printErrorMessage(MessageConstants.FOLLOW_PROVIDER_FAIL_MSG);
          }
          this.checkFollowProvider();

        });
    });
    
  }

  // unfollow a service
  unFollowAProvider() {
    this.followAProviderEntity = {};
    this._dataService.get("/UserManagement/GetAllUser").subscribe((response: any) => {
      this.UIS = response;
      this.user = JSON.parse(localStorage.getItem(SystemConstants.CURRENT_USER));
      this.userId = this.UIS.find(x => x.fullName == this.user.fullName).id;
      this.followAProviderEntity.userId = this.userId;
      this.followAProviderEntity.providerId = this.newId;
      this.followAProviderEntity.isReceiveEmail = true;
      this._dataService
        .post(
          "/ProviderFollowing/UnFollowProviders",this.followAProviderEntity
        )
        .subscribe((response: any) => {
          if(response.isValid == true){
            this._notificationService.printSuccessMessage(MessageConstants.UNFOLLOW_PROVIDER_OK_MSG);
            
          }
          else{
            this._notificationService.printErrorMessage(MessageConstants.UNFOLLOW_PROVIDER_FAIL_MSG);
          }
          
        });
    });
    
  }

  // check if current user followed a provider or not
  checkFollowProvider(){
    this._dataService.get("/UserManagement/GetAllUser").subscribe((response: any) => {
      this.UIS = response;
      this.user = JSON.parse(localStorage.getItem(SystemConstants.CURRENT_USER));
      this.userIdInCheck = this.UIS.find(x => x.fullName == this.user.fullName).id;
      console.log("userid in follow provider = "+this.userIdInCheck);
      
      this._dataService.get("/ProviderFollowing/GetListProviderFollow?idProvider="+this.newId)
        .subscribe((response: any) => {
          this.checkArr = response;
          // this.checkId = response[0].userId;
          this.checkId = this.checkArr.find(x => x.userId == this.userIdInCheck).userId;
          console.log("checkId = "+this.checkId);
          if (this.checkArr == []) {
            this.isFollowedProvider = false;
            console.log("isFollow = "+this.isFollowedProvider);
          }
          else if(this.userIdInCheck == this.checkId) {
            this.isFollowedProvider = true;
            console.log("isFollow = "+this.isFollowedProvider);
          }
          else if(this.userIdInCheck !== this.checkId){
            this.isFollowedProvider = false;
            console.log("isFollow = "+this.isFollowedProvider);
          }

        });
    });
  }

  loadServiceOfProvice(){
    debugger;
    this._dataService.get("/UserManagement/GetAllUser").subscribe((response: any) => {
      this.UIS = response;
      this.user = JSON.parse(localStorage.getItem(SystemConstants.CURRENT_USER));
      this.userId = this.UIS.find(x => x.fullName == this.user.fullName).id;
      this._dataService
        .get(
          "/Service/getAllPostUserServiceByUserId?idUser="+this.userId+"&isProvider=true"
        )
        .subscribe((response: any) => {
          this.viewOfService = response;
          console.log(this.viewOfService);
        });
    });
  }


}
