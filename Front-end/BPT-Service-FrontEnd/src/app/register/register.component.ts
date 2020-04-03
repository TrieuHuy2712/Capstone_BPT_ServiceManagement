import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  email: boolean = true;
  name: boolean = true;
  fullName: boolean = true;
  pass: boolean = true;
  passnd: boolean = true;
  emailVal: string = "";
  nameVal: string = "";
  fullNameVal: string = "";
  passVal: string = "";
  passndVal: string = "";
  registerBtn: boolean = true;
  constructor() { }

  ngOnInit() {
  }

  emailValid(event) {
    this.emailVal = event.target.value;
    if (event.target.value.length < 8) {
      this.email = false;
    }
    else {
      this.email = true;
    }
  }

  nameValid(event) {
    this.nameVal = event.target.value;
    if (event.target.value.length < 8) {
      this.name = false;
    }
    else {
      this.name = true;
    }
  }

  fullNameValid(event) {
    this.fullNameVal = event.target.value;
    if (event.target.value.length < 8) {
      this.fullName = false;
    }
    else {
      this.fullName = true;
    }
  }

  passValid(event) {
    this.passVal = event.target.value;
    if (event.target.value.length < 8) {
      this.pass = false;
    }
    else {
      this.pass = true;
    }
  }

  passndValid(event) {
    this.passndVal = event.target.value;
    if (event.target.value.length < 8) {
      this.passnd = false;
    }
    else {
      this.passnd = true;
    }
    if (event.target.value.length > 8 && this.emailVal.length > 8 && this.nameVal.length > 8 && this.fullNameVal.length > 8 && this.passVal.length > 8) {
      this.registerBtn = false;
    }
    else {
      this.registerBtn = true;
    }
  }
}
