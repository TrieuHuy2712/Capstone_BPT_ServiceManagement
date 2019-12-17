import { Component, OnInit, ViewChild } from "@angular/core";
import { ModalDirective } from "ngx-bootstrap/modal";
import { Router } from "@angular/router";
import {
  IMultiSelectOption,
  IMultiSelectTexts
} from "angular-2-dropdown-multiselect";
import { SystemConstants } from 'src/app/core/common/system,constants';
import { DataService } from 'src/app/core/services/data.service';
import { NotificationService } from 'src/app/core/services/notification.service';
import { UtilityService } from 'src/app/core/services/utility.service';
import { UploadService } from 'src/app/core/services/upload.service';
import { AuthenService } from 'src/app/core/services/authen.service';
import { MessageConstants } from 'src/app/core/common/message.constants';
import { UrlConstants } from 'src/app/core/common/url.constants';
declare var moment: any;

@Component({
  selector: "app-user",
  templateUrl: "./user.component.html",
  styleUrls: ["./user.component.css"]
})
export class UserComponent implements OnInit {
  @ViewChild("modalAddEdit",{static: false}) public modalAddEdit: ModalDirective;
  @ViewChild("avatar",{static: false}) avatar;
  public myRoles: string[] = [];
  public pageIndex: number = 1;
  public pageSize: number = 20;
  public pageDisplay: number = 10;
  public totalRow: number;
  public filter: string = "";
  public users: any[];
  public entity: any;
  public baseFolder: string = SystemConstants.BASE_API;
  public allRoles: IMultiSelectOption[] = [];
  public roles: any[];
  public currentUser: string;
  public currentPOS: number;
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
    if (_authenService.checkAccess("USER") == false) {
      this._notificationService.printErrorMessage(
        MessageConstants.PERMISSION_OK_MSG
      );
      this.router.navigate([UrlConstants.HOME]);
    }
  }

  ngOnInit() {
    this.currentUser = SystemConstants.CURRENT_USER;
    this.loadRoles();
    this.loadData();
  }

  loadData() {
    this._dataService
      .get(
        "/api/appUser/getlistpaging?page=" +
          this.pageIndex +
          "&pageSize=" +
          this.pageSize +
          "&filter=" +
          this.filter
      )
      .subscribe((response: any) => {
        this.users = response.Items;
        this.pageIndex = response.PageIndex;
        this.pageSize = response.PageSize;
        this.totalRow = response.TotalRows;
      });
  }
  loadRoles() {
    this._dataService.get("/api/appRole/getlistall").subscribe(
      (response: any[]) => {
        this.allRoles = [];
        for (let role of response) {
          this.allRoles.push({ id: role.Name, name: role.Description });
        }
      },
      error => this._dataService.handleError(error)
    );
  }
  loadUserDetail(id: any) {
    this._dataService
      .get("/api/appUser/detail/" + id)
      .subscribe((response: any) => {
        this.entity = response;
        this.myRoles = [];
        for (let role of this.entity.Roles) {
          this.myRoles.push(role);
        }
        this.entity.BirthDay = moment(new Date(this.entity.BirthDay)).format(
          "DD/MM/YYYY"
        );
        console.log(this.allRoles);
        console.log(this.myRoles);
        console.log(this.entity);
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
      this.entity.Roles = this.myRoles;
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
  private saveData() {
    if (this.entity.Id == undefined) {
      if (this.currentPOS != 0) {
        this.entity.POS = this.currentPOS;
      }
      this.entity.BonusPoint = 0;
      this._dataService.post("/api/appUser/add", this.entity).subscribe(
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
      this._dataService.put("/api/appUser/update", this.entity).subscribe(
        (response: any) => {
          this.loadData();
          this.modalAddEdit.hide();
          this._notificationService.printSuccessMessage(
            MessageConstants.UPDATED_OK_MSG
          );
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
      .delete("/api/appUser/delete", "id", id)
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
