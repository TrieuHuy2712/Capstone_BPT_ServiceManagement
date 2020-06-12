import { Component, OnInit, ViewChild } from '@angular/core';
import { DataService } from 'src/app/core/services/data.service';
import { NotificationService } from 'src/app/core/services/notification.service';
import { LoggedInUser } from 'src/app/core/domain/loggedin.user';
import { SystemConstants } from 'src/app/core/common/system,constants';
import { ModalDirective } from 'ngx-bootstrap';
import { UploadService } from 'src/app/core/services/upload.service';
import { MessageConstants } from 'src/app/core/common/message.constants';

enum Status {
  InActive = 0,
  Active = 1,
  Pending = 2,
  UpdatePending = 3
}

export interface TagList {
  isAdd: boolean,
  tagName: string,
  tagId: string,
}

export interface ImageList {
  isAvatar: boolean,
  path: string,
  dataImage: File[];
}

@Component({
  selector: 'app-view',
  templateUrl: './view.component.html',
  styleUrls: ['./view.component.css']
})
export class ViewComponent implements OnInit {

  @ViewChild("modalAddEdit", { static: false })
  public modalAddEdit: ModalDirective;

  @ViewChild("modalImage", { static: false })
  public modalImage: ModalDirective;

  @ViewChild("modalReason", { static: false })
  public modalReason: ModalDirective;

  @ViewChild('imgPath', { static: false }) imgPath;

  public viewOfService: any[];
  public user: LoggedInUser;
  public user01: any[];
  public userId: any;
  public UIS: any[];

  // edit

  public filter: string = "";
  public permission: any;
  public entity: any;
  public tagName: string = "";

  // List get data
  public provider: any[];
  public category: any[];
  public tag: any[];
  public services: any[];

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
  public functionId: string = "";
  spinnerService: any;

  public reject: any;
  public defaultStatus: number = 5;


  constructor(
    private _dataService: DataService,
    private _notificationService: NotificationService,
    private _uploadService: UploadService,
  ) { }

  ngOnInit() {
    this.entity = {};
    this.getAllUser();
    this.tagName = "";
    // this.loadData();

  }

  ngAfterViewInit() {
    this.getAllCategory();
    this.getAllProvider();
    this.getAllUser01();
    this.getAllTag();
  }
  //  get data of service of current user. Note ***
  getAllUser() {
    this._dataService.get("/UserManagement/GetAllUser").subscribe((response: any) => {
      this.UIS = response;
      this.user = JSON.parse(localStorage.getItem(SystemConstants.CURRENT_USER));
      this.userId = this.UIS.find(x => x.fullName == this.user.fullName).id;
      this._dataService
        .get(
          "/Service/getAllPostUserServiceByUserId?idUser="+this.userId+"&isProvider=true"
        )
        .subscribe((response: any) => {
          this.viewOfService = response;
          console.log(this.viewOfService);
          
        });
    });

  }

  // get data of current service, to edit info.

  // loadData() {
  //   this._dataService
  //     .get(
  //       "/Service/getAllPagingPostService?page=1&pageSize=20&keyword=" +
  //       this.filter +
  //       "&isAdminPage=true&filter=5"
  //     )
  //     .subscribe((response: any) => {
  //       this.services = response.results;
  //     });
  // }

  // show form to edit
  showEditModal(id: any) {
    this.loadRole(id);
    this.modalAddEdit.show();
  }
  async saveChange(valid: boolean) {
    if (valid) {
      for (const item of this.listImage) {
        if (item.dataImage != null) {
          await this._uploadService.postWithFile('/UploadImage/saveImage/service', null, item.dataImage)
            .then((imageUrl: any) => {
              item.path = imageUrl;
            })
        }
      }
      this.saveData();
    }
  }



