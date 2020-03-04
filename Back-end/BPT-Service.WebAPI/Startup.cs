using BPT_Service.Application.AuthenticateService.Command.ResetPasswordAsync;
using BPT_Service.Application.AuthenticateService.Command.ResetPasswordAsyncCommand;
using BPT_Service.Application.AuthenticateService.Query.AuthenticateofAuthenticationService;
using BPT_Service.Application.AuthenticateService.Query.GetAllAuthenticateService;
using BPT_Service.Application.AuthenticateService.Query.GetByIdAuthenticateService;
using BPT_Service.Application.CategoryService.Command.AddCategoryService;
using BPT_Service.Application.CategoryService.Command.DeleteCategoryService;
using BPT_Service.Application.CategoryService.Command.UpdateCategoryService;
using BPT_Service.Application.CategoryService.Query.GetAllAsyncCategoryService;
using BPT_Service.Application.CategoryService.Query.GetAllPagingAsyncCategoryService;
using BPT_Service.Application.CategoryService.Query.GetByIDCategoryService;
using BPT_Service.Application.FunctionService.Command.AddFunctionService;
using BPT_Service.Application.FunctionService.Command.DeleteFunctionService;
using BPT_Service.Application.FunctionService.Command.UpdateFunctionService;
using BPT_Service.Application.FunctionService.Command.UpdateParentId;
using BPT_Service.Application.FunctionService.Query.CheckExistedIdFunctionService;
using BPT_Service.Application.FunctionService.Query.GetAllFunctionService;
using BPT_Service.Application.FunctionService.Query.GetAllWithParentIdFunctionService;
using BPT_Service.Application.FunctionService.Query.GetByIdFunctionService;
using BPT_Service.Application.FunctionService.Query.GetListFunctionWithPermission;
using BPT_Service.Application.FunctionService.Query.ReOrderFunctionService;
using BPT_Service.Application.PermissionService.Query.GetPermissionRole;
using BPT_Service.Application.PermissionService.Query.GetPermissionRoleQuery;
using BPT_Service.Application.ProviderService.Command.ApproveProviderService;
using BPT_Service.Application.ProviderService.Command.DeleteProviderService;
using BPT_Service.Application.ProviderService.Command.RegisterProviderService;
using BPT_Service.Application.ProviderService.Command.RejectProviderService;
using BPT_Service.Application.ProviderService.Query.GetAllPagingProviderService;
using BPT_Service.Application.ProviderService.Query.GetAllProviderofUserService;
using BPT_Service.Application.ProviderService.Query.GetByIdProviderService;
using BPT_Service.Application.RoleService.Command.AddRoleAsync;
using BPT_Service.Application.RoleService.Command.DeleteRoleAsync;
using BPT_Service.Application.RoleService.Command.SavePermissionRole;
using BPT_Service.Application.RoleService.Command.UpdateRoleAsync;
using BPT_Service.Application.RoleService.Query.GetAllAsync;
using BPT_Service.Application.RoleService.Query.GetAllPagingAsync;
using BPT_Service.Application.RoleService.Query.GetAllPermission;
using BPT_Service.Application.RoleService.Query.GetByIdAsync;
using BPT_Service.Application.RoleService.Query.GetListFunctionWithRole;
using BPT_Service.Application.TagService.Command.AddServiceAsync;
using BPT_Service.Application.TagService.Command.DeleteServiceAsync;
using BPT_Service.Application.TagService.Command.UpdateTagServiceAsync;
using BPT_Service.Application.TagService.Query.GetAllPagingServiceAsync;
using BPT_Service.Application.TagService.Query.GetAllServiceAsync;
using BPT_Service.Application.TagService.Query.GetByIDTagServiceAsync;
using BPT_Service.Application.UserService.Command.AddCustomerAsync;
using BPT_Service.Application.UserService.Command.AddExternalAsync;
using BPT_Service.Application.UserService.Command.AddUserAsync;
using BPT_Service.Application.UserService.Command.DeleteUserAsync;
using BPT_Service.Application.UserService.Command.UpdateUserAsync;
using BPT_Service.Application.UserService.Query.GetAllAsync;
using BPT_Service.Application.UserService.Query.GetAllPagingAsync;
using BPT_Service.Application.UserService.Query.GetByIdAsync;
using BPT_Service.Common.Helpers;
using BPT_Service.Common.Support;
using BPT_Service.Data;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System;
using BPT_Service.Application.PermissionService.Query.GetPermissionAction;
using BPT_Service.Application.ProviderService.Query.CheckUserIsProvider;
using BPT_Service.Application.PostService.Command.ApprovePostService;
using BPT_Service.Application.PostService.Command.PostServiceFromProvider.DeleteServiceFromProvider;
using BPT_Service.Application.PostService.Command.PostServiceFromProvider.RegisterServiceFromProvider;
using BPT_Service.Application.PostService.Command.PostServiceFromUser.DeleteServiceFromUser;
using BPT_Service.Application.PostService.Command.PostServiceFromUser.RegisterServiceFromUser;
using BPT_Service.Application.PostService.Command.RejectPostService;
using BPT_Service.Application.PostService.Command.UpdatePostService;
using BPT_Service.Application.PostService.Query.GetAllPagingPostService;
using BPT_Service.Application.PostService.Query.GetPostServiceById;
using BPT_Service.Application.ProviderService.Command.UpdateProviderService;
using BPT_Service.Application.NewsProviderService.Command.ApproveNewsProvider;
using BPT_Service.Application.NewsProviderService.Command.DeleteNewsProviderService;
using BPT_Service.Application.NewsProviderService.Command.RegisterNewsProviderService;
using BPT_Service.Application.NewsProviderService.Command.RejectNewsProvider;
using BPT_Service.Application.NewsProviderService.Command.UpdateNewsProviderService;
using BPT_Service.Application.NewsProviderService.Query.GetAllPagingProviderNewsOfProvider;
using BPT_Service.Application.NewsProviderService.Query.GetAllPagingProviderNewsService;
using BPT_Service.Application.NewsProviderService.Query.GetByIdProviderNewsService;
using BPT_Service.Application.PermissionService.Query.CheckUserIsAdmin;

