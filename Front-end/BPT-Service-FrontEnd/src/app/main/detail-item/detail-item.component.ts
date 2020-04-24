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
  message: string;
  newId: string = "";
  private details: any[];
  Uid:string = "";
  public user: LoggedInUser;
  


  constructor(
    private route: ActivatedRoute,
    private _dataService: DataService,
    private _notificationService: NotificationService,
  ) {
  }

  ngOnInit() {
    this.newId = this.route.snapshot.paramMap.get("id");
    console.log("ket qua ne "+this.newId);
    this.loadData();
    this.user = JSON.parse(localStorage.getItem(SystemConstants.CURRENT_USER));
    SystemConstants.const_permission = this.user.username;
    
  }
  sliderConfigDetail = { "slidesToShow": 2.5, "slidesToScroll": 1, "arrows": true, "autoplay":true, "autoplaySpeed": 500 };
  afterChange(e) {
    console.log('afterChange');
  }

  // load data
  loadData() {

    this._dataService
      .get(
        "/Service/getPostServiceById?idService="+this.newId          
      )
      .subscribe((response: any) => {
        this.details = response.myModel;
        this.Uid = response.myModel.userId;
      });
  }
}
