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
using BPT_Service.Application.CommentService.Command.AddCommentServiceAsync;
using BPT_Service.Application.CommentService.Command.DeleteCommentServiceAsync;
using BPT_Service.Application.CommentService.Command.UpdateCommentServiceAsync;
using BPT_Service.Application.CommentService.Query.GetCommentServiceByIDAsync;
using BPT_Service.Application.EmailService.Command.AddNewEmailService;
using BPT_Service.Application.EmailService.Command.DeleteEmailService;
using BPT_Service.Application.EmailService.Command.UpdateNewEmailService;
using BPT_Service.Application.EmailService.Query.GetAllEmailService;
using BPT_Service.Application.EmailService.Query.GetAllPagingEmailService;
using BPT_Service.Application.EmailService.Query.GetEmailByIdService;
using BPT_Service.Application.FollowingPostService.Command.FollowPostService;
using BPT_Service.Application.FollowingPostService.Command.UnFollowPostService;
using BPT_Service.Application.FollowingPostService.Query.GetFollowByPost;
using BPT_Service.Application.FollowingPostService.Query.GetFollowByUser;
using BPT_Service.Application.FollowingProviderService.Command.FollowProviderService;
using BPT_Service.Application.FollowingProviderService.Command.RegisterEmailProviderService;
using BPT_Service.Application.FollowingProviderService.Command.UnFollowProviderService;
using BPT_Service.Application.FollowingProviderService.Query.GetFollowByProvider;
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
using BPT_Service.Application.LocationService.Command.AddCityProvinceService;
using BPT_Service.Application.LocationService.Command.DeleteCityProvinceService;
using BPT_Service.Application.LocationService.Command.UpdateCityProvinceService;
using BPT_Service.Application.LocationService.Query.GetAllCityProvinceService;
using BPT_Service.Application.LocationService.Query.GetAllPagingCityProvinceService;
using BPT_Service.Application.LocationService.Query.GetByIdCityProvinceService;
using BPT_Service.Application.NewsProviderService.Command.ApproveNewsProvider;
using BPT_Service.Application.NewsProviderService.Command.DeleteNewsProviderService;
using BPT_Service.Application.NewsProviderService.Command.RegisterNewsProviderService;
using BPT_Service.Application.NewsProviderService.Command.RejectNewsProvider;
using BPT_Service.Application.NewsProviderService.Command.UpdateNewsProviderService;
using BPT_Service.Application.NewsProviderService.Query.GetAllPagingProviderNewsOfProvider;
using BPT_Service.Application.NewsProviderService.Query.GetAllPagingProviderNewsService;
using BPT_Service.Application.NewsProviderService.Query.GetByIdProviderNewsService;
using BPT_Service.Application.PermissionService.Query.CheckOwnService;
using BPT_Service.Application.PermissionService.Query.CheckUserIsAdmin;
using BPT_Service.Application.PermissionService.Query.GetPermissionAction;
using BPT_Service.Application.PermissionService.Query.GetPermissionRole;
using BPT_Service.Application.PermissionService.Query.GetPermissionRoleQuery;
using BPT_Service.Application.PostService.Command.ApprovePostService;
using BPT_Service.Application.PostService.Command.PostServiceFromProvider.DeleteServiceFromProvider;
using BPT_Service.Application.PostService.Command.PostServiceFromProvider.RegisterServiceFromProvider;
using BPT_Service.Application.PostService.Command.PostServiceFromUser.DeleteServiceFromUser;
using BPT_Service.Application.PostService.Command.PostServiceFromUser.RegisterServiceFromUser;
using BPT_Service.Application.PostService.Command.RejectPostService;
using BPT_Service.Application.PostService.Command.UpdatePostService;
using BPT_Service.Application.PostService.Query.Extension.GetAvtInformation;
using BPT_Service.Application.PostService.Query.Extension.GetListTagInformation;
using BPT_Service.Application.PostService.Query.Extension.GetProviderInformation;
using BPT_Service.Application.PostService.Query.Extension.GetServiceRating;
using BPT_Service.Application.PostService.Query.Extension.GetUserInformation;
using BPT_Service.Application.PostService.Query.FilterAllPagingPostService;
using BPT_Service.Application.PostService.Query.GetAllPagingPostService;
using BPT_Service.Application.PostService.Query.GetPostServiceById;
using BPT_Service.Application.PostService.Query.GetPostUserServiceByUserId;
using BPT_Service.Application.ProviderService.Command.ApproveProviderService;
using BPT_Service.Application.ProviderService.Command.DeleteProviderService;
using BPT_Service.Application.ProviderService.Command.RegisterProviderService;
using BPT_Service.Application.ProviderService.Command.RejectProviderService;
using BPT_Service.Application.ProviderService.Command.UpdateProviderService;
using BPT_Service.Application.ProviderService.Query.CheckUserIsProvider;
using BPT_Service.Application.ProviderService.Query.GetAllPagingProviderService;
using BPT_Service.Application.ProviderService.Query.GetAllProviderofUserService;
using BPT_Service.Application.ProviderService.Query.GetByIdProviderService;
using BPT_Service.Application.RatingService.Command.AddRatingService;
using BPT_Service.Application.RatingService.Command.DeleteRatingService;
using BPT_Service.Application.RatingService.Query.GetAllPagingRatingServiceByOwner;
using BPT_Service.Application.RatingService.Query.GetAllServiceRatingByUser;
using BPT_Service.Application.RatingService.Query.GetListAllPagingRatingService;
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
using BPT_Service.Common.Dtos;
using BPT_Service.Common.Helpers;
using BPT_Service.Common.Support;
using BPT_Service.Data;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Infrastructure.Interfaces;
using Hangfire;
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
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

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
            services.AddHangfire(x => x.UseSqlServerStorage("Data Source=6YLCMH2\\SQLEXPRESS;Initial Catalog=HangFireTutorial;Integrated Security=True"));
            services.AddHangfireServer();
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
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = @"JWT Authorization header using the Bearer scheme. \r\n\r\n
                      Enter 'Bearer' [space] and then your token in the text input below.
                      \r\n\r\nExample: 'Bearer 12345abcdef'",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(
                    new OpenApiSecurityRequirement(){
                    {
                        new OpenApiSecurityScheme{
                        Reference = new OpenApiReference
                          {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                          },
                          Scheme = "oauth2",
                          Name = "Bearer",
                          In = ParameterLocation.Header,
                        },
                        new List<string>()
                      }
                    });
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
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
            ApplicationContext(services);

            //Read email config json
            services.Configure<EmailConfigModel>(Configuration.GetSection("EmailConfig"));
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

            app.UseHangfireDashboard();
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
            // Enable middleware to serve generated Swagger as a JSON endpoint
            app.UseSwagger();

            // Enable middleware to serve swagger-ui assets (HTML, JS, CSS etc.)
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });
        }

        public void ApplicationContext(IServiceCollection services)
        {

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
            services.AddScoped<ICheckOwnService, CheckOwnService>();

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
            services.AddScoped<IDeleteServiceFromUserCommand, DeleteServiceFromUserCommand>();
            services.AddScoped<IFilterAllPagingPostServiceQuery, FilterAllPagingPostServiceQuery>();
            services.AddScoped<IGetAllPagingPostServiceQuery, GetAllPagingPostServiceQuery>();
            services.AddScoped<IGetPostServiceByIdQuery, GetPostServiceByIdQuery>();
            services.AddScoped<IRegisterServiceFromProviderCommand, RegisterServiceFromProviderCommand>();
            services.AddScoped<IRegisterServiceFromUserCommand, RegisterServiceFromUserCommand>();
            services.AddScoped<IRejectPostServiceCommand, RejectPostServiceCommand>();
            services.AddScoped<IUpdatePostServiceCommand, UpdatePostServiceCommand>();
            services.AddScoped<IGetPostUserServiceByUserIdQuery, GetPostUserServiceByUserIdQuery>();
            //Extension
            services.AddScoped<IGetAvtInformationQuery, GetAvtInformationQuery>();
            services.AddScoped<IGetListTagInformationQuery, GetListTagInformationQuery>();
            services.AddScoped<IGetProviderInformationQuery, GetProviderInformationQuery>();
            services.AddScoped<IGetServiceRatingQuery, GetServiceRatingQuery>();
            services.AddScoped<IGetUserInformationQuery, GetUserInformationQuery>();

            //NewsProvider
            services.AddScoped<IApproveNewsProviderServiceCommand, ApproveNewsProviderServiceCommand>();
            services.AddScoped<IDeleteNewsProviderServiceCommand, DeleteNewsProviderServiceCommand>();
            services.AddScoped<IGetAllPagingProviderNewsOfProviderQuery, GetAllPagingProviderNewsOfProviderQuery>();
            services.AddScoped<IGetAllPagingProviderNewsServiceQuery, GetAllPagingProviderNewsServiceQuery>();
            services.AddScoped<IGetByIdProviderNewsServiceQuery, GetByIdProviderNewsServiceQuery>();
            services.AddScoped<IRegisterNewsProviderServiceCommand, RegisterNewsProviderServiceCommand>();
            services.AddScoped<IRejectNewsProviderServiceCommand, RejectNewsProviderServiceCommand>();
            services.AddScoped<IUpdateNewsProviderServiceCommand, UpdateNewsProviderServiceCommand>();

            //Location Service
            services.AddScoped<IAddCityProvinceServiceCommand, AddCityProvinceServiceCommand>();
            services.AddScoped<IDeleteCityProvinceServiceCommand, DeleteCityProvinceServiceCommand>();
            services.AddScoped<IGetAllCityProvinceServiceQuery, GetAllCityProvinceServiceQuery>();
            services.AddScoped<IGetAllPagingCityProvinceServiceQuery, GetAllPagingCityProvinceServiceQuery>();
            services.AddScoped<IGetByIdCityProvinceServiceQuery, GetByIdCityProvinceServiceQuery>();
            services.AddScoped<IUpdateCityProvinceServiceCommand, UpdateCityProvinceServiceCommand>();

            //Comment service
            services.AddScoped<IGetCommentServiceByIDAsyncQuery, GetCommentServiceByIDAsyncQuery>();
            services.AddScoped<IAddCommentServiceAsyncCommand, AddCommentServiceAsyncCommand>();

            //Follow service
            services.AddScoped<IFollowPostServiceCommand, FollowPostServiceCommand>();
            services.AddScoped<IGetFollowByPostQuery, GetFollowByPostQuery>();
            services.AddScoped<IGetFollowByUserQuery, GetFollowByUserQuery>();
            services.AddScoped<IUnFollowPostServiceCommand, UnFollowPostServiceCommand>();

            //Follow provider
            services.AddScoped<IFollowProviderServiceCommand, FollowProviderServiceCommand>();
            services.AddScoped<IUnFollowProviderServiceCommand, UnFollowProviderServiceCommand>();
            services.AddScoped<IRegisterEmailProviderServiceCommand, RegisterEmailProviderServiceCommand>();
            services.AddScoped<IGetFollowByProviderQuery, GetFollowByProviderQuery>();
            services.AddScoped<IGetFollowByUserQuery, GetFollowByUserQuery>();

            //Comment
            services.AddScoped<IGetCommentServiceByIDAsyncQuery, GetCommentServiceByIDAsyncQuery>();
            services.AddScoped<IAddCommentServiceAsyncCommand, AddCommentServiceAsyncCommand>();
            services.AddScoped<IUpdateCommentServiceAsyncCommand, UpdateCommentServiceAsyncCommand>();
            services.AddScoped<IDeleteCommentServiceAsyncCommand, DeleteCommentServiceAsyncCommand>();

            //Email
            services.AddScoped<IAddNewEmailServiceCommand, AddNewEmailServiceCommand>();
            services.AddScoped<IUpdateNewEmailServiceCommand, UpdateNewEmailServiceCommand>();
            services.AddScoped<IDeleteEmailServiceCommand, DeleteEmailServiceCommand>();
            services.AddScoped<IGetAllEmailServiceQuery, GetAllEmailServiceQuery>();
            services.AddScoped<IGetAllPagingEmailServiceQuery, GetAllPagingEmailServiceQuery>();
            services.AddScoped<IGetEmailByIdService, GetEmailByIdService>();

            //Rating service
            services.AddScoped<IAddUpdateRatingServiceCommand, AddUpdateRatingServiceCommand>();
            services.AddScoped<IDeleteRatingServiceCommand, DeleteRatingServiceCommand>();
            services.AddScoped<IGetAllPagingRatingServiceByOwnerQuery, GetAllPagingRatingServiceByOwnerQuery>();
            services.AddScoped<IGetAllServiceRatingByUserQuery, GetAllServiceRatingByUserQuery>();
            services.AddScoped<IGetListAllPagingRatingServiceQuery, GetListAllPagingRatingServiceQuery>();

            //Another service
            services.AddScoped<RandomSupport, RandomSupport>();
            services.AddScoped<RemoveSupport, RemoveSupport>();
            services.AddScoped<LevenshteinDistance, LevenshteinDistance>();
        }
    }
}