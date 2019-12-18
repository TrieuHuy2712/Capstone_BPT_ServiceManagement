import { Component, OnInit, ViewChild } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { DataService } from 'src/app/core/services/data.service';
import { NotificationService } from 'src/app/core/services/notification.service';
import { MessageConstants } from 'src/app/core/common/message.constants';
@Component({
  selector: 'app-role',
  templateUrl: './role.component.html',
  styleUrls: ['./role.component.css']
})
export class RoleComponent implements OnInit {
  @ViewChild('modalAddEdit',{static: false}) public modalAddEdit: ModalDirective;
  public pageIndex: number = 1;
  public pageSize: number = 20;
  public pageDisplay: number = 10;
  public totalRow: number;
  public filter: string = '';
  public roles: any[];
  public entity: any;
  constructor(private _dataService: DataService, private _notificationService: NotificationService) { }

  ngOnInit() {
    this.loadData();
  }

  loadData() {
    this._dataService.get('/AdminRole/GetAllPaging?page=' + this.pageIndex + '&pageSize=' + this.pageSize + '&keyword=' + this.filter)
      .subscribe((response: any) => {
        console.log(response);
        this.roles = response.results;
        this.pageIndex = response.currentPage;
        this.pageSize = response.pageSize;
        this.totalRow = response.rowCount;
      });
  }
  pageChanged(event: any): void {
    this.pageIndex = event.page;
    this.loadData();
  }
  showAddModal() {
    this.entity = {};
    this.modalAddEdit.show();
  }
  loadRole(id: any) {
    this._dataService.get('/AdminRole/GetById/' + id)
      .subscribe((response: any) => {
        this.entity = response;
        console.log(this.entity);
      });
  }
  showEditModal(id: any) {
    this.loadRole(id);
    console.log(id);
    this.modalAddEdit.show();
  }
  saveChange(valid: boolean) {
    debugger
    if (valid) {
      if (this.entity.Id == undefined) {
        console.log('vo day duoc')
        this._dataService.post('/AdminRole/SaveEntity', this.entity)
          .subscribe((response: any) => {
            this.loadData();
            this.modalAddEdit.hide();
            this._notificationService.printSuccessMessage(MessageConstants.CREATED_OK_MSG);
          }, error => this._dataService.handleError(error));
      } else {
        this._dataService.post('/AdminRole/SaveEntity', this.entity)
          .subscribe((response: any) => {
            this.loadData();
            this.modalAddEdit.hide();
            this._notificationService.printSuccessMessage(MessageConstants.UPDATED_OK_MSG);
          }, error => this._dataService.handleError(error));
      }
    }
  }
  deleteItem(id: any) {
    this._notificationService.printConfirmationDialog(MessageConstants.CONFIRM_DELETE_MSG, () => this.deleteItemConfirm(id));
  }
  deleteItemConfirm(id: any) {
      this._dataService.delete('/AdminRole/Delete','id',id).subscribe((response: Response)=>{
        this._notificationService.printSuccessMessage(MessageConstants.DELETED_OK_MSG);
        this.loadData();
      })
  }
}