import { Component, OnInit } from "@angular/core";
import { AuthenService } from 'src/app/core/services/authen.service';
import { NotificationService } from 'src/app/core/services/notification.service';
import { Router } from '@angular/router';
import { UrlConstants } from 'src/app/core/common/url.constants';

@Component({
  selector: "app-login",
  templateUrl: "./login.component.html",
  styleUrls: ["./login.component.css"]
})
export class LoginComponent implements OnInit {
  loading = false;
  model: any = {};
  returnUrl: string;
  constructor(
    private authenService: AuthenService,
    private notificationService: NotificationService,
    private router: Router
  ) {}

  ngOnInit() {}
  login() {
    this.loading = true;
    this.authenService
      .login(this.model.username, this.model.password)
      .subscribe(
        data => {
          this.router.navigate([UrlConstants.HOME]);
        },
        error => {
          this.notificationService.printErrorMessage("Có lỗi rồi nhóc");
          this.loading = false;
        }
      );
  }
}
