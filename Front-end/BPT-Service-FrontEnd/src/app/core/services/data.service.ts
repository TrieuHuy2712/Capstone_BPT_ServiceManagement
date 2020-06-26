import { Injectable } from '@angular/core';
import {HttpClient, HttpHeaders} from '@angular/common/http';
import {Router} from "@angular/router";
import { AuthenService } from './authen.service';
import { map } from 'rxjs/operators';
import {Observable} from 'rxjs';
import {MessageConstants} from './../common/message.constants';
import {NotificationService} from './notification.service';
import { SystemConstants } from '../common/system,constants';
import { UtilityService } from './utility.service';
@Injectable({
  providedIn: 'root'
})
export class DataService {
 private headers: HttpHeaders;
  constructor(private _http: HttpClient, private _router:Router,private _authenService: AuthenService,
    private _notificationService: NotificationService,private _utilityService : UtilityService) {
      this.headers=new HttpHeaders();
      this.headers.set('Content-Type','application/json');
     }
  get(uri:string){
    this.headers.delete("Authorization");
    this.headers.append("Authorization","Bearer "+this._authenService.getLoggedInUser().token);
    return this._http.get(SystemConstants.BASE_API+uri,
      {headers:new HttpHeaders().set('Content-Type', 'application/json').set('Authorization', 'Bearer ' + this._authenService.getLoggedInUser().token)}).pipe(map(this.extractData));
  }

  // get no authentication
  getNoAu(uri:string){
    return this._http.get(SystemConstants.BASE_API+uri);
  }

  post(uri:string,data?:any): Observable<any>{
    this.headers.delete("Authorization");
    this.headers.append("Authorization","Bearer "+this._authenService.getLoggedInUser().token);
    return this._http.post(SystemConstants.BASE_API+uri,data,
      {headers:new HttpHeaders().set('Content-Type', 'application/json').set('Authorization', 'Bearer ' + this._authenService.getLoggedInUser().token)}).pipe(map(this.extractData));
  }
  postNoAu(uri:string,data?:any): Observable<any>{
    return this._http.post(SystemConstants.BASE_API+uri,data)
      
  }
  put(uri:string, data?:any){
    this.headers.delete("Authorization");
    this.headers.append("Authorization","Bearer "+this._authenService.getLoggedInUser().token);
    return this._http.put(SystemConstants.BASE_API+uri,data,
      {headers:new HttpHeaders().set('Content-Type', 'application/json').set('Authorization', 'Bearer ' + this._authenService.getLoggedInUser().token)}).pipe(map(this.extractData));
  }
  putStatus(uri:string,id:string){
    this.headers.delete("Authorization");
    this.headers.append("Authorization","Bearer "+this._authenService.getLoggedInUser().token);
    return this._http.put(SystemConstants.BASE_API+uri+"/"+id,
    {headers:new HttpHeaders().set('Content-Type', 'application/json').set('Authorization', 'Bearer ' + this._authenService.getLoggedInUser().token)}).pipe(map(this.extractData));
  }
  delete(uri:string,key:string,id:string){
    this.headers.delete("Authorization");
    this.headers.append("Authorization","Bearer "+this._authenService.getLoggedInUser().token);
    return this._http.delete(SystemConstants.BASE_API+uri+"?"+key+"="+id,
    {headers:new HttpHeaders().set('Content-Type', 'application/json').set('Authorization', 'Bearer ' + this._authenService.getLoggedInUser().token)}).pipe(map(this.extractData));
  }
  deleteWithMultiParams(uri: string, params) {
    this.headers.delete('Authorization');

    this.headers.append("Authorization", "Bearer " + this._authenService.getLoggedInUser().token);
    var paramStr: string = '';
    for (let param in params) {
      paramStr += param + "=" + params[param] + '&';
    }
    return this._http.delete(SystemConstants.BASE_API + uri + "/?" + paramStr, 
    {headers:new HttpHeaders().set('Content-Type', 'application/json').set('Authorization', 'Bearer ' + this._authenService.getLoggedInUser().token)}).pipe(map(this.extractData));

  }
  postFile(uri:string, data?:any){
    let newHeader = new HttpHeaders();
    newHeader.append("Authorization", "Bearer " + this._authenService.getLoggedInUser().token);
    return this._http.post(SystemConstants.BASE_API + uri, data, 
      {headers:new HttpHeaders().set('Authorization', 'Bearer ' + this._authenService.getLoggedInUser().token)}).pipe(map(this.extractData));
  }
  private extractData(res:Response){
    let body = res;
    return body || {};
  }
  public handleError(error: any) {
    if (error.status == 401) {
      localStorage.removeItem(SystemConstants.CURRENT_USER);
      this._notificationService.printErrorMessage(MessageConstants.LOGIN_AGAIN_MSG);
      this._utilityService.navigateToLogin();
    }
    else if (error.status == 403) {
      localStorage.removeItem(SystemConstants.CURRENT_USER);
      this._notificationService.printErrorMessage(MessageConstants.FORBIDDEN);
      this._utilityService.navigateToLogin();
    }
    else {
      let errMsg = JSON.parse(error._body).Message;
      this._notificationService.printErrorMessage(errMsg);

      return Observable.throw(errMsg);
    }
  }
}