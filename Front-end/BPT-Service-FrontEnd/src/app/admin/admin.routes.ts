import { Routes } from '@angular/router';
import { AuthMainGuard } from '../core/guards/authMain.guard';
export const adminRoutes: Routes = [
    //localhost:4200/main
    { path: '', redirectTo: 'adminlogin', pathMatch: 'full' },

    { path: 'adminlogin', loadChildren: './admin-login/admin-login.module#AdminLoginModule' },
    { path: 'main', loadChildren: './admin-main/admin-main.module#AdminMainModule', canActivate: [AuthMainGuard] },
]