import { Component, OnInit, ViewChild } from '@angular/core';
import { DataService } from 'src/app/core/services/data.service';
import { MessageConstants } from 'src/app/core/common/message.constants';
import { ModalDirective } from 'ngx-bootstrap';
import { NotificationService } from 'src/app/core/services/notification.service';
import { UploadService } from 'src/app/core/services/upload.service';
enum Status {
  InActive = 0,
  Active = 1,
  Pending = 2,
  UpdatePending= 3
}
@Component({
  selector: 'app-provider',
  templateUrl: './provider.component.html',
  styleUrls: ['./provider.component.css']
})
export class ProviderComponent implements OnInit {
  @ViewChild("modalAddEdit", { static: false })
  public modalAddEdit: ModalDirective;
  @ViewChild("modalReason", { static: false })
  public modalReason: ModalDirective;
  @ViewChild('avatarPath', { static: false }) avatarPath;
  public pageIndex: number = 1;
  public pageSize: number = 0;
  public pageDisplay: number = 10;
  public defaultStatus:number=5;
  public totalRow: number;
  public filter: string = "";
  public provider: any[];
  public permission: any;
  public entity: any;
  public location: any[];
  public users: any[];
  public state:any[];
  public locationState:any[];
  public reject:any;
  public functionId: string = "PROVIDER";
  constructor(
    private _dataService: DataService,
    private _notificationService: NotificationService,
    private _uploadService: UploadService
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
    this.getAllLocation();
    this.getAllUser();
  }
  loadData() {
    this._dataService
      .get(
        "/Provider/GetAllPaging?page=" +
        this.pageIndex +
        "&pageSize=" +
        this.pageSize +
        "&keyword=" +
        this.filter
        +"&filter="+
        this.defaultStatus
      )
      .subscribe((response: any) => {
        this.provider = response.results;
        this.pageIndex = response.currentPage;
        this.pageSize = response.pageSize;
        this.totalRow = response.rowCount;
        this.loadPermission();
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
    let findIdthis = this.provider[id];
    this.entity = findIdthis;
  }
  showEditModal(id: any) {
    this.loadRole(id);
    this.modalAddEdit.show();
  }
  saveChange(valid: boolean) {
    if (valid) {
      let fi = this.avatarPath.nativeElement;
      if (fi.files.length > 0) {
        this._uploadService.postWithFile('/UploadImage/saveImage/category', null, fi.files)
          .then((imageUrl: any) => {
            this.entity.avatarPath = imageUrl;
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
      let getUser= this.users.findIndex(x => x.userName == this.entity.userName);
      this.entity.userId= this.users[getUser].id;
      let getCityId= this.location.findIndex(x => x.city+"_"+x.province == this.entity.cityName);
      this.entity.cityId= this.location[getCityId].id;
      this._dataService.post("/Provider/RegisterProvider", this.entity).subscribe(
        (response: any) => {
          if (response.isValid == true) {
            this.provider.push(response.myModel);
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
      this._dataService.post("/Provider/UpdateProviderService", this.entity).subscribe(
        (response: any) => {
          if (response.isValid == true) {
            let getPostition = this.provider.indexOf(x => x.id == this.entity.id);
            this.provider[getPostition] = response.myModel;
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
      () => this.deleteItemConfirm(idRole)
    );
  }
  deleteItemConfirm(idRole: any) {
    this._dataService
      .delete("/Provider/DeleteProvider", "id", idRole)
      .subscribe(() => {
        this._notificationService.printSuccessMessage(
          MessageConstants.DELETED_OK_MSG
        );
        this.loadData();
      });
  }
  filterChanged(id: any) {
    this.pageSize = id;
    this.loadData()
  }
  getAllLocation() {
    this._dataService.get("/LocationManagement/GetAllLocation").subscribe((response: any) => {
      this.location = response;
      let allLocation= new Array();
      this.location.forEach(el=>{
        allLocation.push(el.city+"_"+el.province);
      });
      this.locationState=allLocation;
    });
  }
  getAllUser() {
    this._dataService.get("/UserManagement/GetAllUser").subscribe((response: any) => {
      this.users = response;
      let userName= new Array();
      this.users.forEach(element => {
        userName.push(element.userName);
      });
      this.state= userName;
    });
  }
  approveProvider(){
    this._dataService.post("/Provider/ApproveProvider", this.entity).subscribe(
      (response: any) => {
        if (response.isValid == true) {
          let getPostition = this.provider.indexOf(x => x.id == this.entity.id);
          this.provider[getPostition] = response.myModel;
          this.modalAddEdit.hide();
          this._notificationService.printSuccessMessage(
            MessageConstants.UPDATED_OK_MSG
          );
        } else {
          this._notificationService.printErrorMessage(
            response.errorMessage
          );
        }
      },
      error => this._dataService.handleError(error)
    );
  }
  rejectProvider(){
    this._dataService.post("/Provider/RejectProvider", this.entity).subscribe(
      (response: any) => {
        if (response.isValid == true) {
          let getPostition = this.provider.indexOf(x => x.id == this.entity.id);
          this.provider[getPostition] = response.myModel;
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

  checkUserName(userName:string){
    let getUserName= this.state.find(x=>x==userName);
    if(getUserName==null){
      return false;
    };
    return true
  }
  checkLocation(location:string){
    let locationName= this.locationState.find(x=>x==location);
    if(locationName==null){
      return false;
    };
    return true
  }

}
