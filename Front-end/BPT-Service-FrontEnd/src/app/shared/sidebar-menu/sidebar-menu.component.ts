import { Component, OnInit } from "@angular/core";
import { DataService } from "src/app/core/services/data.service";
import { SystemConstants } from "src/app/core/common/system,constants";

@Component({
  selector: "app-sidebar-menu",
  templateUrl: "./sidebar-menu.component.html",
  styleUrls: ["./sidebar-menu.component.css"]
})
export class SidebarMenuComponent implements OnInit {
  public functions: any[];
  constructor(private dataService: DataService) {}
  ngOnInit() {
    debugger
    this.dataService
      .get("/function/GetAll/admin")
      .subscribe(
        (response: any) => {
          // this.functions = response.sort((n1, n2) => {
          //   if (n1.SortOrder > n2.SortOrder) return 1;
          //   else if (n1.SortOrder < n2.SortOrder) return -1;
          //   return 0;
          // });
          this.functions=response;
          console.log(response);
        },
        error => this.dataService.handleError(error)
      );
  }
}
