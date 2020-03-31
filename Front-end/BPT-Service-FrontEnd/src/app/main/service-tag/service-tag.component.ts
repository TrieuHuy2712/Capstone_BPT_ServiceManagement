import { Component, OnInit } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { DataService } from 'src/app/core/services/data.service';
import { NotificationService } from 'src/app/core/services/notification.service';
import { SystemConstants } from 'src/app/core/common/system,constants';
import { MessageConstants } from 'src/app/core/common/message.constants';
import { TranslationService } from 'src/app/core/services/translation.service';

@Component({
  selector: 'app-service-tag',
  templateUrl: './service-tag.component.html',
  styleUrls: ['./service-tag.component.css']
})
export class ServiceTagComponent implements OnInit {
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
    private translationService: TranslationService
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
        if (localStorage.getItem(SystemConstants.const_username) != "admin") {
          this.loadPermission();
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
      .get("/TagManagement/GetTagById?id=" + id)
      .subscribe((response: any) => {
        this.entity = response.result;
        console.log(response);
      });
  }
  showEditModal(id: any) {
    this.loadRole(id);
    console.log(id);
    this.modalAddEdit.show();
  }
  saveChange(valid: boolean) {
    debugger;
    if (valid) {
      if (this.entity.id == undefined) {
        console.log("vo day duoc");
        this._dataService.post("/TagManagement/AddNewTag", this.entity).subscribe(
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
        this._dataService.put("/TagManagement/updateTag", this.entity).subscribe(
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
  }
  deleteItem(id: any) {
    this._notificationService.printConfirmationDialog(
      MessageConstants.CONFIRM_DELETE_MSG,
      () => this.deleteItemConfirm(id)
    );
  }
  deleteItemConfirm(id: any) {
    this._dataService
      .delete("/TagManagement/DeleteTag", "id", id)
      .subscribe((response: Response) => {
        this._notificationService.printSuccessMessage(
          MessageConstants.DELETED_OK_MSG
        );
        this.loadData();
      });
  }
}