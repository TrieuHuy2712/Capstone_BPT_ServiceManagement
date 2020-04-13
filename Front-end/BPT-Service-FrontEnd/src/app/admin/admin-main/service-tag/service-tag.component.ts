import { Component, OnInit, ViewChild } from '@angular/core';
import { DataService } from 'src/app/core/services/data.service';
import { MessageConstants } from 'src/app/core/common/message.constants';
import { ModalDirective } from 'ngx-bootstrap';
import { NotificationService } from 'src/app/core/services/notification.service';
import { TranslationService } from 'src/app/core/services/translation.service';
import { Ng4LoadingSpinnerService } from 'ng4-loading-spinner';

@Component({
  selector: 'app-service-tag',
  templateUrl: './service-tag.component.html',
  styleUrls: ['./service-tag.component.css']
})
export class ServiceTagComponent implements OnInit {
  @ViewChild("modalAddEdit", { static: false })
  public modalAddEdit: ModalDirective;
  public pageIndex: number = 1;
  public pageSize: number = 20;
  public pageDisplay: number = 10;
  public totalRow: number;
  public filter: string = "";
  public tags: any[];
  public permission: any;
  public entity: any;
  public functionId: string = "TAG";
  constructor(
    private _dataService: DataService,
    private _notificationService: NotificationService,  
    private spinnerService: Ng4LoadingSpinnerService) {

  }

  ngOnInit() {
    this.permission = {
      canCreate: true,
      canDelete: true,
      canUpdate: true,
      canRead: true
    };
    this.loadData();

  }
  loadData() {
    this.spinnerService.show();
    this._dataService
      .get(
        "/TagManagement/GetAllPaging?page=" +
        this.pageIndex +
        "&pageSize=" +
        this.pageSize +
        "&keyword=" +
        this.filter
      )
      .subscribe((response: any) => {
        this.tags = response.results;
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
  pageChanged(event: any): void {
    this.pageIndex = event.page;
    this.loadData();
  }
  showAddModal() {
    this.entity = {};
    this.modalAddEdit.show();
  }
  loadRole(id: any) {
    let findIdthis = this.tags[id];
    this.entity = findIdthis;
  }
  showEditModal(id: any) {
    this.loadRole(id);
    this.modalAddEdit.show();
  }
  saveChange(valid: boolean) {
    if (valid) {
      this.spinnerService.show();
      if (this.entity.id == undefined) {
        this._dataService.post("/TagManagement/AddNewTag", this.entity).subscribe(
          (response: any) => {
            if (response.isValid == true) {
              this.tags.push(response.myModel);
              this._notificationService.printSuccessMessage(
                MessageConstants.CREATED_OK_MSG
              );
              this.modalAddEdit.hide();
            } else {
              this._notificationService.printErrorMessage(
                MessageConstants.CREATED_FAIL_MSG
              );
            }
            
          },
          error => this._dataService.handleError(error)
        );
      } else {
        this._dataService.put("/TagManagement/updateTag", this.entity).subscribe(
          (response: any) => {
            if (response.isValid == true) {
              let getPostition = this.tags.indexOf(x => x.id == this.entity.id);
              this.tags[getPostition] = response.myModel;
              this.modalAddEdit.hide();
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
      this.spinnerService.hide();
    }
  }
  deleteItem(idTag: any, id: any) {
    this._notificationService.printConfirmationDialog(
      MessageConstants.CONFIRM_DELETE_MSG,
      () => this.deleteItemConfirm(idTag,id)
    );
  }
  deleteItemConfirm(idTag: any,id: any) {
    this.spinnerService.show();
    this._dataService
      .delete("/TagManagement/DeleteTag", "id", idTag)
      .subscribe((response: any) => {
        if (response.isValid == true) {
          this.tags.splice(id, 1);
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
    console.log(id);
    this.pageSize = id;
    this.loadData()

  }
}