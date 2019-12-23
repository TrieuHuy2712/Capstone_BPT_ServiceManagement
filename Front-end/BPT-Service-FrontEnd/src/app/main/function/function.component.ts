import { Component, OnInit, ViewChild } from "@angular/core";

import { DataService } from "src/app/core/services/data.service";
import { MessageConstants } from "src/app/core/common/message.constants";
import { ModalDirective } from "ngx-bootstrap/modal";
import { NotificationService } from "src/app/core/services/notification.service";
import { TreeComponent } from "angular-tree-component";
import { UtilityService } from "src/app/core/services/utility.service";
import { SystemConstants } from 'src/app/core/common/system,constants';

@Component({
  selector: "app-function",
  templateUrl: "./function.component.html",
  styleUrls: ["./function.component.css"]
})
export class FunctionComponent implements OnInit {
  @ViewChild("addEditModal", { static: false })
  public addEditModal: ModalDirective;
  @ViewChild("permissionModal", { static: false })
  public permissionModal: ModalDirective;
  @ViewChild(TreeComponent, { static: false })
  private treeFunction: TreeComponent;

  public _functionHierachy: any[];
  public _functions: any[];
  public entity: any;
  public editFlag: boolean;
  public filter: string = "";
  public functionId: string;
  public _permission: any[];
  public _functionId: string = "FUNCTION";
  public _userPermission:any;
  public _currentUser:string=localStorage.getItem(SystemConstants.const_username);
  public _adminPermission:any;
  constructor(
    private _dataService: DataService,
    private notificationService: NotificationService,
    private utilityService: UtilityService
  ) {}

  ngOnInit() {
    this._userPermission = {
      canCreate: true,
      canDelete: true,
      canUpdate: true,
      canRead: true
    };
    this.search();
  }
  public showPermission(id: any) {
    this._dataService.get("/AdminRole/getAllPermission/" + id).subscribe((response: any[]) => {
        console.log(response);
        this.functionId = id;
        this._permission = response;
        this.permissionModal.show();
      },
      error => {console.log(error)}
    )
  }
  public savePermission(valid: boolean, _permission: any[]) {
    if (valid) {
      var data = {
        Permissions: this._permission,
        FunctionId: this.functionId
      };
      this._dataService.post("/AdminRole/SavePermission", data).subscribe(
        (response: any) => {
          console.log(response);
          this.notificationService.printSuccessMessage(MessageConstants.UPDATED_OK_MSG);
          this.permissionModal.hide();
        },
        error => {console.log(error)}
    )
    }
  }
  //Show add form
  public showAddModal() {
    this.entity = {};
    this.addEditModal.show();
    this.editFlag = false;
  }
  //Load data
  public search() {
    this._dataService.get("/function/GetAll/admin").subscribe(
      (response: any) => {
        console.log(response);
        this._functions = response;
        this._functionHierachy = this._functions[0].childrenId;
        this._functions = this._functions.filter(x => x.key != null);
        if (this._functionHierachy.length != this._functions.length) {
          for (let index = 0; index < this._functionHierachy.length; index++) {
            let count = 0;
            for (let index1 = 0; index1 < this._functions.length; index1++) {
              if (
                this._functionHierachy[index].id == this._functions[index1].key
              ) {
                console.log(this._functionHierachy[index].id);
                count++;
              }
            }
            if (count == 0) {
              this._functions.push({ key: this._functionHierachy[index].id });
            }
          }
        }
        console.log(this._functions);
        if (this._currentUser != "admin") {
          this.loadPermission();
        }
        
      },
      error => this._dataService.handleError(error)
    );
  }
  loadPermission() {
    this._dataService.get("/PermissionManager/GetAllPermission/" +this._currentUser+"/"+this._functionId
      ).subscribe((response: any) => {
        this._userPermission = response;
        console.log(response);
      });
  }

  //Save change for modal popup
  public saveChanges(valid: boolean) {
    if (valid) {
      if (this.editFlag == false) {
        this._dataService.post("/function/addEntity", this.entity).subscribe(
          (response: any) => {
            this.search();
            this.addEditModal.hide();
            this.notificationService.printSuccessMessage(
              MessageConstants.CREATED_OK_MSG
            );
          },
          error => this._dataService.handleError(error)
        );
      } else {
        this._dataService.put("/function/updateEntity", this.entity).subscribe(
          (response: any) => {
            this.search();
            this.addEditModal.hide();
            this.notificationService.printSuccessMessage(
              MessageConstants.UPDATED_OK_MSG
            );
          },
          error => this._dataService.handleError(error)
        );
      }
    }
  }
  //Show edit form
  public showEdit(id: string) {
    this._dataService.get("/function/GetById/" + id).subscribe(
      (response: any) => {
        console.log(response);
        this.entity = response;
        this.editFlag = true;
        this.addEditModal.show();
      },
      error => this._dataService.handleError(error)
    );
  }

  //Action delete
  public deleteConfirm(id: any) {
    this._dataService
      .delete("/function/DeleteFunction", "id", id)
      .subscribe((response: any) => {
        console.log(response);
        this.notificationService.printSuccessMessage(
          MessageConstants.DELETED_OK_MSG
        );
        this.search();
      });
  }
  //Click button delete turn on confirm
  public delete(id: any) {
    this.notificationService.printConfirmationDialog(
      MessageConstants.CONFIRM_DELETE_MSG,
      () => this.deleteConfirm(id)
    );
    
  }
}
