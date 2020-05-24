import { Component, OnInit } from '@angular/core';
import { DataService } from 'src/app/core/services/data.service';
import { NotificationService } from 'src/app/core/services/notification.service';
import { SystemConstants } from 'src/app/core/common/system,constants';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-list-of-item',
  templateUrl: './list-of-item.component.html',
  styleUrls: ['./list-of-item.component.css']
})
export class ListOfItemComponent implements OnInit {

  public pageIndex: number = 1;
  public pageSize: number = 20;
  public pageDisplay: number = 10;
  public totalRow: number;
  public filter: string = "";
  public services: any[];
  public services2: any[];
  public services3: any[];

  public permission: any;
  public entity: any;
  public functionId: string = "SERVICES";
  public indexOfServices: number;
  locations: any;
  public newId:any;
  constructor(
    private route: ActivatedRoute,
    private _dataService: DataService,
    private _notificationService: NotificationService,
  ) { }

  ngOnInit() {
    this.newId = this.route.snapshot.paramMap.get("id");
    
    this.loadData();
    this.loadCategoryData();
    this.loadDataOfLocation();
    this.loadDataBySearching();
  }
  // load data function
  loadData() {
    this._dataService
      .get(
        "/Service/getFilterAllPaging?page=" +
        this.pageIndex +
        "&pageSize=" +
        this.pageSize +
        "&typeFilter=Location"+
        "&filterName="+this.newId
      )
      .subscribe((response: any) => {
        this.services = response.results;
        this.pageIndex = response.currentPage;
        this.pageSize = response.pageSize;
        this.totalRow = response.rowCount;
        this.indexOfServices = this.services.length;
        
      });
      
  }

  loadCategoryData() {
    this._dataService
      .get(
        "/Service/getFilterAllPaging?page=" +
        this.pageIndex +
        "&pageSize=" +
        this.pageSize +
        "&typeFilter=Category"+
        "&filterName="+this.newId
      )
      .subscribe((response: any) => {
        this.services2 = response.results;
        this.pageIndex = response.currentPage;
        this.pageSize = response.pageSize;
        this.totalRow = response.rowCount;
        this.indexOfServices = this.services2.length;
        console.log("fuking thing is here "+this.services2);
        
      });
      
  }
  // send data


  loadDataOfLocation(){
    this._dataService
      .get(
        "/LocationManagement/GetAllLocation"
      )
      .subscribe((response: any) => {
        this.locations = response;

      });
  }


  // start load slide
  sliderConfigPopular = { "slidesToShow": 4, "slidesToScroll": 1, "arrows" : true};
  afterChange(e) {
    console.log('afterChange');
  }
  // end load slide


  // loading data by searching 
  loadDataBySearching() {
    this._dataService
      .get(
        "/Service/getAllPagingPostService?keyword="+this.newId+"&isAdminPage=true&filter=5"
      )
      .subscribe((response: any) => {
        this.services3 = response.results;

      });
      
  }
}
