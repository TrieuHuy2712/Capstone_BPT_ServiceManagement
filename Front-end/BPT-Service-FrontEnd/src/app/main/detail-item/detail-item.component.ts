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
  public details: any = [];
  public Uid: string = "";
  public user: LoggedInUser;
  public isFollowed: boolean = false;
  public UIS: any[];
  public usId: any;

  public followEntity: any;
  public followServiceId: any;
  public unfEntity: any;
  public flId: any[];
  locations: any;

  // comment param
  public commentContent: any;
  public commentEntity: any;
  public comments: any[];
  public testComment: any[];
  public child_comment: any[];
  public isAuthor: boolean = false;
  public currenUserId: any;

  // rating param
  public ratingVal: number;
  public ratingEntity: any;
  public myRating: any[];

  constructor(
    private route: ActivatedRoute,
    private _dataService: DataService,
    private _notificationService: NotificationService,
  ) {
  }

  ngOnInit() {

    this.user = JSON.parse(localStorage.getItem(SystemConstants.CURRENT_USER));
    this.followEntity = {};
    this.unfEntity = {};
    this.commentEntity = {};
    this.ratingEntity= {};
    this.newId = this.route.snapshot.paramMap.get("id");
    this.getFollowId();


    this.user = JSON.parse(localStorage.getItem(SystemConstants.CURRENT_USER));
    this.loadData();
    this.loadDataOfLocation();
    this.loadDataOfComment();

    console.log("ket qua "+this.ratingVal);
    this.getCurrentUserId();
    console.log(this.currenUserId);
    
    
    

  }


  sliderConfigDetail = { "slidesToShow": 3, "slidesToScroll": 1, "arrows": true, "autoplay": false };
  afterChange(e) {
    console.log('afterChange');
  }

  // load data
  loadData() {
    this._dataService.get("/Service/getPostServiceById?idService=" + this.newId)
      .subscribe((response: any) => {
        this.details = response;
        this.Uid = response.userId;
        this.details.userId = "";
        console.log(this.details);
      });
  }

  // follow a service

  followAService() {
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
              MessageConstants.FOLLOW_OK_MSG

            );
            // this.isFollowed = !this.isFollowed;
            this.getFollowId();
          } else {
            this._notificationService.printErrorMessage(
              MessageConstants.FOLLOW_FAIL_MSG
            );
          }
        });
    });

  }

  unFollowAService() {
    this._dataService.get("/UserManagement/GetAllUser").subscribe((response: any) => {
      this.UIS = response;
      this.usId = this.UIS.find(x => x.fullName == this.user.fullName).id;
      this._dataService.get("/ServiceFollowing/GetServiceFollow?idService=" + this.newId)
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
                  MessageConstants.UNFOLLOW_OK_MSG
                );
                // this.isFollowed = !this.isFollowed;
                this.getFollowId();

              } else {
                this._notificationService.printErrorMessage(
                  MessageConstants.UNFOLLOW_FAIL_MSG
                );
              }
            });
        });
    });


  }

  getFollowId() {
    this._dataService.get("/ServiceFollowing/GetServiceFollow?idService=" + this.newId)
      .subscribe((response: any) => {
        this.flId = response;
        if (this.flId.length == 0) {
          this.isFollowed = true;
        }
        else {
          this.isFollowed = false;
        }

      });

  }
  // get location
  loadDataOfLocation() {
    this._dataService
      .get(
        "/LocationManagement/GetAllLocation"
      )
      .subscribe((response: any) => {
        this.locations = response;
        console.log(this.locations);
      });
  }

  // add a comment in a service
  postAComment(mes: string) {
    this.commentEntity.contentOfRating = mes;
    this._dataService.get("/UserManagement/GetAllUser").subscribe((response: any) => {
      this.UIS = response;
      this.usId = this.UIS.find(x => x.fullName == this.user.fullName).id;
      this.commentEntity.userId = this.usId;
      this.commentEntity.serviceId = this.newId;

      this._dataService
        .post(
          "/CommentManagement/addNewComment", this.commentEntity
        )
        .subscribe((response: any) => {
          this.testComment.push(response.myModel);
          console.log(this.testComment);
          this.loadDataOfComment();
        });
    });

  }

  // load data of comment of a service

  loadDataOfComment(){
    this._dataService
      .get(
        "/CommentManagement/getComment?id="+this.newId
      )
      .subscribe((response: any) => {
        this.testComment = response;
        this.testComment.reverse();
        for(let x = 0; x < response.length; x++){
          if(response[x] != null){
            this.child_comment.push(response.listVm);
          }
        }
        console.log(this.child_comment);
      });
  }


  // delete comment
  deleteComment(id: any) {
    this._notificationService.printConfirmationDialog(
      MessageConstants.CONFIRM_DELETE_COMMENT_MSG,
      () => this.deleteCommentConfirm(id)
    );
  }
  deleteCommentConfirm( id: any) {
    this._dataService
      .delete("/CommentManagement/DeleteComment","id",id)
      .subscribe((response: any) => {
        if (response.isValid == true) {
          this._notificationService.printSuccessMessage(
            MessageConstants.COMMENT_DEL_OK_MSG
          );
          this.loadDataOfComment();
        } else {
          this._notificationService.printErrorMessage(
            MessageConstants.COMMENT_DEL_FAIL_MSG
          );
        }
      });
  }
  // confirm author of that comment
  // confirmAuthorOfComment(){
  //   this._dataService.get("/UserManagement/GetAllUser").subscribe((response: any) => {
  //     this.UIS = response;
  //     this.usId = this.UIS.find(x => x.fullName == this.user.fullName).id;
  //     this._dataService
  //       .post(
  //         "/CommentManagement/addNewComment", this.commentEntity
  //       )
  //       .subscribe((response: any) => {
  //         this.testComment.push(response.myModel);
  //         console.log(this.testComment);
  //         this.loadDataOfComment();
  //       });
  //   });
  // }
  getCurrentUserId(){
    this._dataService.get("/UserManagement/GetAllUser").subscribe((response: any) => {
      this.UIS = response;
      this.currenUserId = this.UIS.find(x => x.fullName == this.user.fullName).id;
      
    });
    return this.currenUserId;
  }

  // get rating value
  getContentOfRating(val:number){
    this.ratingVal = val;
    this.ratingAService();
  }

  ratingAService(){
    this._dataService.get("/UserManagement/GetAllUser").subscribe((response: any) => {
      this.UIS = response;
      this.usId = this.UIS.find(x => x.fullName == this.user.fullName).id;
      // this.ratingEntity.userId = this.usId;
      // this.ratingEntity.serviceId = this.newId;
      // this.ratingEntity.numberOfRating = this.ratingVal;
      this._dataService
        .post(
          "/RatingService/AddUpdateRating?userId="+this.usId+"&ServiceId="+this.newId+"&NumberOfRating="+this.ratingVal
        )
        .subscribe((response: any) => {
          this.myRating.push(response.myModel);
          console.log(this.myRating);
        });
    });
  }

}
