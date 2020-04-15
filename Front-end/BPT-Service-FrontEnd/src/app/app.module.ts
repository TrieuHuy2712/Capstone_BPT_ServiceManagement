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
    BrowserAnimationsModule
  ],
  providers: [AuthGuard,  LanguageService,AuthMainGuard,
    TranslationService],
  bootstrap: [AppComponent]
})
export class AppModule { }
