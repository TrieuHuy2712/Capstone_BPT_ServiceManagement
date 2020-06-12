export class LoggedInUser {
    constructor( token: string, username: string, fullName: string, email: string, avatar: string,roles:any,permissions:any, isProvider:any
    ) {
        this.token = token;
        this.fullName = fullName;
        this.username = username;
        this.email = email;
        this.avatar = avatar;
        this.roles = roles;
        this.permissions = permissions;
        this.isProvider = isProvider;
    }
    public Id: string;
    public token: string;
    public username: string;
    public fullName: string;
    public email: string;
    public avatar: string;
    public permissions:any;
    public roles: any;
    public isProvider: any;
}