import { Component, OnInit, ViewChild } from '@angular/core';
import { DataService } from '../../../core/services/data.service';
import { NotificationService } from '../../../core/services/notification.service';
import { Ng4LoadingSpinnerService } from 'ng4-loading-spinner';
import { MessageConstants } from '../../../core/common/message.constants';
import { ModalDirective } from 'ngx-bootstrap/modal';

@Component({
  selector: 'app-admin-email',
  templateUrl: './admin-email.component.html',
})
export class AdminEmailComponent implements OnInit {

  @ViewChild("modalAddEdit", { static: false })
  public modalAddEdit: ModalDirective;
  public pageIndex: number = 1;
  public pageSize: number = 0;
  public pageDisplay: number = 10;
  public totalRow: number;
  public filter: string = "";
  public emails: any[];
  public permission: any;
  public entity: any;
  public functionId: string = "EMAIL";
  constructor(
    private _dataService: DataService,
    private _notificationService: NotificationService,
    private spinnerService: Ng4LoadingSpinnerService
  ) { }

  ngOnInit() {
    this.permission = {
      canCreate: false,
      canDelete: false,
      canUpdate: false,
      canRead: false
    };
    this.loadData();
  }
  loadData() {
    this.spinnerService.show();
    this._dataService
      .get(
        "/EmailManagement/GetAllPaging?page=" +
        this.pageIndex +
        "&pageSize=" +
        this.pageSize +
        "&keyword=" +
        this.filter
      )
      .subscribe((response: any) => {
        this.emails = response.results;
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
        "/PermissionManager/GetAllPermission/" + this.functionId
      )
      .subscribe((response: any) => {
        this.permission = response;
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
  loadRole(id: any) {
    let findIdthis = this.emails[id];
    this.entity = findIdthis;
  }
  showEditModal(id: any) {
    this.loadRole(id);
    this.modalAddEdit.show();
  }
  saveChange(valid: boolean) {
    if (valid) {
      this.spinnerService.show();
      if (this.entity.id === undefined) {
        this._dataService.post("/EmailManagement/AddNewEmail", this.entity).subscribe(
          (response: any) => {
            if (response.isValid == true) {
              this.emails.push(response.myModel);
              this._notificationService.printSuccessMessage(
                MessageConstants.CREATED_OK_MSG
              );
              this.modalAddEdit.hide();
            } else {
              this._notificationService.printErrorMessage(
                MessageConstants.CREATED_FAIL_MSG
              );
            }
            this.spinnerService.hide();

          },
          error => this._dataService.handleError(error)
        );
      } else {
        this._dataService.put("/EmailManagement/UpdateEmail", this.entity).subscribe(
          (response: any) => {
            if (response.isValid == true) {
              let getPostition = this.emails.indexOf(x => x.id == this.entity.id);
              this.emails[getPostition] = response.myModel;
              this.modalAddEdit.hide();
              this._notificationService.printSuccessMessage(
                MessageConstants.UPDATED_OK_MSG
              );
            } else {
              this._notificationService.printErrorMessage(
                MessageConstants.UPDATED_FAIL_MSG
              );
              this.spinnerService.hide()
            }

          },
          error => this._dataService.handleError(error)
        );
      }
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
      .delete("/EmailManagement/DeleteEmail", "id", idRole)
      .subscribe((response: any) => {
        if (response.isValid == true) {
          this.emails.splice(id, 1);
          this._notificationService.printSuccessMessage(
            MessageConstants.DELETED_OK_MSG
          );
        } else {
          this._notificationService.printErrorMessage(
            MessageConstants.DELETED_FAIL_MSG
          );
        }
        this.spinnerService.hide();
      });
  }
  filterChanged(id: any) {
    this.pageSize = id;
    this.loadData()

  }
}
