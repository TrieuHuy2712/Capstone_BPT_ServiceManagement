import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";
import { LoginComponent } from './login.component';
import { AuthenService } from 'src/app/core/services/authen.service';
import { NotificationService } from 'src/app/core/services/notification.service';
import {Routes, RouterModule} from '@angular/router';
import { FormsModule } from '@angular/forms';
export const routes: Routes = [

  {
    path: '',
    component: LoginComponent
  }
]
@NgModule({
  imports: [CommonModule, FormsModule, RouterModule.forChild(routes)],
  providers: [AuthenService, NotificationService],
  declarations: [LoginComponent]
})
export class LoginModule {}
