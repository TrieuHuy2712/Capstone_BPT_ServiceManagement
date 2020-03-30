using BPT_Service.Model.Entities;
using BPT_Service.Model.Entities.ServiceModel;
using BPT_Service.Model.Enums;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BPT_Service.Data
{
    public class DbInitializer
    {
        private readonly AppDbContext _context;
        private UserManager<AppUser> _userManager;
        private RoleManager<AppRole> _roleManager;

        public DbInitializer(AppDbContext context, UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task Seed()
        {
            if (!_roleManager.Roles.Any())
            {
                await _roleManager.CreateAsync(new AppRole()
                {
                    Name = "Admin",
                    NormalizedName = "Admin",
                    Description = "Top manager",
                });
                await _roleManager.CreateAsync(new AppRole()
                {
                    Name = "Staff",
                    NormalizedName = "Staff",
                    Description = "Staff",
                });
                await _roleManager.CreateAsync(new AppRole()
                {
                    Name = "Provider",
                    NormalizedName = "Provider",
                    Description = "Provider",
                });
                await _roleManager.CreateAsync(new AppRole()
                {
                    Name = "Customer",
                    NormalizedName = "Customer",
                    Description = "Customer",
                });
            }
            if (!_userManager.Users.Any())
            {
                await _userManager.CreateAsync(new AppUser()
                {
                    UserName = "admin",
                    FullName = "Administrator",
                    Email = "huytrieu2712@gmail.com",
                    DateCreated = DateTime.Now,
                    DateModified = DateTime.Now,
                    Status = Status.Active
                }, "123654$");
                var user = await _userManager.FindByNameAsync("admin");
                await _userManager.AddToRoleAsync(user, "Admin");

                await _userManager.CreateAsync(new AppUser()
                {
                    UserName = "huytrieu",
                    FullName = "Trieu Duc Huy",
                    Email = "tommy.jimm2712@gmail.com",
                    DateCreated = DateTime.Now,
                    DateModified = DateTime.Now,
                    Status = Status.Active
                }, "123654$");
                var user1 = await _userManager.FindByNameAsync("huytrieu");
                await _userManager.AddToRoleAsync(user1, "Staff");
                await _userManager.AddToRoleAsync(user1, "Customer");
                await _userManager.RemoveFromRoleAsync(user1, "Customer");
            }
            if (_context.Categories.Count() == 0)
            {
                List<Category> categories = new List<Category>() {
                    new Category()
                    {
                        CategoryName="Cleaner",
                        Description="This is cleaner",
                    },
                    new Category()
                    {
                        CategoryName="Cleaner 1",
                        Description="This is cleaner 1",
                    }
                };
                await _context.Categories.AddRangeAsync(categories);
                await _context.SaveChangesAsync();
            }
            if (_context.Services.Count() == 0)
            {
                List<Service> services = new List<Service>() {
                    new Service()
                    {
                        ServiceName="Osin",
                        Description="This is Osin",
                        CategoryId=1,
                        
                    }
                };
                await _context.Services.AddRangeAsync(services);
                await _context.SaveChangesAsync();
            }
            if (_context.Functions.Count() == 0)
            {
                List<Function> functions = new List<Function>()
                {
                    new Function() {Id = "SYSTEM", Name = "System",ParentId = null,SortOrder = 1,Status = Status.Active,URL = "/",IconCss = "fa-desktop"},
                    new Function() {Id = "ROLE", Name = "Role",ParentId = "SYSTEM",SortOrder = 1,Status = Status.Active,URL = "/main/role/index",IconCss = "fa-home"},
                    new Function() {Id = "FUNCTION", Name = "Function",ParentId = "SYSTEM",SortOrder = 2,Status = Status.Active,URL = "/main/function/index",IconCss = "fa-home"},
                    new Function() {Id = "USER", Name = "User",ParentId = "SYSTEM",SortOrder =3,Status = Status.Active,URL = "/main/user/index",IconCss = "fa-home"},

                    new Function() {Id = "PROVIDER",Name = "Provider",ParentId = null,SortOrder = 2,Status = Status.Active,URL = "/",IconCss = "fa-chevron-down"},
                    new Function() {Id = "SERVICE_CATEGORY",Name = "Category",ParentId = "PROVIDER",SortOrder =1,Status = Status.Active,URL = "/main/category/index",IconCss = "fa-chevron-down"},
                    new Function() {Id = "SERVICE",Name = "Service",ParentId = "PROVIDER",SortOrder = 2,Status = Status.Active,URL = "/main/product/index",IconCss = "fa-chevron-down"},
                    new Function() {Id = "SERVICE_TAG",Name = "Service_Tag",ParentId = "PROVIDER",SortOrder = 4,Status = Status.Active,URL = "/main/tag/index",IconCss = "fa-chevron-down"},
                };
                await _context.Functions.AddRangeAsync(functions);
                await _context.SaveChangesAsync();
            }
            if (_context.CityProvinces.Count() == 0)
            {
                List<CityProvince> cities = new List<CityProvince>()
                {
                    new CityProvince() {City="Hà Nội",Province="Hà Nội"},
                     new CityProvince() {City="Hồ Chí Minh",Province="Hồ Chí Minh"},
                      new CityProvince() {City="Bảo Lộc",Province="Lâm Đồng"},
                       new CityProvince() {City="Buôn Ma Thuột",Province="Đắk Lắk"},
                        new CityProvince() {City="Đà Lạt",Province="Lâm Đồng"},
                };
                await _context.CityProvinces.AddRangeAsync(cities);
                await _context.SaveChangesAsync();
            }

            if (_context.Emails.Count() == 0)
            {
                List<Email> emails = new List<Email>()
                {
                    new Email()
                    {
                        Subject="Approve Provider Request",
                        Name ="Approve_Provider",
                        Message= "Dear #UserName </br> Your provider has been accepted."
                    },
                    new Email()
                    {
                        Subject="Reject Provider Request",
                        Name ="Reject_Provider",
                        Message= "Dear #UserName </br> Your provider has been rejected.<br> " +
                        "<strong>#Reason</strong>"
                    },
                    new Email()
                    {
                        Subject="Approve Service Request",
                        Name ="Approve_Service",
                        Message= "Dear #UserName </br> Your provider has been accepted."
                    },
                    new Email()
                    {
                        Subject="Reject Provider Request",
                        Name ="Reject_Service",
                        Message= "Dear #ProviderName_#UserName </br> Your provider has been rejected.<br> " +
                        "<strong>#Reason</strong>"
                    },
                    new Email()
                    {
                        Subject="Approve News of your Provider Request",
                        Name ="Approve_News",
                        Message= "Dear #UserName </br> Your #TitleNews news has been accepted.<br> "
                    },
                    new Email()
                    {
                        Subject="Reject Provider Request",
                        Name ="Reject_News",
                        Message= "Dear #UserName </br> Your #TitleNews news has been rejected.<br> " +
                        "<strong>#Reason</strong></br>"+
                        "If you have problem, please contact us"
                    },
                    new Email()
                    {
                        Subject="You had sign in BPT Service",
                        Name ="Social_Login",
                        Message= "Dear #UserName </br> Your new password is #Password " +
                        "<strong>You can access to your personal page for change password or using login by social login</strong></br>"+
                        "If you have problem, please contact us"
                    },
                };
                await _context.Emails.AddRangeAsync(emails);
                await _context.SaveChangesAsync();
            }
        }
    }
}