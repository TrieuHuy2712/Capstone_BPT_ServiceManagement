using BPT_Service.Model.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BPT_Service.Application.PermissionService.Query.CheckUserIsAdmin
{
    public class CheckUserIsAdminQuery : ICheckUserIsAdminQuery
    {
        private readonly UserManager<AppUser> _userRepository;
        private string Admin = "Admin";

        public CheckUserIsAdminQuery(UserManager<AppUser> userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task<bool> ExecuteAsync(string userId)
        {
            try
            {
                var getUser = await _userRepository.FindByIdAsync(userId);
                var getUserInRole = await _userRepository.GetRolesAsync(getUser);
                foreach (var item in getUserInRole)
                {
                    if (item == Admin)
                    {
                        return true;
                    }
                }
                return false;

            }
            catch (Exception)
            {

                return false;
            }
        }
    }
}
