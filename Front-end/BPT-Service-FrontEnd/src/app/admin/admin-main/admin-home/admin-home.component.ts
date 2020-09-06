import { Component, OnInit, ViewChild } from '@angular/core';
import { DataService } from 'src/app/core/services/data.service';
import { NotificationService } from 'src/app/core/services/notification.service';
import { Ng4LoadingSpinnerService } from 'ng4-loading-spinner';
import { ModalDirective } from 'ngx-bootstrap/modal';

@Component({
  selector: 'app-home',
  templateUrl: './admin-home.component.html',
})
export class HomeComponent implements OnInit {
  @ViewChild("modalAddEdit", { static: false })
  public modalAddEdit: ModalDirective;
  constructor(
    private _dataService: DataService,
    private _notificationService: NotificationService,
  ) { }

  ngOnInit() {
  }


  showAddModal() {
    this.modalAddEdit.show();
  }
}
