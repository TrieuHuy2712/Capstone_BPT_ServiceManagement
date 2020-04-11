import { Component, OnInit, ViewChild, TestabilityRegistry } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { DataService } from 'src/app/core/services/data.service';
import { NotificationService } from 'src/app/core/services/notification.service';
import { UploadService } from 'src/app/core/services/upload.service';
import { Ng4LoadingSpinnerService } from 'ng4-loading-spinner';
import { MessageConstants } from 'src/app/core/common/message.constants';
enum Status {
  InActive = 0,
  Active = 1,
  Pending = 2,
  UpdatePending = 3
}
export interface TagList {
  isNew: boolean,
  tagName: string,
}
export interface ImageList {
  isAvatar: boolean,
  imageName: string,
  dataImage: any;
}
@Component({
  selector: 'app-post-service',
  templateUrl: './post-service.component.html',
})
export class PostServiceComponent implements OnInit {
  @ViewChild("modalAddEdit", { static: false })
  public modalAddEdit: ModalDirective;

  @ViewChild("modalImage", { static: false })
  public modalImage: ModalDirective;


  @ViewChild("modalReason", { static: false })
  public modalReason: ModalDirective;

  @ViewChild('imgPath', { static: false }) imgPath;
  public pageIndex: number = 1;
  public pageSize: number = 0;
  public pageDisplay: number = 10;
  public defaultStatus: number = 5;
  public totalRow: number;
  public filter: string = "";
  public services: any[];
  public reject: any;
  public permission: any;
  public entity: any;
  // List get data
  public provider: any[];
  public user: any[];
  public category: any[];
  public tag: any[];
  // List state get data
  public providerState: any[];
  public userState: any[];
  public categoryState: any[];
  public tagState: any[];
  //Another list
  //Tag list
  public aTag: TagList;
  public listTag: TagList[] = [];
  //Image list
  public aImage: ImageList;
  public listImage: ImageList[] = [];
  //Support for function
  public kindOfStyle: number = 0;
  public functionId: string = "SERVICE";
  constructor(
    private _dataService: DataService,
    private _notificationService: NotificationService,
    private _uploadService: UploadService,
    private spinnerService: Ng4LoadingSpinnerService
  ) { }

