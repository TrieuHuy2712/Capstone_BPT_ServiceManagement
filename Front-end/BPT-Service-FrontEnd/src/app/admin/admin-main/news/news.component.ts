import { Component, OnInit, ViewChild } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { DataService } from 'src/app/core/services/data.service';
import { NotificationService } from 'src/app/core/services/notification.service';
import { UploadService } from 'src/app/core/services/upload.service';
import { MessageConstants } from 'src/app/core/common/message.constants';
import { Ng4LoadingSpinnerService } from 'ng4-loading-spinner';
enum Status {
  InActive = 0,
  Active = 1,
  Pending = 2,
  UpdatePending= 3
}
@Component({
  selector: 'app-news',
  templateUrl: './news.component.html',
})
export class NewsComponent implements OnInit {
  @ViewChild("modalAddEdit", { static: false })
  public modalAddEdit: ModalDirective;
  @ViewChild("modalReason", { static: false })
  public modalReason: ModalDirective;
  @ViewChild('imgPath', { static: false }) imgPath;
  public pageIndex: number = 1;
  public pageSize: number = 0;
  public pageDisplay: number = 10;
  public defaultStatus:number=5;
  public totalRow: number;
  public filter: string = "";
  public news: any[];
  public reject:any;
  public permission: any;
  public entity: any;
  public provider: any[];
  public state:any[];
  public functionId: string = "NEWS";
  constructor(
    private _dataService: DataService,
    private _notificationService: NotificationService,
    private _uploadService: UploadService,
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
    this.getAllProvider();
  }
  loadData() {
    this.spinnerService.show();
    this._dataService
      .get(
        "/ProviderNews/GetAllPagingProviderNews?page=" +
        this.pageIndex +
        "&pageSize=" +
        this.pageSize +
        "&keyword=" +
        this.filter+
        "&isAdminPage=true&filter="+this.defaultStatus
      )
      .subscribe((response: any) => {
        this.news = response.results;
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
    let findIdthis = this.news[id];
    this.entity = findIdthis;
  }
  showEditModal(id: any) {
    this.loadRole(id);
    this.modalAddEdit.show();
  }
  saveChange(valid: boolean) {
    this.spinnerService.show();
    if (valid) {
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
    }
  }
  saveData() {
    if (this.entity.id == undefined) {
      let getUser= this.provider.findIndex(x => x.providerName == this.entity.providerName);
      this.entity.providerId= this.provider[getUser].id;
      this._dataService.post("/ProviderNews/RegisterNewsProvider", this.entity).subscribe(
        (response: any) => {
          if (response.isValid == true) {
            this.news.push(response.myModel);
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
      this._dataService.post("/ProviderNews/UpdateNewsProvider", this.entity).subscribe(
        (response: any) => {
          if (response.isValid == true) {
            let getPostition = this.provider.indexOf(x => x.id == this.entity.id);
            this.news[getPostition] = response.myModel;
            this.modalAddEdit.hide();
            this._notificationService.printSuccessMessage(
              MessageConstants.UPDATED_OK_MSG
              
            );
          } else {
            this._notificationService.printErrorMessage(
              MessageConstants.UPDATED_FAIL_MSG
            );
          }
          this.spinnerService.hide();
        },
        error => this._dataService.handleError(error)
      );
    }
  }
  deleteItem(idRole: any, id: any) {
    this._notificationService.printConfirmationDialog(
      MessageConstants.CONFIRM_DELETE_MSG,
      () => this.deleteItemConfirm(idRole,id)
    );
  }
  deleteItemConfirm(idRole: any,id:any) {
    this.spinnerService.show();
    this._dataService
      .delete("/ProviderNews/DeleteNewsProvider", "id", idRole)
      .subscribe((response:any) => {
        if(response.isValid){
          this._notificationService.printSuccessMessage(
            MessageConstants.DELETED_OK_MSG
          );
          this.news = this.news.splice(id,1);
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
  approveProvider(){
    this.spinnerService.show();
    this._dataService.post("/ProviderNews/ApproveNewsProvider", this.entity).subscribe(
      (response: any) => {
        if (response.isValid == true) {
          let getPostition = this.news.findIndex(x => x.id == this.entity.id);
          this.news[getPostition].status = 1;
          this.modalAddEdit.hide();
          this._notificationService.printSuccessMessage(
            MessageConstants.UPDATED_OK_MSG
          );
        } else {
          this._notificationService.printErrorMessage(
            response.errorMessage
          );
        }
        this.spinnerService.hide();
      },
      error => this._dataService.handleError(error)
    );
  }
  rejectProvider(){
    this.spinnerService.show();
    this._dataService.post("/ProviderNews/RejectNewsProvider", this.entity).subscribe(
      (response: any) => {
        if (response.isValid == true) {
          let getPostition = this.news.findIndex(x => x.id == this.entity.id);
          this.news[getPostition].status = 0;
          this.modalReason.hide();
          this.modalAddEdit.hide();
          this._notificationService.printSuccessMessage(
            MessageConstants.UPDATED_OK_MSG
          );
        } else {
          this._notificationService.printErrorMessage(
            response.errorMessage
          );
        }
        this.spinnerService.hide();
      },
      error => this._dataService.handleError(error)
    );
  }
  showRejectProvider(){
    this.modalReason.show();
    this.reject=[];
  }
  filterStatus(id: any){
    this.defaultStatus=id;
    this.loadData();
  }
  checkProviderName(userName:any){
    if(userName.pristine){
      return true;
    }
    let getUserName= this.state.find(x=>x==userName.value);
    if(getUserName==null){
      return false;
    };
    return true
  }
  getAllProvider() {
    this._dataService.get("/Provider/GetAllPaging?page=1&pageSize=0&keyword=&filter=5").subscribe((response: any) => {
      this.provider = response.results;
      let userName= new Array();
      this.provider.forEach(element => {
        userName.push(element.providerName);
      });
      this.state= userName;
    });
  }
}
