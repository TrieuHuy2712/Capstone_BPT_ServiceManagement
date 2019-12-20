import { Component, OnInit } from "@angular/core";
import { DataService } from "src/app/core/services/data.service";
import { AuthenService } from 'src/app/core/services/authen.service';
import { SystemConstants } from 'src/app/core/common/system,constants';

@Component({
  selector: "app-sidebar-menu",
  templateUrl: "./sidebar-menu.component.html",
  styleUrls: ["./sidebar-menu.component.css"]
})
export class SidebarMenuComponent implements OnInit {
  public functions: any[];
  constructor(private dataService: DataService,private _authenService: AuthenService) {}
  ngOnInit() {
    this.dataService
      .get("/function/GetAll/"+localStorage.getItem(SystemConstants.const_username))
      .subscribe(
        (response: any) => {
          console.log(response);
          this.functions=response;
        },
        error => this.dataService.handleError(error)
      );
  }
}