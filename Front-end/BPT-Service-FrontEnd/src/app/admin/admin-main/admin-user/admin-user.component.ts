import { Component, OnInit, ViewChild } from "@angular/core";
import {
  IMultiSelectOption,
  IMultiSelectTexts
} from "angular-2-dropdown-multiselect";

import { AuthenService } from "src/app/core/services/authen.service";
import { DataService } from "src/app/core/services/data.service";
import { MessageConstants } from "src/app/core/common/message.constants";
import { ModalDirective } from "ngx-bootstrap/modal";
import { NotificationService } from "src/app/core/services/notification.service";
import { Router } from "@angular/router";
import { SystemConstants } from "src/app/core/common/system,constants";
import { UploadService } from "src/app/core/services/upload.service";
import { UrlConstants } from "src/app/core/common/url.constants";
import { UtilityService } from "src/app/core/services/utility.service";
import { Ng4LoadingSpinnerService } from 'ng4-loading-spinner';

declare var moment: any;

@Component({
  selector: "app-user",
  templateUrl: "./admin-user.component.html",
})
export class UserComponent implements OnInit {
  @ViewChild("modalAddEdit", { static: false })
  public modalAddEdit: ModalDirective;
  @ViewChild("avatar", { static: false }) avatar;
  public myRoles: string[] = [];
  public currentRole: string[] = [];
  public pageIndex: number = 1;
  public pageSize: number = 0;
  public pageDisplay: number = 10;
  public totalRow: number;
  public filter: string = "";
  public users: any[];
  public entity: any;
  public baseFolder: string = SystemConstants.BASE_API;
  public allRoles: IMultiSelectOption[] = [];
  public permission: any;
  public roles: any[];
  public currentUser: string;
  public currentPOS: number;
  public functionId: string = "USER";
  public dateOptions: any = {
    locale: { format: "DD/MM/YYYY" },
    alwaysShowCalendars: false,
    singleDatePicker: true
  };

  constructor(
    private _dataService: DataService,
    private _notificationService: NotificationService,
    private _utilityService: UtilityService,
    private _uploadService: UploadService,
    public _authenService: AuthenService,
    private router: Router,
    private spinnerService: Ng4LoadingSpinnerService
  ) { }

  ngOnInit() {
    this.currentUser = SystemConstants.CURRENT_USER;
    this.permission = {
      canCreate: false,
      canDelete: false,
      canUpdate: false,
      canRead: false
    };
    this.loadRoles();
    this.loadData();
  }

  loadData() {
    this.spinnerService.show();
    this._dataService
      .get(
        "/UserManagement/GetAllPaging?page=" +
        this.pageIndex +
        "&pageSize=" +
        this.pageSize +
        "&keyword=" +
        this.filter
      )
      .subscribe((response: any) => {
        this.users = response.results;
        this.pageIndex = response.currentPage;
        this.pageSize = response.pageSize;
        this.totalRow = response.rowCount;
        this.loadPermission();
        this.spinnerService.hide();
      });
  }
  loadPermission() {
    this._dataService
      .get(
        "/PermissionManager/GetAllPermission/" +
        this.functionId
      )
      .subscribe((response: any) => {
        this.permission = response;
      });
  }

  loadRoles() {
    this._dataService.get("/AdminRole/GetAll").subscribe(
      (response: any[]) => {
        this.allRoles = [];
        for (let role of response) {
          this.allRoles.push({ id: role.name, name: role.description });
        }
      },
      error => this._dataService.handleError(error)
    );
  }
  loadUserDetail(id: any) {
    let findIdthis = this.users[id];
    this.entity = findIdthis;
    for (let role of this.entity.roles) {
      this.myRoles.push(role);
      this.currentRole.push(role);
    }
  }
  pageChanged(event: any): void {
    this.pageIndex = event.page;
    this.loadData();
  }
  showAddModal() {
    this.entity = {};
    this.modalAddEdit.show();
  }
  showEditModal(id: any) {
    this.loadUserDetail(id);
    this.modalAddEdit.show();
  }
  saveChange(valid: boolean) {
    if (valid) {
      this.spinnerService.show();
      this.entity.NewRoles = this.myRoles;
      let fi = this.avatar.nativeElement;
      if (fi.files.length > 0) {
        this._uploadService
          .postWithFile("/UploadImage/saveImage/avatar", null, fi.files)
          .then((imageUrl: string) => {
            this.entity.avatar = imageUrl;
          })
          .then(() => {
            this.saveData();
          });
      } else {
        this.saveData();
      }
    }
  }
  public saveData() {
    if (this.entity.id == undefined) {
      this._dataService
        .post("/UserManagement/AddNewUser", this.entity)
        .subscribe(
          (response: any) => {
            if (response.isValid == true) {
              this.users.push(response.myModel);
              this.modalAddEdit.hide();
              this._notificationService.printSuccessMessage(
                MessageConstants.CREATED_OK_MSG
              );
              this.spinnerService.hide();
            } else {
              this._notificationService.printErrorMessage(
                MessageConstants.CREATED_FAIL_MSG
              );
              this.spinnerService.hide();
            }
          },
          error => this._dataService.handleError(error)
        );
    } else {
      this._dataService
        .put("/UserManagement/UpdateUser", this.entity)
        .subscribe(
          (response: any) => {
            if (response.isValid == true) {
              this.modalAddEdit.hide();
              let getPostition = this.users.indexOf(x => x.id == this.entity.id);
              this.users[getPostition] = response.myModel;
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
  }
  deleteItem(idRole: any, id: any) {
    this._notificationService.printConfirmationDialog(
      MessageConstants.CONFIRM_DELETE_MSG,
      () => this.deleteItemConfirm(idRole, id)
    );
  }
  deleteItemConfirm(idRole: any, id: any) {
    this.spinnerService.show();
    this._dataService
      .delete("/UserManagement/DeleteUser", "id", idRole)
      .subscribe((response: any) => {
        if (response.isValid == true) {
          this.users.splice(id, 1);
          this._notificationService.printSuccessMessage(
            MessageConstants.DELETED_OK_MSG
          );
          this.spinnerService.hide();
        } else {
          this._notificationService.printErrorMessage(
            MessageConstants.DELETED_FAIL_MSG
          );
          this.spinnerService.hide();
        }
      });
      
  }
  public selectGender(event) {
    this.entity.Gender = event.target.value;
  }
  filterChanged(id: any) {
    this.pageSize = id;
    this.loadData()
  }
}
