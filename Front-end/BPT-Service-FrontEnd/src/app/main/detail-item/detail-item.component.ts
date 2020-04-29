import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { NavigationStart } from '@angular/router';
import { ActivatedRoute } from '@angular/router'
import { DataService } from 'src/app/core/services/data.service';
import { NotificationService } from 'src/app/core/services/notification.service';
import { LoggedInUser } from 'src/app/core/domain/loggedin.user';
import { SystemConstants } from 'src/app/core/common/system,constants';


@Component({
  selector: 'app-detail-item',
  templateUrl: './detail-item.component.html',
  styleUrls: ['./detail-item.component.css']
})
export class DetailItemComponent implements OnInit {
  
  public newId: string = "";
  public details: any=[];
  public Uid:string = "";
  public user: LoggedInUser;
  


  constructor(
    private route: ActivatedRoute,
    private _dataService: DataService,
    private _notificationService: NotificationService,
  ) {
  }

  ngOnInit() {
    this.user = JSON.parse(localStorage.getItem(SystemConstants.CURRENT_USER));
    this.loadData();
  }

  
  sliderConfigDetail = { "slidesToShow": 2.5, "slidesToScroll": 1, "arrows": true, "autoplay":true, "autoplaySpeed": 500 };
  afterChange(e) {
    console.log('afterChange');
  }

  // load data
  loadData() {
    this.newId = this.route.snapshot.paramMap.get("id");
    this._dataService.get("/Service/getPostServiceById?idService="+this.newId)
      .subscribe((response: any) => {
        this.details = response;
        this.Uid = response.userId;
        this.details.userId="df516b56-04cc-41a8-c63c-08d7e6c2695a";

      });
  }
}
