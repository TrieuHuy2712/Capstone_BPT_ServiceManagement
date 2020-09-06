import { Component, OnInit, ViewChild } from '@angular/core';
import { DataService } from '../core/services/data.service';
import { NotificationService } from '../core/services/notification.service';
import { UtilityService } from '../core/services/utility.service';
import { UploadService } from '../core/services/upload.service';
import { AuthenService } from '../core/services/authen.service';
import { Router } from '@angular/router';
import { MessageConstants } from '../core/common/message.constants';
import { UrlConstants } from '../core/common/url.constants';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  @ViewChild("avatar", { static: false }) avatar;
  // 

  public users: any[];
  public entity: any;
  public roles: any[];
  public dateOptions: any = {
    locale: { format: "DD/MM/YYYY" },
    alwaysShowCalendars: false,
    singleDatePicker: true
  };
  allRoles: any[];


  constructor(
    private _dataService: DataService,
    private _notificationService: NotificationService,
    private _utilityService: UtilityService,
    private _uploadService: UploadService,
    public _authenService: AuthenService,
    private router: Router
  ) { }

  ngOnInit() {
    this.entity = {};

    // this.loadData();
    // this.loadRoles();

  }

  // loadData() {
  //   this._dataService
  //     .get(
  //       "/UserManagement/GetAllPaging?keyword=&page=1&pageSize=20"
  //     )
  //     .subscribe((response: any) => {
  //       this.users = response.results;
  //     });

  // }

  // loadRoles() {
  //   this._dataService.get("/AdminRole/GetAllPaging?page=1&pageSize=20&keyword=Customer").subscribe((response: any) => {
  //     this.roles = response.results;

  //   });
  //   console.log("ket qua ");
  // }
  //
  saveChange(valid: boolean) {
    if (valid) {

      this.saveData();

    }
  }
  //  
  public saveData() {
    if (this.entity.id == undefined) {
      this._dataService
        .postNoAu("/UserManagement/CreateNewuser", this.entity)
        .subscribe(
          (response: any) => {
            this._notificationService.printSuccessMessage(
              MessageConstants.REGISTER_CREATE_OK_MSG
            );
            this.router.navigate([UrlConstants.LOGIN]);
          },
          error => this._dataService.handleError(error)
        );
    }
  }

  // login

}