  ngOnInit() {
    this.permission = {
      canCreate: true,
      canDelete: true,
      canUpdate: true,
      canRead: true
    };
    this.loadData();
    this.getAllCategory();
    this.getAllProvider();
    this.getAllUser();
    this.getAllTag();

  }
  loadData() {
    this.spinnerService.show();
    this._dataService
      .get(
        "/Service/getAllPagingPostService?page=" +
        this.pageIndex +
        "&pageSize=" +
        this.pageSize +
        "&keyword=" +
        this.filter +
        "&isAdminPage=true&filter=" + this.defaultStatus
      )
      .subscribe((response: any) => {
        this.services = response.results;
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
    this.aTag = {
      isNew: true,
      tagName: ""
    };
    this.modalAddEdit.show();
  }
  loadRole(id: any) {
    let findIdthis = this.services[id];
    this.entity = findIdthis;
  }
  showEditModal(id: any) {
    this.loadRole(id);
    this.modalAddEdit.show();
  }
  saveChange(valid: boolean) {
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
    this.spinnerService.show();
    if (this.entity.id == undefined) {
      let getUser = this.provider.findIndex(x => x.providerName == this.entity.providerName);
      this.entity.providerId = this.provider[getUser].id;
      this._dataService.post("/ProviderNews/RegisterNewsProvider", this.entity).subscribe(
        (response: any) => {
          if (response.isValid == true) {
            this.services.push(response.myModel);
            this._notificationService.printSuccessMessage(
              MessageConstants.CREATED_OK_MSG
            );
            this.modalAddEdit.hide();
            this.spinnerService.hide();
          } else {
            this._notificationService.printErrorMessage(
              MessageConstants.CREATED_FAIL_MSG
            );
            this.spinnerService.hide();
          }
        },
        error => this._dataService.handleError(error)
      );
    } else {
      this._dataService.post("/ProviderNews/UpdateNewsProvider", this.entity).subscribe(
        (response: any) => {
          if (response.isValid == true) {
            let getPostition = this.provider.indexOf(x => x.id == this.entity.id);
            this.services[getPostition] = response.myModel;
            this.modalAddEdit.hide();
            this._notificationService.printSuccessMessage(
              MessageConstants.UPDATED_OK_MSG

            );
            this.spinnerService.hide();
          } else {
            this._notificationService.printErrorMessage(
              MessageConstants.UPDATED_FAIL_MSG
            );
            this.spinnerService.hide();
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
    this._dataService
      .delete("/ProviderNews/DeleteNewsProvider", "id", idRole)
      .subscribe((response: any) => {
        if (response.isValid) {
          this._notificationService.printSuccessMessage(
            MessageConstants.DELETED_OK_MSG
          );
          this.services = this.services.splice(id, 1);
        } else {
          this._notificationService.printErrorMessage(
            response.errorMessage
          );
        }

      });
  }
  filterChanged(id: any) {
    this.pageSize = id;
    this.loadData()
  }
  approveProvider() {
    this.spinnerService.show();
    this._dataService.post("/ProviderNews/ApproveNewsProvider", this.entity).subscribe(
      (response: any) => {
        if (response.isValid == true) {
          let getPostition = this.services.findIndex(x => x.id == this.entity.id);
          this.services[getPostition].status = 1;
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
  rejectProvider() {
    this.spinnerService.show();
    this._dataService.post("/ProviderNews/RejectNewsProvider", this.entity).subscribe(
      (response: any) => {
        if (response.isValid == true) {
          let getPostition = this.services.findIndex(x => x.id == this.entity.id);
          this.services[getPostition].status = 0;
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
  showRejectProvider() {
    this.modalReason.show();
    this.reject = [];
  }
  filterStatus(id: any) {
    this.defaultStatus = id;
    this.loadData();
  }
  checkProviderName(userName: any) {
    if (userName.pristine) {
      return true;
    }
    let getUserName = this.providerState.find(x => x == userName.value);
    if (getUserName == null) {
      return false;
    };
    return true
  }
  checkCategoryName(category: any) {
    if (category.pristine) {
      return true;
    }
    let getUserName = this.categoryState.find(x => x == category.value);
    if (getUserName == null) {
      return false;
    };
    return true
  }
  // checkTagName(tag:any){
  //   if(tag.pristine){
  //     return true;
  //   }
  //   let getTagName= this.tagState.find(x=>x==tag.value);
  //   if(getTagName==null){
  //     return false;
  //   };
  //   return true
  // }
  checkUserName(userName: any) {
    if (userName.pristine) {
      return true;
    }
    let getUserName = this.userState.find(x => x == userName.value);
    if (getUserName == null) {
      return false;
    };
    return true
  }
  getAllProvider() {
    this._dataService.get("/Provider/GetAllPaging?page=1&pageSize=0&keyword=&filter=5").subscribe((response: any) => {
      this.provider = response.results;
      let userName = new Array();
      this.provider.forEach(element => {
        userName.push(element.providerName);
      });
      this.providerState = userName;
    });
  }
  getAllCategory() {
    this._dataService.get("/CategoryManagement/GetAllCategory").subscribe((response: any) => {
      this.category = response;
      let categoryName = new Array();
      this.category.forEach(element => {
        categoryName.push(element.categoryName);
      });
      this.categoryState = categoryName;
    });
  }
  getAllTag() {
    this._dataService.get("/TagManagement/GetAllTag").subscribe((response: any) => {
      this.tag = response;
      let tagName = new Array();
      this.tag.forEach(element => {
        tagName.push(element.tagName);
      });
      this.tagState = tagName;
    });
  }
  getAllUser() {
    this._dataService.get("/UserManagement/GetAllUser").subscribe((response: any) => {
      this.user = response;
      let userName = new Array();
      this.user.forEach(element => {
        userName.push(element.userName);
      });
      this.userState = userName;
    });
  }
  filterUserService(style: any) {
    if (style == 0) {
      this.kindOfStyle = 0;
    } else {
      this.kindOfStyle = 1;
    }
  }
  onEnter(value: string) {
    var findIsIndex = this.tagState.findIndex(x => x == value);
    if (findIsIndex == -1) {
      this.aTag.isNew = true;
      this.aTag.tagName = value;

    } else {
      this.aTag.isNew = false;
      this.aTag.tagName = value;
    }
    this.listTag.push(this.aTag);
    this.aTag = {
      isNew: false,
      tagName: ""
    }
  }
  removeIndex(index: any) {
    this.listTag.splice(index, 1);
  }
  showImageModel(){
    this.modalImage.show();
  }
  addImage(){
    let newImage:ImageList= {
      dataImage: null,
      imageName:"",
      isAvatar:false
    }
    this.listImage.push(newImage);
  }
  selectChangeFile(event:any, img:number){
    this.listImage[img].dataImage = event.files;
    console.log(this.listImage);
  }
}
