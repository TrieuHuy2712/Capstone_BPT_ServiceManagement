dotnet-ef --startup-project ../../BPT-Service.WebAPI/ migrations add Initial
dotnet ef --startup-project ../../BPT-Service.WebAPI/ database update 
dotnet tool install --global dotnet-ef --version 3.0.0
https://stackoverflow.com/questions/38705694/add-migration-with-different-assembly

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
//Store Procedure
var sp = @"CREATE PROCEDURE [dbo].[DeleteUserRoles]
 @UserIdDelete varchar(36), @RoleIdDelete varchar(36)
AS
BEGIN
	Delete from AppUserRoles where UserId = @UserIdDelete and RoleId= @RoleIdDelete;
END";
            migrationBuilder.Sql(sp);

//Get listEmail by Role

select distinct users.Email from dbo.AppUsers users
inner join dbo.AppUserRoles userRoles
on users.Id = userRoles.UserId
inner join dbo.AppRoles roles
on userRoles.RoleId = roles.Id