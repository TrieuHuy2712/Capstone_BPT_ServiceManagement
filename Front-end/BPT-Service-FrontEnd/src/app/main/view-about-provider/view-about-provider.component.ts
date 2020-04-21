import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { DataService } from 'src/app/core/services/data.service';
import { NotificationService } from 'src/app/core/services/notification.service';

@Component({
  selector: 'app-view-about-provider',
  templateUrl: './view-about-provider.component.html',
  styleUrls: ['./view-about-provider.component.css']
})
export class ViewAboutProviderComponent implements OnInit {
  userId:string;
  private user: any;


  constructor(
    private route: ActivatedRoute,
    private _dataService: DataService,
    private _notificationService: NotificationService,
  ) { }

  ngOnInit() {
    this.userId = this.route.snapshot.paramMap.get("id");
    console.log("ket qua ne "+this.userId);
    this.loadData();
  }

  loadData() {

    this._dataService
      .get(
        "/UserManagement/getById?id="+this.userId          
      )
      .subscribe((response: any) => {
        this.user = response;
        
      });
  }

}
