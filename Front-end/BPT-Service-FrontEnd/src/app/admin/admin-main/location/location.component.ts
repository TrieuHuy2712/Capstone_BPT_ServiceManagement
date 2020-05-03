import { Component, OnInit, ViewChild } from '@angular/core';
import { DataService } from 'src/app/core/services/data.service';
import { MessageConstants } from 'src/app/core/common/message.constants';
import { ModalDirective } from 'ngx-bootstrap';
import { NotificationService } from 'src/app/core/services/notification.service';
import { TranslationService } from 'src/app/core/services/translation.service';
import { SystemConstants } from 'src/app/core/common/system,constants';
import { UploadService } from 'src/app/core/services/upload.service';
import { Ng4LoadingSpinnerService } from 'ng4-loading-spinner';

@Component({
  selector: 'app-location',
  templateUrl: './location.component.html',
  styleUrls: ['./location.component.css']
})
export class LocationComponent implements OnInit {
  @ViewChild("modalAddEdit", { static: false })
  public modalAddEdit: ModalDirective;
  @ViewChild('imagePath', { static: false }) imagePath;
  public pageIndex: number = 1;
  public pageSize: number = 0;
  public pageDisplay: number = 10;
  public totalRow: number;
  public filter: string = "";
  public location: any[];
  public permission: any;
  public baseFolder: string = SystemConstants.BASE_API;
  public entity: any;
  public functionId: string = "LOCATION";
  constructor(
    private _dataService: DataService,
    private _notificationService: NotificationService,
    private translationService: TranslationService,
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
        "/LocationManagement/GetAllPaging?page=" +
        this.pageIndex +
        "&pageSize=" +
        this.pageSize +
        "&keyword=" +
        this.filter
      )
      .subscribe((response: any) => {
        this.location = response.results;
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
    let findIdthis = this.location[id];
    this.entity = findIdthis;
  }
  showEditModal(id: any) {
    this.loadRole(id);
    this.modalAddEdit.show();
  }
  saveChange(valid: boolean) {
    if (valid) {
      this.spinnerService.show();
      let fi = this.imagePath.nativeElement;
      if (fi.files.length > 0) {
        this._uploadService.postWithFile('/UploadImage/saveImage/location', null, fi.files)
          .then((imageUrl: any) => {
            this.entity.imagePath = imageUrl;
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
      this._dataService.post("/LocationManagement/addNewLocation", this.entity).subscribe(
        (response: any) => {
          if (response.isValid == true) {
            this.location.push(response.myModel);
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
      this._dataService.put("/LocationManagement/updateLocation", this.entity).subscribe(
        (response: any) => {
          if (response.isValid == true) {
            let getPostition = this.location.indexOf(x => x.id == this.entity.id);
            this.location[getPostition] = response.myModel;
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
      .delete("/LocationManagement/DeleteCategory", "id", idRole)
      .subscribe((response: any) => {
        if (response.isValid == true) {
          this.location.splice(id, 1);
          this._notificationService.printSuccessMessage(
            MessageConstants.DELETED_OK_MSG
          );
        } else {
          this._notificationService.printErrorMessage(
            MessageConstants.DELETED_FAIL_MSG
          );
        }
      });
      this.spinnerService.hide();
  }
  filterChanged(id: any) {
    this.pageSize = id;
    this.loadData()
  }
}

