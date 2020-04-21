import { Component, OnInit, ViewChild } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { LoggedInUser } from 'src/app/core/domain/loggedin.user';
import { SystemConstants } from 'src/app/core/common/system,constants';

@Component({
  selector: 'app-user-profile',
  templateUrl: './user-profile.component.html',
  styleUrls: ['./user-profile.component.css']
})
export class UserProfileComponent implements OnInit {
  @ViewChild(ModalDirective, {static: false})
  public modalAddEdit: ModalDirective;
  public entity:any;
  public profile: LoggedInUser;
  public id:string;

  constructor() { }

  ngOnInit() {
    this.profile = JSON.parse(localStorage.getItem(SystemConstants.CURRENT_USER));
    SystemConstants.const_permission = this.profile.username;
    
  }
  showAddModal() {
    this.entity = {};
    this.modalAddEdit.show();
    
  }

}
