import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, Router } from '@angular/router';
import { UrlConstants } from '../../core/common/url.constants';
import { Injectable } from '@angular/core';
import { SystemConstants } from '../common/system,constants';

@Injectable()
export class AuthMainGuard implements CanActivate {
    constructor(private router: Router,
        ) {

    }
    canActivate(activateRoute: ActivatedRouteSnapshot, routerState: RouterStateSnapshot) {
        if (localStorage.getItem(SystemConstants.CURRENT_USER) && JSON.parse(localStorage.getItem(SystemConstants.CURRENT_USER)).status==1) {
            return true;
        }
        else {
            alert("You are currently not logged in, please provide Login!")
            this.router.navigate([UrlConstants.LOGIN], {
                queryParams: {
                    returnUrl: routerState.url
                }
            });
            return false;
        }
    }
}