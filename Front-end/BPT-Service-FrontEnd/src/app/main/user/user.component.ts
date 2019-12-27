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

declare var moment: any;

@Component({
  selector: "app-user",
  templateUrl: "./user.component.html",
  styleUrls: ["./user.component.css"]
})
export class UserComponent implements OnInit {
  @ViewChild("modalAddEdit", { static: false })
  public modalAddEdit: ModalDirective;
  @ViewChild("avatar", { static: false }) avatar;
  public myRoles: string[] = [];
  public currentRole:string[]=[];
  public pageIndex: number = 1;
  public pageSize: number = 20;
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
    private router: Router
  ) {
  }

  ngOnInit() {
    this.currentUser = SystemConstants.CURRENT_USER;
    this.permission =
    {
      canCreate: true,
      canDelete: true,
      canUpdate: true,
      canRead: true
    };
    this.loadRoles();
    this.loadData();
  }

  loadData() {
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
        this.pageIndex = response.PageIndex;
        this.pageSize = response.PageSize;
        this.totalRow = response.TotalRows;
        console.log(response);
        if (localStorage.getItem(SystemConstants.const_username) != "admin") {
          this.loadPermission();
        } else {
          let adminPermission: any = {
            canCreate: true,
            canDelete: true,
            canUpdate: true,
            canRead: true
          };
          this.permission = adminPermission;
        }
      });
  }
  loadPermission() {
    this._dataService
      .get(
        "/PermissionManager/GetAllPermission/" +
        localStorage.getItem(SystemConstants.const_username) +
        "/" +
        this.functionId
      )
      .subscribe((response: any) => {
        console.log(response);
        this.permission = response.result;
        console.log(this.permission);
      });
  }

  loadRoles() {
    this._dataService.get("/AdminRole/GetAll").subscribe(
      (response: any[]) => {
        this.allRoles = [];
        for (let role of response) {
          this.allRoles.push({ id: role.name, name: role.description });
        }
        console.log(response);
      },
      error => this._dataService.handleError(error)
    );
  }
  loadUserDetail(id: any) {
    this._dataService
      .get("/UserManagement/GetById/" + id)
      .subscribe((response: any) => {
        console.log(response);
        this.entity = response;
        this.myRoles = [];
        for (let role of this.entity.roles) {
          this.myRoles.push(role);
          this.currentRole.push(role);
        }
        this.entity.birthDay = moment(new Date(this.entity.birthDay)).format(
          "DD/MM/YYYY"
        );
      });
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
      this.entity.NewRoles = this.myRoles;
      console.log(this.myRoles);
      let fi = this.avatar.nativeElement;
      if (fi.files.length > 0) {
        this._uploadService
          .postWithFile("/api/upload/saveImage?type=avatar", null, fi.files)
          .then((imageUrl: string) => {
            this.entity.Avatar = imageUrl;
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
      this._dataService.post("/UserManagement/AddNewUser", this.entity).subscribe(
        (response: any) => {
          this.loadData();
          this.modalAddEdit.hide();
          this._notificationService.printSuccessMessage(
            MessageConstants.CREATED_OK_MSG
          );
        },
        error => this._dataService.handleError(error)
      );
    } else {
      this._dataService.put("/UserManagement/UpdateUser", this.entity).subscribe(
        (response: any) => {
          this.modalAddEdit.hide();
          this._notificationService.printSuccessMessage(
            MessageConstants.UPDATED_OK_MSG
          );
          this.loadData();
        },
        error => this._dataService.handleError(error)
      );
    }
  }
  deleteItem(id: any) {
    this._notificationService.printConfirmationDialog(
      MessageConstants.CONFIRM_DELETE_MSG,
      () => this.deleteItemConfirm(id)
    );
  }
  deleteItemConfirm(id: any) {
    this._dataService
      .delete("/UserManagement/DeleteUser", "id", id)
      .subscribe((response: Response) => {
        this._notificationService.printSuccessMessage(
          MessageConstants.DELETED_OK_MSG
        );
        this.loadData();
      });
  }
  public selectGender(event) {
    this.entity.Gender = event.target.value;
  }

  public selectedDate(value: any) {
    this.entity.BirthDay = moment(value.end._d).format("DD/MM/YYYY");
  }
}
