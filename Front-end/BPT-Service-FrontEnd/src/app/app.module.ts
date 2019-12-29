import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { AppComponent } from './app.component';
import { AuthGuard } from './core/guards/auth.guard';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';
import {PaginationModule} from 'ngx-bootstrap/pagination';
import { appRoutes } from './app.routes';
import { NotificationService } from './core/services/notification.service';
import { UtilityService } from './core/services/utility.service';

@NgModule({
  declarations: [
    AppComponent,
  ],
  imports: [
    BrowserModule,
    FormsModule,
    HttpClientModule,
    RouterModule.forRoot(appRoutes),
    PaginationModule.forRoot()
  ],
  providers: [AuthGuard, NotificationService,UtilityService],
  bootstrap: [AppComponent]
})
export class AppModule { }
