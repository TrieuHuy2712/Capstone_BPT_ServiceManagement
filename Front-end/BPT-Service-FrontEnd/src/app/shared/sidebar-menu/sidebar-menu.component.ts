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
  public vnFunctionion= "";
  public user: LoggedInUser;

  constructor(
    private dataService: DataService,
    private _authenService: AuthenService,
    private languageService: LanguageService,
    
  ) { }
  ngOnInit() {

    this.user = JSON.parse(localStorage.getItem(SystemConstants.CURRENT_USER));
    SystemConstants.const_permission = this.user.username;
    const uName = localStorage.getItem(SystemConstants.const_username);
    if (uName) {
      this.dataService.get(
        "/function/GetAll/" +
        uName
      )
      .subscribe((response: any) => {
        console.log(response);      
        this._functions = response;
        this._functions = this._functions.filter(x => x.key != null);
      });
    };
  }

  toggleMenuItem(id: number) {
    const tmp = document.getElementById('item-' + id);
    const itemContainer = document.getElementById('item-container-' + id);
    
    document.querySelectorAll('.item-container').forEach(i => {
      i.classList.remove('active');
    });

    itemContainer.classList.add('active');

    tmp.classList.toggle('in');
    tmp.classList.toggle('show');
  }
  toLowKey(str:string){
    return str.toLowerCase();
  }
  onChange(deviceValue) {
    console.log(deviceValue);
    this.languageService.setLanguage(deviceValue);
  }
}
