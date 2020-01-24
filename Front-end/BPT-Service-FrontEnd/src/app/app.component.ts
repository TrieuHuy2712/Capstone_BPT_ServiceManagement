import { Component } from '@angular/core';
import { SystemConstants } from './core/common/system,constants';
import { DataService } from './core/services/data.service';
import { Router } from '@angular/router';
import { LoggedInUser } from './core/domain/loggedin.user';
import { AuthenService } from './core/services/authen.service';
import { NotificationService } from './core/services/notification.service';
import { UrlConstants } from './core/common/url.constants';
import { LanguageService } from './core/services/language.service';

export const loginState = false;

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title = 'BPT-ServiceManagement';
  _functions: any;
  public user: LoggedInUser;
  loading = false;
  isLoginPage: boolean = true;
  constructor(private dataService: DataService, private router:Router, private _authenService: AuthenService,
    
    private notificationService: NotificationService, 
    private languageService: LanguageService) {
    
  }


  ngOnInit() {
    const uName = localStorage.getItem(SystemConstants.const_username);
    if (uName) {
      this.dataService.get(
        "/function/GetAll/" +
        uName
      )
      .subscribe((response: any) => {
        console.log(response);      
        this._functions = response;
        this._functions = this._functions.filter(x => x.key != null);
      });
    };

    

    this.user = this._authenService.getLoggedInUser();
    
    this.router.events.subscribe((e: any) => {
      if (e.urlAfterRedirects && !e.urlAfterRedirects.includes('login')) {
        setTimeout(() => {
          this.isLoginPage = false;
        }, 1);
      }
    });
  }

  toggleMenuItem(id: number) {
    const tmp = document.getElementById('item-' + id);
    const itemContainer = document.getElementById('item-container-' + id);
    
    document.querySelectorAll('.item-container').forEach(i => {
      i.classList.remove('active');
    });

    itemContainer.classList.add('active');

    tmp.classList.toggle('in');
    tmp.classList.toggle('show');
  }
  toLowKey(str:string){
    return str.toLowerCase();
  }
  onChange(deviceValue) {
    console.log(deviceValue);
    this.languageService.setLanguage(deviceValue);
  }

  logout() {
    localStorage.removeItem(SystemConstants.CURRENT_USER);
    localStorage.clear();
    this._authenService.logout();
    localStorage.removeItem('token');
    this.isLoginPage = true;
    this.router.navigate(['login']);
  }
}

