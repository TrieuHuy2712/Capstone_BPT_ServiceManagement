import { Component, OnInit, ViewChild } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { LoggedInUser } from 'src/app/core/domain/loggedin.user';
import { SystemConstants } from 'src/app/core/common/system,constants';
import { DataService } from 'src/app/core/services/data.service';
import { NotificationService } from 'src/app/core/services/notification.service';
import { UploadService } from 'src/app/core/services/upload.service';
import { MessageConstants } from 'src/app/core/common/message.constants';

@Component({
  selector: 'app-user-profile',
  templateUrl: './user-profile.component.html',
  styleUrls: ['./user-profile.component.css']
})
export class UserProfileComponent implements OnInit {
  @ViewChild(ModalDirective, { static: false })
  public modalAddEdit: ModalDirective;
  @ViewChild("modalEditProfile", { static: false })
  public modalEditProfile: ModalDirective;
  public editProfile: ModalDirective;
  public entity: any;
  public profile: LoggedInUser;
  public id: string;
  @ViewChild("modalReason", { static: false })
  public modalReason: ModalDirective;
  @ViewChild('avatarPath', { static: false }) avatarPath;
  public pageIndex: number = 1;
  public pageSize: number = 0;
  public pageDisplay: number = 10;
  public defaultStatus: number = 5;
  public totalRow: number;
  public filter: string = "";
  public provider: any[];
  public permission: any;
  public location: any[];
  public users: any;
  public state: any[];
  public locationState: any[];
  public reject: any;
  public services: any;
  public detailUSers: any[];
  public userEntity: any;

  constructor(
    private _dataService: DataService,
    private _notificationService: NotificationService,
    private _uploadService: UploadService
  ) { }

  ngOnInit() {
    this.entity = {};
    this.userEntity = {};
    this.profile = JSON.parse(localStorage.getItem(SystemConstants.CURRENT_USER));
    SystemConstants.const_permission = this.profile.username;
    if (this.profile.avatar == null) {
      this.profile.avatar = "../../../../assets/images/default.png";
    }
    this.userEntity = this.profile;
    this.getAllLocation();
    this.getAllUser();

  }
  showAddModal() {
    this.entity = {};
    this.modalAddEdit.show();
    this.getAllLocation();
    this.getAllUser();
  }

  showEditProfile() {
    this.modalEditProfile.show();
  }

  // register to become a provider
  saveChange(valid: boolean) {
    if (valid) {
      let fi = this.avatarPath.nativeElement;
      if (fi.files.length > 0) {
        this._uploadService.postWithFile('/UploadImage/saveImage/category', null, fi.files)
          .then((imageUrl: any) => {
            this.entity.avatarPath = imageUrl;
          }).then(() => {
            this.saveData();
          });
      }
      else {
        this.saveData();
      }
    }
  }

  saveData() {
    if (this.entity.id == undefined) {
      let getUser = this.users.findIndex(x => x.userName == this.entity.userName);
      this.entity.userId = this.users[getUser].id;
      let getCityId = this.location.findIndex(x => x.city + "_" + x.province == this.entity.cityName);
      this.entity.cityId = this.location[getCityId].id;
      this._dataService.post("/Provider/RegisterProvider", this.entity).subscribe(
        (response: any) => {
          if (response.isValid == true) {
            this._notificationService.printSuccessMessage("Thông tin của bạn đã được gửi đến nhà quản trị để xác thực, vui lòng kiểm tra email !!!");
            this.modalAddEdit.hide();
          } else {
            this._notificationService.printErrorMessage(
              "Thông tin chưa được gửi, vui lòng kiểm tra lại thông tin !!!"
            );
          }
        },
        error => this._dataService.handleError(error)
      );
    }
  }


  // sava new data of user

  saveEditUser(valid: boolean) {
    if (valid) {
      this.saveUserData();
    }
  }

  public saveUserData() {
    this._dataService
      .put("/UserManagement/UpdateUser", this.userEntity)
      .subscribe(
        (response: any) => {
          if (response.isValid == true) {
            this.modalEditProfile.hide();
            this._notificationService.printSuccessMessage(
              MessageConstants.UPDATED_OK_MSG
            );
          } else {
            this._notificationService.printErrorMessage(
              MessageConstants.UPDATED_FAIL_MSG
            );
          }
        },
        error => this._dataService.handleError(error)
      );
  }

  getAllLocation() {
    this._dataService.get("/LocationManagement/GetAllLocation").subscribe((response: any) => {
      this.location = response;
      let allLocation = new Array();
      this.location.forEach(el => {
        allLocation.push(el.city + "_" + el.province);
      });
      this.locationState = allLocation;
    });
  }

  // get all user
  getAllUser() {
    this._dataService.get("/UserManagement/GetAllUser").subscribe((response: any) => {
      this.users = response;
      let userName = new Array();
      this.users.forEach(element => {
        userName.push(element.userName);
      });
      this.state = userName;
    });
  }

  // load user detail
  loadUserDetail(id: any) {
    debugger;
    let findIdthis = this.users[id];
    this.entity = findIdthis;
    this.userEntity = {};

  }

  checkUserName(userName: any) {
    if (userName.pristine) {
      return true;
    }
    let getUserName = this.state.find(x => x == userName.value);
    if (getUserName == null) {
      return false;
    };
    return true
  }
  checkLocation(location: any) {
    if (location.pristine) {
      return true;
    }
    let locationName = this.locationState.find(x => x == location.value);
    if (locationName == null) {
      return false;
    };
    return true
  }


}
