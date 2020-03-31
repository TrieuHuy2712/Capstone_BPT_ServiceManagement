import { Component, OnInit, ViewChild } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';

@Component({
  selector: 'app-user-profile',
  templateUrl: './user-profile.component.html',
  styleUrls: ['./user-profile.component.css']
})
export class UserProfileComponent implements OnInit {
  @ViewChild(ModalDirective, {static: false})
  public modalAddEdit: ModalDirective;
  public entity:any;
  constructor() { }

  ngOnInit() {
  }
  showAddModal() {
    this.entity = {};
    this.modalAddEdit.show();
  }

}
