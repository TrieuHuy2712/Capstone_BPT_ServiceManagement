import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { AppComponent } from './app.component';
import { AuthGuard } from './core/guards/auth.guard';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';
import { PaginationModule } from 'ngx-bootstrap/pagination';
import { appRoutes } from './app.routes';
import { LanguageService } from './core/services/language.service';
import { TranslationService } from './core/services/translation.service';
import { TypeaheadModule } from 'ngx-bootstrap/typeahead';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { AuthMainGuard } from './core/guards/authMain.guard';
import { BarRatingModule } from "ngx-bar-rating";
import { UploadService } from './core/services/upload.service';


@NgModule({
  declarations: [
    AppComponent
  ],
  imports: [
    BrowserModule,
    FormsModule,
    HttpClientModule,
    RouterModule.forRoot(appRoutes),
    PaginationModule.forRoot(),
    TypeaheadModule.forRoot(),
    BrowserAnimationsModule,
    BarRatingModule
  ],
  providers: [AuthGuard, LanguageService, AuthMainGuard, UploadService,
    TranslationService],
  bootstrap: [AppComponent]
})
export class AppModule { }
