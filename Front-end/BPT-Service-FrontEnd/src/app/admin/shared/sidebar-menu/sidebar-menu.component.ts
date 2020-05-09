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
            this._functions[0].key = "Quản lý";
            this._functions[0].childrenId[0].name = "Danh mục";
            this._functions[0].childrenId[1].name = "Tin tức";
            this._functions[0].childrenId[2].name = "Nhà cung cấp";
            this._functions[0].childrenId[3].name = "Dịch vụ";
            this._functions[0].childrenId[4].name = "Đuôi của dịch vụ";
            // vietsub system
            this._functions[1].key = "Hệ thống";
            this._functions[1].childrenId[0].name = "Thư điện tử";
            this._functions[1].childrenId[1].name = "Chức năng";
            this._functions[1].childrenId[2].name = "Vị trí";
            this._functions[1].childrenId[3].name = "Người dùng";
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
