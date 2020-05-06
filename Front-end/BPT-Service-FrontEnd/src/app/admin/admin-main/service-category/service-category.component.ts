import { Component, OnInit, ViewChild } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { DataService } from 'src/app/core/services/data.service';
import { NotificationService } from 'src/app/core/services/notification.service';
import { SystemConstants } from 'src/app/core/common/system,constants';
import { MessageConstants } from 'src/app/core/common/message.constants';
import { UploadService } from 'src/app/core/services/upload.service';
import { Ng4LoadingSpinnerService } from 'ng4-loading-spinner';

@Component({
  selector: 'app-service-category',
  templateUrl: './service-category.component.html',
  styleUrls: ['./service-category.component.css']
})
export class ServiceCategoryComponent implements OnInit {

  @ViewChild("modalAddEdit", { static: false })
  public modalAddEdit: ModalDirective;
  @ViewChild('imgPath', { static: false }) imgPath;
  public pageIndex: number = 1;
  public pageSize: number = 0;
  public pageDisplay: number = 10;
  public totalRow: number;
  public filter: string = "";
  public categories: any[];
  public permission: any;
  public entity: any;
  public functionId: string = "CATEGORY";
  constructor(
    private _dataService: DataService,
    private _notificationService: NotificationService,
    private _uploadService: UploadService,
    private spinnerService: Ng4LoadingSpinnerService
  ) {

  }

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
    if(this.imgPath != undefined){
      this.imgPath.nativeElement.value = "";
    }
    this.entity = {};
    this.modalAddEdit.show();
  }
  loadRole(id: any) {
    let findIdthis = this.categories[id];
    this.entity = findIdthis;
  }
  showEditModal(id: any) {
    if(this.imgPath != undefined){
      this.imgPath.nativeElement.value = "";
    }
    this.loadRole(id);
    this.modalAddEdit.show();
  }
  saveChange(valid: boolean) {
    if (valid) {
      this.spinnerService.show();
      let fi = this.imgPath.nativeElement;
      if (fi.files.length > 0) {
        this._uploadService.postWithFile('/UploadImage/saveImage/category', null, fi.files)
          .then((imageUrl: any) => {
            this.entity.imgPath = imageUrl;
          }).then(() => {
            this.saveData();
          });
      }
      else {
        this.saveData();
      }
      this.spinnerService.hide();
    }
  }
  saveData() {
      if (this.entity.id == undefined) {
        this._dataService.post("/CategoryManagement/AddNewCategory", this.entity).subscribe(
          (response: any) => {
            if (response.isValid == true) {
              this.categories.push(response.myModel);
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
        this._dataService.put("/CategoryManagement/updateCategory", this.entity).subscribe(
          (response: any) => {
            if (response.isValid == true) {
              let getPostition = this.categories.indexOf(x => x.id == this.entity.id);
              this.categories[getPostition] = response.myModel;
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
      .delete("/CategoryManagement/DeleteCategory", "id", idRole)
      .subscribe((response: any) => {
        if(response.isValid==true){
          this._notificationService.printSuccessMessage(
            MessageConstants.DELETED_OK_MSG
          );
          this.categories.splice(id,1);
        }else{
          this._notificationService.printErrorMessage(
            response.errorMessage
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

