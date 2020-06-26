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
  public isFollowed: boolean;
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
  public isAuthor: boolean;
  public currenUserId: any;
  public emptyMess: string;

  // rating param
  public ratingVal: number;
  public ratingEntity: any;
  public myRating: any[];
  public averageRatingOfAService: any;
  public currentRating: any[];
  public isRated: boolean = true;
  public canDeleteComment: boolean = false;

  // provider param
  public provider: any;
  public providerId: any;

  // system param
  public isAvailable: boolean = true;


  // user param
  public isLoggin: boolean = true;
  
  constructor(
    private route: ActivatedRoute,
    private _dataService: DataService,
    private _notificationService: NotificationService,
  ) {
  }

  ngOnInit() {
    // check if user loggin or not?
    this.isUserLogging();

    this.followEntity = {};
    this.unfEntity = {};
    this.commentEntity = {};
    this.ratingEntity = {};
    this.newId = this.route.snapshot.paramMap.get("id");
    this.checkFollowService();

    this.user = JSON.parse(localStorage.getItem(SystemConstants.CURRENT_USER));

    if(this.user !== null){
      if (this.user.avatar == null) {
        this.user.avatar = "../../../../assets/images/default.png";
      }
    }
    this.loadData();
    this.loadDataOfLocation();
    this.loadDataOfComment();

    console.log("ket qua " + this.ratingVal);
    this.getCurrentUserId();
    console.log(this.currenUserId);
    this.averageRatingContent();
    this.getInfoOfProvider();

  }


  sliderConfigDetail = { "slidesToShow": 3, "slidesToScroll": 1, "arrows": true, "autoplay": false };
  afterChange(e) {
    console.log('afterChange');
  }

  // load data
  loadData() {
    this._dataService.getNoAu("/Service/getPostServiceById?idService=" + this.newId)
      .subscribe((response: any) => {
        this.details = response;
        this.Uid = response.userId;
        this.details.userId = '';
        if(localStorage.getItem(SystemConstants.CURRENT_USER) != null){
          this.saveLogService(response.id);
        }
        if (response.id === undefined) {
          this.isAvailable = false;
        }
        else {
          this.isAvailable = true;
        }
      });
  }
  // Save seen detail
    saveLogService(idService: any) {
        this._dataService.post('/Recommendation/ViewService', idService).subscribe((response: any) => {
        });
    }

  isUserLogging(){
    this.user = JSON.parse(localStorage.getItem(SystemConstants.CURRENT_USER));
    if(this.user == null){
      this.isLoggin = false;
    }
  }

  // follow a service

  followAService() {
    this._dataService.getNoAu("/UserManagement/GetAllUser").subscribe((response: any) => {
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
            this.isFollowed = !this.isFollowed;
          } else {
            this._notificationService.printErrorMessage(
              MessageConstants.FOLLOW_FAIL_MSG
            );
          }
          this.checkFollowService();
        });
    });

  }

  unFollowAService() {
    this._dataService.getNoAu("/UserManagement/GetAllUser").subscribe((response: any) => {
      this.UIS = response;
      this.usId = this.UIS.find(x => x.fullName == this.user.fullName).id;
      this._dataService.getNoAu("/ServiceFollowing/GetServiceFollow?idService=" + this.newId)
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
                this.isFollowed = !this.isFollowed;

              } else {
                this._notificationService.printErrorMessage(
                  MessageConstants.UNFOLLOW_FAIL_MSG
                );
              }
              this.checkFollowService();
            });
        });
    });
  }
  // function check if u follow a service or not
  checkFollowService() {
    // get current user id
    this._dataService.getNoAu("/UserManagement/GetAllUser").subscribe((response: any) => {
      this.UIS = response;
      this.currenUserId = this.UIS.find(x => x.fullName == this.user.fullName).id;
      // get all user follow that service
      this._dataService.getNoAu("/ServiceFollowing/GetServiceFollow?idService=" + this.newId)
        .subscribe((response: any) => {
          this.flId = response;
          let checkId = this.flId.find(x => x.userId == this.currenUserId).userId;
          console.log("check id = " + checkId);

          if (checkId == this.currenUserId) {
            this.isFollowed = true;
            console.log("isFollow = " + this.isFollowed);
          }
          else if (checkId == undefined) {
            this.isFollowed = false;
          }
          else {
            this.isFollowed = false;
            console.log("isFollow = " + this.isFollowed);

          }

        });
    });
  }

  // transfer html  to text


  // get location
  loadDataOfLocation() {
    this._dataService
      .getNoAu(
        "/LocationManagement/GetAllLocation"
      )
      .subscribe((response: any) => {
        this.locations = response;
        console.log(this.locations);
      });
  }

  // add a comment in a service
  postAComment(mes) {
    this.commentEntity.contentOfRating = mes;
    this._dataService.getNoAu("/UserManagement/GetAllUser").subscribe((response: any) => {
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
          this.emptyMess = " ";

        });
    });

  }

  // load data of comment of a service

  loadDataOfComment() {
    this._dataService
      .getNoAu(
        "/CommentManagement/getComment?id=" + this.newId
      )
      .subscribe((response: any) => {
        for (let x = 0; x < response.length; x++) {
          if (response[x].avatarPath == null) {
            response[x].avatarPath == "../../../../assets/images/default.png";
            
          }
          this.testComment = response;
          this.testComment.reverse();
        }

      });
  }


  // delete comment
  deleteComment(id: any) {
    this._notificationService.printConfirmationDialog(
      MessageConstants.CONFIRM_DELETE_COMMENT_MSG,
      () => this.deleteCommentConfirm(id)
    );
  }
  deleteCommentConfirm(id: any) {
    this._dataService
      .delete("/CommentManagement/DeleteComment", "id", id)
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
  //   this._dataService.getNoAu("/UserManagement/GetAllUser").subscribe((response: any) => {
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
  getCurrentUserId() {
    this._dataService.getNoAu("/UserManagement/GetAllUser").subscribe((response: any) => {
      this.UIS = response;
      this.currenUserId = this.UIS.find(x => x.fullName == this.user.fullName).id;

    });
    return this.currenUserId;
  }

  // get rating value
  getContentOfRating(val: number) {
    this.ratingVal = val;
    this.ratingAService();
  }

  ratingAService() {
    this._dataService.getNoAu("/UserManagement/GetAllUser").subscribe((response: any) => {
      this.UIS = response;
      this.usId = this.UIS.find(x => x.fullName == this.user.fullName).id;
      // this.ratingEntity.userId = this.usId;
      // this.ratingEntity.serviceId = this.newId;
      // this.ratingEntity.numberOfRating = this.ratingVal;
      this._dataService
        .post(
          "/RatingService/AddUpdateRating?userId=" + this.usId + "&ServiceId=" + this.newId + "&NumberOfRating=" + this.ratingVal
        )
        .subscribe((response: any) => {
          if (response.isValid == true) {
            this._notificationService.printSuccessMessage(
              MessageConstants.RATING_OK_MSG
            );
          } else {
            this._notificationService.printErrorMessage(
              MessageConstants.RATING_FAIL_MSG
            );
          }

        });
    });
  }
  // Average rating of a service
  averageRatingContent() {
    this._dataService.getNoAu("RatingService/GetRatingByService?idService=" + this.newId)
      .subscribe((response: any) => {
        this.averageRatingOfAService = response;
        this.currentRating = response.listRating;
        let currentUserRating = this.currentRating.find(x => x.userNameWithEmail == this.user.email).numberOfRating;
        console.log("ket qua = " + currentUserRating);
        if (currentUserRating !== undefined) {
          this.isRated = !this.isRated;
        }
      });
  }

  // infor about provider
  getInfoOfProvider() {
    this._dataService.getNoAu("/Service/getPostServiceById?idService=" + this.newId)
      .subscribe((response: any) => {
        this.providerId = response.providerId;
        console.log("Provider id = " + this.providerId);

        this._dataService.getNoAu("/Provider/GetProviderById/" + this.providerId)
          .subscribe((response: any) => {
            this.provider = response.myModel;
            console.log("provider " + this.provider);

          });
      });

  }

}
