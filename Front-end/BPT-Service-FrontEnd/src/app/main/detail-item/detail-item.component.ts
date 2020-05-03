import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { NavigationStart } from '@angular/router';
import { ActivatedRoute } from '@angular/router'
import { DataService } from 'src/app/core/services/data.service';
import { NotificationService } from 'src/app/core/services/notification.service';
import { LoggedInUser } from 'src/app/core/domain/loggedin.user';
import { SystemConstants } from 'src/app/core/common/system,constants';
import { MessageConstants } from 'src/app/core/common/message.constants';


@Component({
  selector: 'app-detail-item',
  templateUrl: './detail-item.component.html',
  styleUrls: ['./detail-item.component.css']
})
export class DetailItemComponent implements OnInit {
  
  public newId: string = "";
  public details: any=[];
  public Uid:string = "";
  public user: LoggedInUser;
  public isFollowed: boolean = false;
  public UIS: any[];
  public usId: any;

  public followEntity: any;
  public followServiceId:any;
  public unfEntity: any;
  public flId:any[];


  constructor(
    private route: ActivatedRoute,
    private _dataService: DataService,
    private _notificationService: NotificationService,
  ) {
  }

  ngOnInit() {
    console.log("ket qua = "+ this.usId);

    this.user = JSON.parse(localStorage.getItem(SystemConstants.CURRENT_USER));
    this.followEntity = {};
    this.unfEntity = {};
    this.newId = this.route.snapshot.paramMap.get("id");
    this.getFollowId();


    this.user = JSON.parse(localStorage.getItem(SystemConstants.CURRENT_USER));
    this.loadData();
    
  }

  
  sliderConfigDetail = { "slidesToShow": 2.5, "slidesToScroll": 1, "arrows": true, "autoplay":true, "autoplaySpeed": 500 };
  afterChange(e) {
    console.log('afterChange');
  }

  // load data
  loadData() {
    this._dataService.get("/Service/getPostServiceById?idService="+this.newId)
      .subscribe((response: any) => {
        this.details = response;
        this.Uid = response.userId;
        this.details.userId="";

      });
  }

  // follow a service

  followAService() {
    debugger;
    this._dataService.get("/UserManagement/GetAllUser").subscribe((response: any) => {
      this.UIS = response;
      this.usId = this.UIS.find(x => x.fullName == this.user.fullName).id;
      this.followEntity.userId = this.usId;
      this.followEntity.serviceId = this.newId;
      this._dataService
        .post(
          "/ServiceFollowing/FollowService", this.followEntity
        )
        .subscribe((response: any) => {
          if (response.isValid == true) {
            
            this._notificationService.printSuccessMessage(
              MessageConstants.CREATED_OK_MSG
              
            );
            // this.isFollowed = !this.isFollowed;
            this.getFollowId();
          } else {
            this._notificationService.printErrorMessage(
              MessageConstants.CREATED_FAIL_MSG
            );
          }
        });
    });

  }

  unFollowAService(){
    this._dataService.get("/UserManagement/GetAllUser").subscribe((response: any) => {
      this.UIS = response;
      this.usId = this.UIS.find(x => x.fullName == this.user.fullName).id;
      this._dataService.get("/ServiceFollowing/GetServiceFollow?idService="+this.newId)
      .subscribe((response: any) => {
        this.flId = response;
        this.followServiceId = this.flId.find(x => x.userId == this.usId).id;
        this.unfEntity.id = this.followServiceId;
        this._dataService
        .post(
          "/ServiceFollowing/UnFollowService", this.unfEntity
        )
        .subscribe((response: any) => {
          if (response.isValid == true) {
            
            this._notificationService.printSuccessMessage(
              MessageConstants.CREATED_OK_MSG
            );
            // this.isFollowed = !this.isFollowed;
            this.getFollowId();

          } else {
            this._notificationService.printErrorMessage(
              MessageConstants.CREATED_FAIL_MSG
            );
          }
        });
      });
    });
    

  }

  getFollowId(){
    this._dataService.get("/ServiceFollowing/GetServiceFollow?idService="+this.newId)
      .subscribe((response: any) => {
        this.flId = response;
        if(this.flId.length == 0){
          this.isFollowed = true;
        }
        else{
          this.isFollowed = false;
        }
        
      });

  }

  
  
}
