using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BPT_Service.Application.Interfaces;
using BPT_Service.Application.ViewModels.System;
using BPT_Service.Common.Dtos;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BPT_Service.Application.Implementation
{
    public class UserService : IUserService
    {
        private readonly UserManager<AppUser> _userManager;


        private readonly RoleManager<AppRole> _roleManager;
        public UserService(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<bool> AddAsync(AppUserViewModel userVm)
        {
            var user = new AppUser()
            {
                UserName = userVm.UserName,
                Avatar = userVm.Avatar,
                Email = userVm.Email,
                FullName = userVm.FullName,
                DateCreated = DateTime.Now,
                PhoneNumber = userVm.PhoneNumber
            };
            var result = await _userManager.CreateAsync(user, userVm.Password);
            if (result.Succeeded && userVm.Roles.Count > 0)
            {
                var appUser = await _userManager.FindByNameAsync(user.UserName);
                if (appUser != null)
                    await _userManager.AddToRolesAsync(appUser, userVm.Roles);

            }
            return true;
        }

        public async Task<bool> AddExternalAsync(SocialUserViewModel socialUserViewModel)
        {

            var getExistEmail = await _userManager.Users.Where(x => x.Email == socialUserViewModel.email).FirstOrDefaultAsync();
            if (getExistEmail != null)
            {
                var user = new AppUser
                {
                    Email = socialUserViewModel.email,
                    Avatar = socialUserViewModel.image,
                    UserName = socialUserViewModel.name,
                    DateCreated = DateTime.Now
                };
                var newPassword = RandomString(12);
                var result = await _userManager.CreateAsync(user, newPassword);
                if (result.Succeeded)
                {
                    var appUser = await _userManager.FindByNameAsync(user.UserName);
                    if (appUser != null)
                        await _userManager.AddToRoleAsync(appUser, "Customer");

                }
                return true;
            }
            return true;

        }
        public async Task<bool> DeleteAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                await _userManager.DeleteAsync(user);
                return true;
            }
            return false;

        }

        public async Task<List<AppUserViewModel>> GetAllAsync()
        {
            //return await _userManager.Users.ProjectTo<AppUserViewModel>(_config).ToListAsync();
            return await _userManager.Users.Select(x => new AppUserViewModel()
            {
                Avatar = x.Avatar,
                DateCreated = x.DateCreated,
                Email = x.Email,
                FullName = x.FullName,
                PhoneNumber = x.PhoneNumber,
                Token = x.Token,
                UserName = x.UserName,
            }).ToListAsync();
        }

        public PagedResult<AppUserViewModel> GetAllPagingAsync(string keyword, int page, int pageSize)
        {
            var query = _userManager.Users;
            if (!string.IsNullOrEmpty(keyword))
                query = query.Where(x => x.FullName.Contains(keyword)
                || x.UserName.Contains(keyword)
                || x.Email.Contains(keyword));

            int totalRow = query.Count();
            query = query.Skip((page - 1) * pageSize)
               .Take(pageSize);

            var data = query.Select(x => new AppUserViewModel()
            {
                UserName = x.UserName,
                Avatar = x.Avatar,
                Email = x.Email,
                FullName = x.FullName,
                Id = x.Id,
                PhoneNumber = x.PhoneNumber,
                Status = x.Status,
                DateCreated = x.DateCreated

            }).ToList();
            var paginationSet = new PagedResult<AppUserViewModel>()
            {
                Results = data,
                CurrentPage = page,
                RowCount = totalRow,
                PageSize = pageSize
            };

            return paginationSet;
        }

        public async Task<AppUserViewModel> GetById(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            var roles = await _userManager.GetRolesAsync(user);
            //var userVm = _mapper.Map<AppUser, AppUserViewModel>(user);
            AppUserViewModel userVm = new AppUserViewModel();
            userVm.Id = user.Id;
            userVm.Avatar = user.Avatar;
            userVm.DateCreated = user.DateCreated;
            userVm.Email = user.Email;
            userVm.FullName = user.FullName;
            userVm.UserName = user.UserName;
            userVm.Token = user.Token;
            userVm.Roles = roles.ToList();
            return userVm;
        }

        public async Task<bool> CreateCustomerAsync(AppUserViewModel userVm, string password)
        {

            //Check exist username
            var user = await _userManager.FindByNameAsync(userVm.UserName);
            if (user != null)
            {
                return false;
            }

            //Check exist email
            var email = await _userManager.FindByEmailAsync(userVm.Email);
            if (email != null)
            {
                return false;
            }

            if (user == null && email == null)
            {
                await _userManager.CreateAsync(new AppUser()
                {
                    UserName = userVm.UserName,
                    FullName = userVm.FullName,
                    Email = userVm.Email,
                    DateCreated = DateTime.Now,
                    DateModified = DateTime.Now,
                    Status = Status.Active
                }, password);
                var newUser = await _userManager.FindByNameAsync(userVm.UserName);
                await _userManager.AddToRoleAsync(user, "Customer");
            }
            return true;
        }

        public async Task<bool> UpdateAsync(AppUserViewModel userVm)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userVm.Id.ToString());
                user.FullName = userVm.FullName;
                user.Status = userVm.Status;
                user.Email = userVm.Email;
                user.PhoneNumber = userVm.PhoneNumber;
                var userRoles = _userManager.GetRolesAsync(user);

                var selectedRole = userVm.NewRoles.ToArray();
                selectedRole = selectedRole ?? new string[] { };

                await _userManager.AddToRolesAsync(user, selectedRole.ToArray());
                var userRoles1 = await _userManager.GetRolesAsync(user);
                await _userManager.UpdateAsync(user);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }
        private static Random random = new Random();
        private static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

    }
}