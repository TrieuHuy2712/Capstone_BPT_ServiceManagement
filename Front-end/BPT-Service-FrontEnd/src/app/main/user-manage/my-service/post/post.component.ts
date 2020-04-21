import { Component, OnInit, ViewChild } from '@angular/core';
import { DataService } from 'src/app/core/services/data.service';
import { NotificationService } from 'src/app/core/services/notification.service';
import { UploadService } from 'src/app/core/services/upload.service';
import { Ng4LoadingSpinnerService } from 'ng4-loading-spinner';
import { LoggedInUser } from 'src/app/core/domain/loggedin.user';
import { SystemConstants } from 'src/app/core/common/system,constants';
import { ModalDirective } from 'ngx-bootstrap';
import { MessageConstants } from 'src/app/core/common/message.constants';

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
  selector: 'app-post',
  templateUrl: './post.component.html',
  styleUrls: ['./post.component.css']
})


export class PostComponent implements OnInit {
  isCategory: boolean = true;
  // 
  @ViewChild("modalImage", { static: false })
  public modalImage: ModalDirective;
  @ViewChild('imgPath', { static: false }) imgPath;
  public category: any[];
  public entity: any;
  public categoryState: any[];
  public provider: any[];
  public user: LoggedInUser;
  public tagName:string="";
  public tag: any[];
  public tagState: any[];
  public services: any[];

  public aTag: TagList;
  public listTag: TagList[] = [];

  public aImage: ImageList;
  public listImage: ImageList[] = [];
  kindOfStyle: number;
  public cId: any;
  // 
  constructor(
    private _dataService: DataService,
    private _notificationService: NotificationService,
    private _uploadService: UploadService,
    private spinnerService: Ng4LoadingSpinnerService
  ) { }

  ngOnInit() {
    this.entity = {};
    this.tagName="";

    this.getAllCategory();
    this.getAllTag();

    // get current user
    this.user = JSON.parse(localStorage.getItem(SystemConstants.CURRENT_USER));
    SystemConstants.const_permission = this.user.username;
    const uName = localStorage.getItem(SystemConstants.const_username);
  }

  selectedCategory(){
    this.isCategory = !this.isCategory;
  }

  // start service method
  async saveChange(valid: boolean) {
    if (valid) {
      for(const item of this.listImage){
        await this._uploadService.postWithFile('/UploadImage/saveImage/service', null, item.dataImage)
          .then((imageUrl: any) => {
            item.path = imageUrl;
          })
      }
      this.saveData();
    }
  }

  saveDataUser() {
    debugger;
    this._dataService.post("/Service/registerServiceFromUser", this.entity).subscribe(
      (response: any) => {
        if (response.isValid == true) {
          // this.services.push(response.myModel);
          this._notificationService.printSuccessMessage(
            MessageConstants.CREATED_OK_MSG
          );
          this.spinnerService.hide();
        } else {
          this._notificationService.printErrorMessage(
            MessageConstants.CREATED_FAIL_MSG
          );
          this.spinnerService.hide();
        }
      },
      error => this._dataService.handleError(error));
  }
  selectOption(val:any){
    this.entity.categoryId = val;

  }

  saveData() {
    this.spinnerService.show();
    if (this.entity.id == undefined) {
      //Assign Id Category
      // this.entity.categoryId = this.category.find(x => x.categoryName == this.entity.categoryName).id;
      //Assign ListImages
      this.entity.listImages = this.listImage;
      //Assign Tag
      this.entity.tagOfServices = this.listTag;
      if (true) {
        //Assign User ID
        this.entity.userId = "";
        this.saveDataUser();
      } 
    } 
  }
  // 

  getAllUser() {
    this._dataService.get("/UserManagement/GetAllUser").subscribe((response: any) => {
      this.user = response;
    });
  }

  getAllCategory() {
    this._dataService.get("/CategoryManagement/GetAllCategory").subscribe((response: any) => {
      this.category = response;
      
    });
  }

  onEnter(value: string) {
    if(value.trim()==""){
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

  removeIndex(index: any) {
    this.listTag.splice(index, 1);
  }

  showImageModel() {
    this.modalImage.show();
  }

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
    console.log(this.listImage);
  }
  removeImage(index: any) {
    this.listImage.splice(index, 1);
  }
  
  // end service method
}
