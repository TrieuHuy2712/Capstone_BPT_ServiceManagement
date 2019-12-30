using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using BPT_Service.Common.Helpers;
using System.Threading.Tasks;
using System.Linq;
using BPT_Service.Application.ViewModels.System;
using BPT_Service.Application.Interfaces;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using BPT_Service.Model.Entities;
using Microsoft.EntityFrameworkCore;

namespace BPT_Service.Application.Implementation
{
    public class AuthenticateService : IAuthenticateService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public AuthenticateService(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public async Task<AppUser> Authenticate(string username, string password)
        {
            var user = await _userManager.FindByNameAsync(username);

            // return null if user not found
            if (user == null)
            {
                return null;
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, password, true);
            if (user != null && result.Succeeded)
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(KeySetting.JWTSetting);

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
            else if (result.IsLockedOut)
            {
                user.Token = KeySetting.TOKENLOCKEDOUT;
                return user;
            }
            else
            {
                return null;
            }
        }

        public async Task<bool> ResetPasswordAsync(string username, string oldPassword, string newPassword)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user != null && await _userManager.CheckPasswordAsync(user, oldPassword))
            {
                string resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
                IdentityResult passwordChangeResult = await _userManager.ResetPasswordAsync(user, resetToken, newPassword);
                if (passwordChangeResult.Succeeded)
                {
                    return true;
                }
            }
            return false;
        }

        public async Task<IEnumerable<AppUserViewModel>> GetAll()
        {
            var model = await _userManager.Users.ToListAsync();
            IEnumerable<AppUserViewModel> modelVm = model.Select(x => new AppUserViewModel
            {
                Id = x.Id,
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
                modelVm.Id = user.Id;
                modelVm.UserName = user.UserName;
                modelVm.Avatar = user.Avatar;
                return modelVm;
            }
        }
    }
}