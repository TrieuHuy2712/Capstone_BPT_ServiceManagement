using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BPT_Service.Common.Helpers;
using BPT_Service.Model.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Threading.Tasks;
using System.Linq;
using AutoMapper;
using BPT_Service.Application.ViewModels.System;
using BPT_Service.Application.Interfaces;

namespace BPT_Service.Application.Implementation
{
    public class AuthenticateService : IAuthenticateService
    {
        private readonly AppSettings _appSettings;
        private UserManager<AppUser> _userManager;

        public AuthenticateService(IOptions<AppSettings> appSettings, UserManager<AppUser> userManager)
        {
            _appSettings = appSettings.Value;
            _userManager = userManager;
        }
        public async Task<AppUser> Authenticate(string username, string password)
        {
            var user = await _userManager.FindByNameAsync(username);

            // return null if user not found
            if (user == null)
                return null;

            if (user != null && await _userManager.CheckPasswordAsync(user, password))
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes("qwertyuioplkjhgfdsazxcvbnmqwertlkjfdslkjflksjfklsjfklsjdflskjflyuioplkjhgfdsazxcvbnmmnbv");

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString()),
                }),
                    Expires = DateTime.UtcNow.AddDays(7),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                user.Token = tokenHandler.WriteToken(token);
                return user;
            }
            else
            {
                return null;
            }
        }

        public async Task<IEnumerable<AppUserViewModel>> GetAll()
        {
            var model = _userManager.Users.ToList();
            IEnumerable<AppUserViewModel> modelVm = model.Select(x => new AppUserViewModel
            {
                UserName = x.UserName,
                Avatar = x.Avatar,
            });
            return modelVm;

        }

        public async Task<AppUserViewModel> GetById(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return null;
            }
            else
            {
                AppUserViewModel modelVm = new AppUserViewModel();
                modelVm.UserName = user.UserName;
                modelVm.Avatar = user.Avatar;
                return modelVm;
            }
        }
    }
}