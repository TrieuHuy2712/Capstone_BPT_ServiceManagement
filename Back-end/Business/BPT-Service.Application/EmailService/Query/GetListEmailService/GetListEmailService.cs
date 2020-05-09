using BPT_Service.Model.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPT_Service.Application.EmailService.Query.GetListEmailService
{
    public class GetListEmailService : IGetListEmailService
    {
        private UserManager<AppUser> _userManager;
        private RoleManager<AppRole> _roleManager;

        public GetListEmailService(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<List<string>> ExecuteAsync()
        {
            List<string> listEmail = new List<string>();
            var listRole = await _roleManager.Roles.Select(x=>x.Name+"@system.com").ToListAsync();
            listEmail.AddRange(listRole);
            var listUser = await _userManager.Users.Select(x => x.Email).ToListAsync();
            listEmail.AddRange(listUser);
            return listEmail;
        }
    }
}