  loadRole(id: any) {
    this.entity = this.viewOfService[id];
    this.entity.providerName = this.entity.author;

    console.log(this.entity);
    
    
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
  getAllUser01() {
    this._dataService.get("/UserManagement/GetAllUser").subscribe((response: any) => {
      this.user01 = response;
      let userName = new Array();
      this.user01.forEach(element => {
        userName.push(element.userName);
      });
      this.userState = userName;
    });
  }

  onEnter(value: string) {
    if (value.trim() == "") {
      return
    }
    var findIsIndex = this.tagState.findIndex(x => x == value);
    if (findIsIndex == -1) {
      let newTag: TagList = {
        isAdd: true,
        tagName: value,
        tagId: null
      }
      this.listTag.push(newTag);
    } else {
      let availableTag: TagList = {
        isAdd: false,
        tagName: value,
        tagId: this.tag.find(x => x.tagName == value).id
      }
      this.listTag.push(availableTag);
    }
    this.aTag = {
      isAdd: false,
      tagName: "",
      tagId: ""
    }
    console.log(this.listTag);
  }

  showImageModel() {
    this.modalImage.show();
  }
  //#endregion
  addImage() {
    let newImage: ImageList = {
      dataImage: null,
      path: "",
      isAvatar: false
    }
    this.listImage.push(newImage);
  }
  selectChangeFile(event: File[], img: number) {
    this.listImage[img].dataImage = event;
  }
  removeImage(index: any) {
    this.listImage.splice(index, 1);
  }
  disableIsAvatar() {
    let findIsAvatar = this.listImage.findIndex(x => x.isAvatar == true);
    if (findIsAvatar == -1) {
      return true;
    }
    return false;
  }

  //#region  Check Validation
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

  saveDataProvider() {
    this._dataService.post("/Service/registerServiceFromProvider", this.entity).subscribe(
      (response: any) => {
        if (response.isValid == true) {
          this.services.push(response.myModel);
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
      error => this._dataService.handleError(error));
  }

  saveDataUser() {
    this._dataService.post("/Service/registerServiceFromUser", this.entity).subscribe(
      (response: any) => {
        if (response.isValid == true) {
          this.services.push(response.myModel);
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
      error => this._dataService.handleError(error));
  }
  saveData() {
    debugger;
    //Assign Id Category
    this.entity.categoryId = this.category.find(x => x.categoryName == this.entity.categoryName).id;
    //Assign ListImages
    this.entity.listImages = this.listImage;
    //Assign Tag
    this.entity.tagofServices = this.listTag;
    this.entity.userId = this.user01.find(x => x.fullName == this.user.fullName).id;
    if (this.entity.id == undefined) {
      //Assign User ID
      

    } else {
      this._dataService.post("/Service/updatePostService", this.entity).subscribe(
        (response: any) => {
          if (response.isValid == true) {
            let getPostition = this.services.indexOf(x => x.id == this.entity.id);
            this.provider[getPostition] = response.myModel;
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
        error => this._dataService.handleError(error));
    }
  }

  deleteItem(idRole: any, isProvider: any, id: any) {
    this._notificationService.printConfirmationDialog(
      MessageConstants.CONFIRM_DELETE_MSG,
      () => this.deleteItemConfirm(idRole, isProvider, id)
    );
  }
  deleteItemConfirm(idRole: any, isProvider: any, id: any) {
    if (isProvider == false) {

      this._dataService
        .delete("/Service/deleteServiceFromUser", "id", idRole)
        .subscribe((response: any) => {
          if (response.isValid) {
            this._notificationService.printSuccessMessage(
              MessageConstants.DELETED_OK_MSG
            );
            this.getAllUser();
            this.services.splice(id, 1);
          } else {
            this._notificationService.printErrorMessage(
              response.errorMessage
            );
          }

        });
    } else if (isProvider == true) {
      this._dataService
        .delete("/Service/deleteServiceFromProvider", "id", idRole)
        .subscribe((response: any) => {
          if (response.isValid) {
            this._notificationService.printSuccessMessage(
              MessageConstants.DELETED_OK_MSG
            );
            this.services.splice(id, 1);
          } else {
            this._notificationService.printErrorMessage(
              response.errorMessage
            );
          }

        });
    }
  }

  removeIndex(index: any) {
    this.listTag.splice(index, 1);
  }

  

  approveProvider() {
    this.spinnerService.show();
    this._dataService.post("/Service/approvePostService", this.entity).subscribe(
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
    this._dataService.post("/Service/rejectPostService", this.entity).subscribe(
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
    // this.loadData();
  }
  filterUserService(style: any) {
    if (style == 0) {
      this.kindOfStyle = 0;
    } else {
      this.kindOfStyle = 1;
    }
  }
}
