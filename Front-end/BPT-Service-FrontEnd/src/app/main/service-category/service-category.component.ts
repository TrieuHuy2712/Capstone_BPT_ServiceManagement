import { Component, OnInit, ViewChild } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { DataService } from 'src/app/core/services/data.service';
import { NotificationService } from 'src/app/core/services/notification.service';
import { SystemConstants } from 'src/app/core/common/system,constants';
import { MessageConstants } from 'src/app/core/common/message.constants';
import { UploadService } from 'src/app/core/services/upload.service';

@Component({
  selector: 'app-service-category',
  templateUrl: './service-category.component.html',
  styleUrls: ['./service-category.component.css']
})
export class ServiceCategoryComponent implements OnInit {

  @ViewChild("modalAddEdit", { static: false })
  public modalAddEdit: ModalDirective;
  @ViewChild('avatar', {static: false}) avatar
  public pageIndex: number = 1;
  public pageSize: number = 20;
  public pageDisplay: number = 10;
  public totalRow: number;
  public filter: string = "";
  public categories: any[];
  public permission: any;
  public entity: any;
  public functionId: string = "CATEGORY";
  constructor(
    private _dataService: DataService,
    private _uploadService: UploadService,
    private _notificationService: NotificationService
  ) {

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
    this._dataService
      .get(
        "/CategoryManagement/GetAllPaging?page=" +
        this.pageIndex +
        "&pageSize=" +
        this.pageSize +
        "&keyword=" +
        this.filter
      )
      .subscribe((response: any) => {
        this.categories = response.results;
        this.pageIndex = response.currentPage;
        this.pageSize = response.pageSize;
        this.totalRow = response.rowCount;
          this.loadPermission();
      });
  }
  loadPermission() {
    this._dataService
      .get(
        "/PermissionManager/GetAllPermission/" +
        "/" +
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
    this._dataService
      .get("/CategoryManagement/GetCatagoryById?id=" + id)
      .subscribe((response: any) => {
        this.entity = response;
      });
  }
  showEditModal(id: any) {
    this.loadRole(id);
    this.modalAddEdit.show();
  }
  saveChange(valid: boolean) {
    if (valid) {
      let fi = this.avatar.nativeElement;
      if (fi.files.length > 0) {
        this._uploadService.postWithFile('/UploadImage/saveImage?type=category', null, fi.files)
          .then((imageUrl: string) => {
            this.entity.Avatar = imageUrl;
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
      this._dataService.post("/CategoryManagement/AddNewCategory", this.entity).subscribe(
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
      this._dataService.put("/CategoryManagement/updateCategory", this.entity).subscribe(
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
      .delete("/CategoryManagement/DeleteCategory", "id", id)
      .subscribe((response: Response) => {
        this._notificationService.printSuccessMessage(
          MessageConstants.DELETED_OK_MSG
        );
        this.loadData();
      });
  }
}