namespace BPT_Service.WebAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlite(Configuration.GetConnectionString("DefaultConnection"),
                o => o.MigrationsAssembly("DataEF/BPT-Service.Data")));
            services.AddControllers().AddNewtonsoftJson();
            services.AddIdentity<AppUser, AppRole>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();
            services.AddMvc()
           .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
            services.AddMvc()
                .AddNewtonsoftJson(x => x.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
            services.AddMemoryCache();

            services.AddTransient<DbInitializer>();
            // Configure Identity
            services.Configure<IdentityOptions>(options =>
            {
                // Password settings
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;

                // Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                // User settings
                options.User.RequireUniqueEmail = true;
            });

            // Services 
            services.AddTransient(typeof(IUnitOfWork), typeof(EFUnitOfWork));
            services.AddTransient(typeof(IRepository<,>), typeof(EFRepository<,>));

            services.AddScoped<UserManager<AppUser>, UserManager<AppUser>>();
            services.AddScoped<RoleManager<AppRole>, RoleManager<AppRole>>();
            services.AddScoped<UserManager<IdentityUser>, UserManager<IdentityUser>>();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            //Authenticate service
            services.AddScoped<IResetPasswordAsyncCommand, ResetPasswordAsyncCommand>();
            services.AddScoped<IAuthenticateServiceQuery, AuthenticateServiceQuery>();
            services.AddScoped<IGetAllAuthenticateServiceQuery, GetAllAuthenticateServiceQuery>();
            services.AddScoped<IGetByIdAuthenticateService, GetByIdAuthenticateServiceQuery>();

            //Category service
            services.AddScoped<IAddCategoryServiceCommand, AddCategoryServiceCommand>();
            services.AddScoped<IDeleteCategoryServiceCommand, DeleteCategoryServiceCommand>();
            services.AddScoped<IGetAllAsyncCategoryServiceQuery, GetAllAsyncCategoryServiceQuery>();
            services.AddScoped<IGetAllPagingAsyncCategoryServiceQuery, GetAllPagingAsyncCategoryServiceQuery>();
            services.AddScoped<IGetByIDCategoryServiceQuery, GetByIDCategoryServiceQuery>();
            services.AddScoped<IUpdateCategoryServiceCommand, UpdateCategoryServiceCommand>();

            //Function Service
            services.AddScoped<IAddFunctionServiceCommand, AddFunctionServiceCommand>();
            services.AddScoped<ICheckExistedFunctionServiceQuery, CheckExistedIdFunctionServiceQuery>();
            services.AddScoped<IDeleteFunctionServiceCommand, DeleteFunctionServiceCommand>();
            services.AddScoped<IGetAllFunctionServiceQuery, GetAllFunctionServiceQuery>();
            services.AddScoped<IGetAllWithParentIdFunctionServiceQuery, GetAllWithParentIdFunctionServiceQuery>();
            services.AddScoped<IGetByIdFunctionServiceQuery, GetByIdFunctionServiceQuery>();
            services.AddScoped<IGetListFunctionWithPermissionQuery, GetListFunctionWithPermissionServiceQuery>();
            services.AddScoped<IReOrderFunctionServiceQuery, ReOrderFunctionServiceQuery>();
            services.AddScoped<IUpdateFunctionServiceCommand, UpdateFunctionServiceCommand>();
            services.AddScoped<IUpdateParentIdServiceCommand, UpdateParentIdServiceCommand>();
            //Permission service
            services.AddScoped<IGetPermissionRoleQuery, GetPermissionRoleQuery>();
            services.AddScoped<IGetPermissionActionQuery, GetPermissionActionQuery>();
            services.AddScoped<ICheckUserIsAdminQuery, CheckUserIsAdminQuery>();

            //Role service
            services.AddScoped<IAddRoleAsyncCommand, AddRoleAsyncCommand>();
            services.AddScoped<IDeleteRoleAsyncCommand, DeleteRoleAsyncCommand>();
            services.AddScoped<ISavePermissionCommand, SavePermissionCommand>();
            services.AddScoped<IUpdateRoleAsyncCommand, UpdateRoleAsyncCommand>();
            services.AddScoped<IGetAllPermissionQuery, GetAllPermissionQuery>();
            services.AddScoped<IGetAllRoleAsyncQuery, GetAllRoleAsyncQuery>();
            services.AddScoped<IGetAllRolePagingAsyncQuery, GetAllRolePagingAsyncQuery>();
            services.AddScoped<IGetListFunctionWithRoleQuery, GetListFunctionWithRoleQuery>();
            services.AddScoped<IGetRoleByIdAsyncQuery, GetRoleByIdAsyncQuery>();

            //Tag service
            services.AddScoped<IAddTagServiceAsyncCommand, AddTagServiceAsyncCommand>();
            services.AddScoped<IDeleteTagServiceAsyncCommand, DeleteTagServiceAsyncCommand>();
            services.AddScoped<IUpdateTagServiceAsyncCommand, UpdateTagServiceAsyncCommand>();
            services.AddScoped<IGetAllPagingTagServiceAsyncQuery, GetAllPagingTagServiceAsyncQuery>();
            services.AddScoped<IGetAllTagServiceAsyncQuery, GetAllTagServiceAsyncQuery>();
            services.AddScoped<IGetByIDTagServiceAsyncQuery, GetByIDTagServiceAsyncQuery>();

            //User service
            services.AddScoped<IAddCustomerAsyncCommand, AddCustomerAsyncCommand>();
            services.AddScoped<IAddExternalAsyncCommand, AddExternalAsyncCommand>();
            services.AddScoped<IAddUserAsyncCommand, AddUserAsyncCommand>();
            services.AddScoped<IDeleteUserAsyncCommand, DeleteUserAsyncCommand>();
            services.AddScoped<IUpdateUserAsyncCommand, UpdateUserAsyncCommand>();
            services.AddScoped<IGetAllPagingUserAsyncQuery, GetAllPagingUserAsyncQuery>();
            services.AddScoped<IGetAllUserAsyncQuery, GetAllUserAsyncQuery>();
            services.AddScoped<IGetByIdUserAsyncQuery, GetByIdUserAsyncQuery>();

            //Provider service
            services.AddScoped<IApproveProviderServiceCommand, ApproveProviderServiceCommand>();
            services.AddScoped<ICheckUserIsProviderQuery, CheckUserProviderQuery>();
            services.AddScoped<IDeleteProviderServiceCommand, DeleteProviderServiceCommand>();
            services.AddScoped<IGetAllPagingProviderServiceQuery, GetAllPagingProviderServiceQuery>();
            services.AddScoped<IGetAllProviderofUserServiceQuery, GetAllProviderofUserServiceQuery>();
            services.AddScoped<IGetByIdProviderServiceQuery, GetByIdProviderServiceQuery>();
            services.AddScoped<IRegisterProviderServiceCommand, RegisterProviderServiceCommand>();
            services.AddScoped<IRejectProviderServiceCommand, RejectProviderServiceCommand>();
            services.AddScoped<IUpdateProviderServiceCommand, UpdateProviderServiceCommand>();

            //Post service 
            services.AddScoped<IApprovePostServiceCommand, ApprovePostServiceCommand>();
            services.AddScoped<IDeleteServiceFromProviderCommand, DeleteServiceFromProviderCommand>();
            services.AddScoped<IRegisterServiceFromProviderCommand, RegisterServiceFromProviderCommand>();
            services.AddScoped<IDeleteServiceFromUserCommand, DeleteServiceFromUserCommand>();
            services.AddScoped<IRegisterServiceFromUserCommand, RegisterServiceFromUserCommand>();
            services.AddScoped<IRejectPostServiceCommand, RejectPostServiceCommand>();
            services.AddScoped<IUpdatePostServiceCommand, UpdatePostServiceCommand>();
            services.AddScoped<IGetAllPagingPostServiceQuery, GetAllPagingPostServiceQuery>();
            services.AddScoped<IGetPostServiceByIdQuery, GetPostServiceByIdQuery>();

            //NewsProvider
            services.AddScoped<IApproveNewsProviderServiceCommand, ApproveNewsProviderServiceCommand>();
            services.AddScoped<IDeleteNewsProviderServiceCommand, DeleteNewsProviderServiceCommand>();
            services.AddScoped<IGetAllPagingProviderNewsOfProviderQuery, GetAllPagingProviderNewsOfProviderQuery>();
            services.AddScoped<IGetAllPagingProviderNewsServiceQuery, GetAllPagingProviderNewsServiceQuery>();
            services.AddScoped<IGetByIdProviderNewsServiceQuery,GetByIdProviderNewsServiceQuery>();
            services.AddScoped<IRegisterNewsProviderServiceCommand, RegisterNewsProviderServiceCommand>();
            services.AddScoped<IRejectNewsProviderServiceCommand, RejectNewsProviderServiceCommand>();
            services.AddScoped<IUpdateNewsProviderServiceCommand, UpdateNewsProviderServiceCommand>();

            //Another service
            services.AddScoped<RandomSupport, RandomSupport>();
            services.AddScoped<RemoveSupport, RemoveSupport>();
            // configure strongly typed settings objects
            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);
            // configure jwt authentication
            var appSettings = appSettingsSection.Get<AppSettings>();
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("qwertyuioplkjhgfdsazxcvbnmqwertlkjfdslkjflksjfklsjfklsjdflskjflyuioplkjhgfdsazxcvbnmmnbv")),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHttpContextAccessor accessor)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseCors(x => x
               .AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader());

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
