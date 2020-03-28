import { Component, OnInit } from '@angular/core';
import { DataService } from 'src/app/core/services/data.service';
import { NotificationService } from 'src/app/core/services/notification.service';
import { SystemConstants } from 'src/app/core/common/system,constants';

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
  public permission: any;
  public entity: any;
  public functionId: string = "SERVICES";
  constructor(
    private _dataService: DataService,
    private _notificationService: NotificationService
  ) { }

  ngOnInit() {
    this.loadData();
  }
  // load data function
  loadData() {
    this._dataService
      .get(
        "/Service/getAllPagingPostService?page=" +
          this.pageIndex +
          "&pageSize=" +
          this.pageSize +
          "&keyword=" +
          this.filter
      )
      .subscribe((response: any) => {
        this.services = response.results;
        this.pageIndex = response.currentPage;
        this.pageSize = response.pageSize;
        this.totalRow = response.rowCount;
        
      });
  }


  // start load slide
  sliderConfigPopular = { "slidesToShow": 4, "slidesToScroll": 1, "arrows" : true};
  afterChange(e) {
    console.log('afterChange');
  }
  // end load slide
}
