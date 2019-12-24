using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Enums;
using Microsoft.AspNetCore.Identity;

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
                    NameVietNamese = "Nguoi quan ly"

                });
                await _roleManager.CreateAsync(new AppRole()
                {
                    Name = "Staff",
                    NormalizedName = "Staff",
                    Description = "Staff",
                    NameVietNamese = "Nhan vien"
                });
                await _roleManager.CreateAsync(new AppRole()
                {
                    Name = "Customer",
                    NormalizedName = "Customer",
                    Description = "Customer",
                    NameVietNamese = "Khach hang"
                });
            }
            if (!_userManager.Users.Any())
            {
                await _userManager.CreateAsync(new AppUser()
                {
                    UserName = "admin",
                    FullName = "Administrator",
                    Email = "admin@gmail.com",
                    DateCreated = DateTime.Now,
                    DateModified = DateTime.Now,
                    Status = Status.Active
                }, "123654$");
                var user = await _userManager.FindByNameAsync("admin");
                await _userManager.AddToRoleAsync(user, "Admin");
            }
            if (_context.Categories.Count() == 0)
            {
                List<Category> categories = new List<Category>() {
                    new Category()
                    {
                        CategoryName="Cleaner",
                        Description="This is cleaner",
                        NameVietnamese="Dich vu don nha"
                    },
                    new Category()
                    {
                        CategoryName="Cleaner 1",
                        Description="This is cleaner 1",
                        NameVietnamese="Dich vu don nha 1"
                    }

                };
                await _context.Categories.AddRangeAsync(categories);
                await _context.SaveChangesAsync();
            }
            if (_context.Functions.Count() == 0)
            {
                List<Function> functions = new List<Function>()
                {
                    new Function() {Id = "SYSTEM", Name = "System",ParentId = null,SortOrder = 1,Status = Status.Active,URL = "/",IconCss = "fa-desktop",NameVietNamese="Cấu hình "  },
                    new Function() {Id = "ROLE", Name = "Role",ParentId = "SYSTEM",SortOrder = 1,Status = Status.Active,URL = "/main/role/index",IconCss = "fa-home",NameVietNamese="Vai trò"  },
                    new Function() {Id = "FUNCTION", Name = "Function",ParentId = "SYSTEM",SortOrder = 2,Status = Status.Active,URL = "/main/function/index",IconCss = "fa-home",NameVietNamese="Chức năng"  },
                    new Function() {Id = "USER", Name = "User",ParentId = "SYSTEM",SortOrder =3,Status = Status.Active,URL = "/main/user/index",IconCss = "fa-home",NameVietNamese="Người dùng"  },
                    new Function() {Id = "ACTIVITY", Name = "Activity",ParentId = "SYSTEM",SortOrder = 4,Status = Status.Active,URL = "/main/activity/index",IconCss = "fa-home",NameVietNamese="Hoạt động"  },
                    new Function() {Id = "ERROR", Name = "Error",ParentId = "SYSTEM",SortOrder = 5,Status = Status.Active,URL = "/main/error/index",IconCss = "fa-home",NameVietNamese="Lỗi"  },
                    new Function() {Id = "SETTING", Name = "Configuration",ParentId = "SYSTEM",SortOrder = 6,Status = Status.Active,URL = "/main/setting/index",IconCss = "fa-home",NameVietNamese="Cài đặt"  },

                    new Function() {Id = "PRODUCT",Name = "Product Management",ParentId = null,SortOrder = 2,Status = Status.Active,URL = "/",IconCss = "fa-chevron-down",NameVietNamese="Sản phẩm"  },
                    new Function() {Id = "PRODUCT_CATEGORY",Name = "Category",ParentId = "PRODUCT",SortOrder =1,Status = Status.Active,URL = "/main/productcategory/index",IconCss = "fa-chevron-down",NameVietNamese="Loại sản phẩm"  },
                    new Function() {Id = "PRODUCT_LIST",Name = "Product",ParentId = "PRODUCT",SortOrder = 2,Status = Status.Active,URL = "/main/product/index",IconCss = "fa-chevron-down",NameVietNamese="Thông tin sản phẩm"  },
                    new Function() {Id = "BILL",Name = "Bill",ParentId = "PRODUCT",SortOrder = 3,Status = Status.Active,URL = "/main/bill/index",IconCss = "fa-chevron-down",NameVietNamese="Hóa đơn"  },

                    new Function() {Id = "CONTENT",Name = "Content",ParentId = null,SortOrder = 3,Status = Status.Active,URL = "/",IconCss = "fa-table",NameVietNamese="Nội dung"  },
                    new Function() {Id = "BLOG",Name = "Blog",ParentId = "CONTENT",SortOrder = 1,Status = Status.Active,URL = "/main/blog/index",IconCss = "fa-table",NameVietNamese="Blog"  },
                    new Function() {Id = "PAGE",Name = "Page",ParentId = "CONTENT",SortOrder = 2,Status = Status.Active,URL = "/main/page/index",IconCss = "fa-table",NameVietNamese="Trang"  },

                    new Function() {Id = "UTILITY",Name = "Utilities",ParentId = null,SortOrder = 4,Status = Status.Active,URL = "/",IconCss = "fa-clone",NameVietNamese="Tiện ích"  },
                    new Function() {Id = "FOOTER",Name = "Footer",ParentId = "UTILITY",SortOrder = 1,Status = Status.Active,URL = "/main/footer/index",IconCss = "fa-clone",NameVietNamese="Chân trang"  },
                    new Function() {Id = "FEEDBACK",Name = "Feedback",ParentId = "UTILITY",SortOrder = 2,Status = Status.Active,URL = "/main/feedback/index",IconCss = "fa-clone",NameVietNamese="Phản hồi"  },
                    new Function() {Id = "CONTACT",Name = "Contact",ParentId = "UTILITY",SortOrder = 4,Status = Status.Active,URL = "/main/contact/index",IconCss = "fa-clone",NameVietNamese="Liên hệ"  },
                    new Function() {Id = "SLIDE",Name = "Slide",ParentId = "UTILITY",SortOrder = 5,Status = Status.Active,URL = "/main/slide/index",IconCss = "fa-clone",NameVietNamese="Trang chiếu"  },
                    new Function() {Id = "ADVERTISMENT",Name = "Advertisment",ParentId = "UTILITY",SortOrder = 6,Status = Status.Active,URL = "/main/advertistment/index",IconCss = "fa-clone",NameVietNamese="Quảng cáo"  },

                    new Function() {Id = "REPORT",Name = "Report",ParentId = null,SortOrder = 5,Status = Status.Active,URL = "/",IconCss = "fa-bar-chart-o",NameVietNamese="Báo cáo"  },
                    new Function() {Id = "REVENUES",Name = "Revenue report",ParentId = "REPORT",SortOrder = 1,Status = Status.Active,URL = "/main/report/revenues",IconCss = "fa-bar-chart-o", NameVietNamese="Lợi nhuận" },
                    new Function() {Id = "ACCESS",Name = "Visitor Report",ParentId = "REPORT",SortOrder = 2,Status = Status.Active,URL = "/main/report/visitor",IconCss = "fa-bar-chart-o",NameVietNamese="Số người ghé thăm"  },
                    new Function() {Id = "READER",Name = "Reader Report",ParentId = "REPORT",SortOrder = 3,Status = Status.Active,URL = "/main/report/reader",IconCss = "fa-bar-chart-o" ,NameVietNamese="Số người đọc" },
                };
                await _context.Functions.AddRangeAsync(functions);
                await _context.SaveChangesAsync();
            }


        }
    }
}