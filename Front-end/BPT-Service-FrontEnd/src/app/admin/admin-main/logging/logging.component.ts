import { Component, OnInit, ViewChild } from '@angular/core';
import { DataService } from '../../../core/services/data.service';
import { NotificationService } from '../../../core/services/notification.service';
import { Ng4LoadingSpinnerService } from 'ng4-loading-spinner';
import { ModalDirective } from 'ngx-bootstrap';


@Component({
  selector: 'app-logging',
  templateUrl: './logging.component.html',
})
export class LoggingComponent implements OnInit {

  @ViewChild("modalAddEdit", { static: false })
  public modalAddEdit: ModalDirective;
  public listNameLog: any;
  public detailFile: any;
  public currentFile: string;
  public currentStatus: string = "ALL";
  public entity:any;
  constructor(
    private _dataService: DataService,
    private _notificationService: NotificationService,
    private spinnerService: Ng4LoadingSpinnerService
  ) { }

  ngOnInit() {
    this.loadData();
  }
  loadData() {
    this.spinnerService.show();
    this._dataService.get("/logging/GetLogFiles").subscribe((response: any) => {
      this.listNameLog = response;
      this.currentFile = this.listNameLog[0];
      this._dataService.get("/logging/GetLogFromAFile?dataLog=" + this.currentFile + "&type=" + this.currentStatus).subscribe((response1: any) => {
        this.detailFile = response1;
      })
    })
  }
  filterChanged(file: any) {
    this.currentFile = file;
    this._dataService.get("/logging/GetLogFromAFile?dataLog=" + this.currentFile + "&type=" + this.currentStatus).subscribe((response: any) => {
      this.detailFile = response;
    })
  }

  filterStatus(status: any) {
    this.currentStatus = status;
    this._dataService.get("/logging/GetLogFromAFile?dataLog=" + this.currentFile + "&type=" + this.currentStatus).subscribe((response: any) => {
      this.detailFile = response;
    })
  }
  loadRole(id: any) {
    let findIdthis = this.detailFile.logs[id];
    this.entity = findIdthis;
  }
  showEditModal(id: any) {
    this.loadRole(id);
    this.modalAddEdit.show();
  }


}
