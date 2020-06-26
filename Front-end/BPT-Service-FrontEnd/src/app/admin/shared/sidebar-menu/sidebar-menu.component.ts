import { Component, OnInit } from "@angular/core";

import { AuthenService } from "src/app/core/services/authen.service";
import { DataService } from "src/app/core/services/data.service";
import { SystemConstants } from "src/app/core/common/system,constants";
import { LanguageService } from 'src/app/core/services/language.service';
import { LoggedInUser } from 'src/app/core/domain/loggedin.user';

@Component({
  selector: "app-sidebar-menu",
  templateUrl: "./sidebar-menu.component.html",
  styleUrls: ["./sidebar-menu.component.css"]
})
export class SidebarMenuComponent implements OnInit {
  public _functions: any[];
  public vnFunctionion = "";
  public currentUser: any;
  public user: LoggedInUser;

  constructor(
    private dataService: DataService,
    private _authenService: AuthenService,
    private languageService: LanguageService,

  ) { }
  ngOnInit() {

    // this.user = JSON.parse(localStorage.getItem(SystemConstants.CURRENT_USER));
    this.getUser();
    console.log("ket qua "+ this._functions);

    


  }
  getUser() {
    this.dataService.get('/UserManagement/GetByContext').subscribe((response: any) => {
      this.currentUser = response;
      SystemConstants.const_permission = this.currentUser.username;
      const uName = localStorage.getItem(SystemConstants.const_username);
      if (uName) {
        this.dataService.get(
          "/function/GetAll/" +
          uName
        )
          .subscribe((response: any) => {
            
            this._functions = response;
            this._functions = this._functions.filter(x => x.key != null);
            // vietsub manage
            if(this._functions[0].key == "Manage"){
              this._functions[0].key = "Quản lý";
            }
            else if(this._functions[0].childrenId[0].name == "Category"){
              this._functions[0].childrenId[0].name = "Danh mục";
            }
            if(this._functions[0].childrenId[1].name == "News"){
              this._functions[0].childrenId[1].name = "Tin tức";
            }
            if(this._functions[0].childrenId[2].name == "Provider"){
              this._functions[0].childrenId[2].name = "Nhà cung cấp";
            }
            if(this._functions[0].childrenId[3].name == "Recommendation"){
              this._functions[0].childrenId[3].name = "Khuyến nghị";
            }
            if(this._functions[0].childrenId[4].name == "Service"){
              this._functions[0].childrenId[4].name = "Dịch vụ";
            }
            if(this._functions[0].childrenId[4].name == "Service"){
              this._functions[0].childrenId[4].name = "Dịch vụ";
            }
            if(this._functions[0].childrenId[5].name == "Service_Tag"){
              this._functions[0].childrenId[5].name = "Tag của dịch vụ";
            }
            // vietsub system
            if(this._functions[1].key == "System"){
              this._functions[1].key = "Hệ thống";
            }
            if(this._functions[1].childrenId[0].name == "Email"){
              this._functions[1].childrenId[0].name = "Thư điện tử";
            }
            if(this._functions[1].childrenId[1].name == "Function"){
              this._functions[1].childrenId[1].name = "Chức năng";
            }
            if(this._functions[1].childrenId[2].name == "Location"){
              this._functions[1].childrenId[2].name = "Vị trí";
            }
            if(this._functions[1].childrenId[3].name == "Role"){
              this._functions[1].childrenId[3].name = "Vai trò";
            }
            if(this._functions[1].childrenId[4].name = "User"){
              this._functions[1].childrenId[4].name = "Người dùng";
            }
          });
      }
      if(this.currentUser.avatar == null){
        this.currentUser.avatar = "../../../../assets/images/default.png";
      }
    })
  }

  // toggleMenuItem(id: number) {
  //   const tmp = document.getElementById('item-' + id);
  //   const itemContainer = document.getElementById('item-container-' + id);

  //   document.querySelectorAll('.item-container').forEach(i => {
  //     i.classList.remove('active');
  //   });

  //   itemContainer.classList.add('active');

  //   tmp.classList.toggle('in');
  //   tmp.classList.toggle('show');
  // }
  // toLowKey(str:string){
  //   return str.toLowerCase();
  // }
  onChange(deviceValue) {
    this.languageService.setLanguage(deviceValue);
  }

  // vietsub this sidebar

}
