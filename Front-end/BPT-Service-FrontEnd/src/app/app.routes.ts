import { Routes } from '@angular/router';
import { AuthGuard } from './core/guards/auth.guard';
export const appRoutes: Routes = [
    //localhost:4200
    // { path: '', redirectTo: 'login', pathMatch: 'full' },
    { path: '', redirectTo: 'main/home', pathMatch: 'full' },
    //localhost:4200/login
    { path: 'login', loadChildren: './login/login.module#LoginModule' },
    //localhost:4200/main
    // { path: 'main', loadChildren: './main/main.module#MainModule',canActivate:[AuthGuard] },
    { path: 'main', loadChildren: './main/main.module#MainModule'},
    { path: 'register', loadChildren: './register/register.module#RegisterModule' },
    { path: 'admin', loadChildren: './admin/admin.module#AdminModule' },
    { path: 'main/home', loadChildren:'./main/home/home.module#HomeModule'}
]