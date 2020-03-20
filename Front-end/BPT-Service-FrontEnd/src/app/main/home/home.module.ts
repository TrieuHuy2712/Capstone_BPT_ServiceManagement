import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HomeComponent } from './home.component';
import { Routes, RouterModule } from '@angular/router';
import { SlickModule } from 'ngx-slick';

const homeRoutes: Routes=[
  {path:'', redirectTo:'index',pathMatch:'full'},
  {path:'index', component: HomeComponent}
]

@NgModule({
  declarations: [HomeComponent],
  imports: [
    CommonModule,
    RouterModule.forChild(homeRoutes),
    SlickModule.forRoot()
  ]
})
export class HomeModule { }