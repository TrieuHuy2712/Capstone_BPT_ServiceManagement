using BPT_Service.Application.AuthenticateService.Query.CheckCanAccessMain;
using BPT_Service.Application.ProviderService.Query.CheckUserIsProvider;
using BPT_Service.Common.Helpers;
using BPT_Service.Model.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BPT_Service.Application.AuthenticateService.Query.AuthenticateofAuthenticationService
{
    public class AuthenticateServiceQuery : IAuthenticateServiceQuery
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ICheckCanAccessMain _checkCanAccessMain;
        private readonly ICheckUserIsProviderQuery _checkUserIsProviderQuery;

        public AuthenticateServiceQuery(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            ICheckCanAccessMain checkCanAccessMain,
            ICheckUserIsProviderQuery checkUserIsProviderQuery)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _checkCanAccessMain = checkCanAccessMain;
            _checkUserIsProviderQuery = checkUserIsProviderQuery;
        }

        public async Task<AppUser> ExecuteAsync(string username, string password)
        {
            var user = await _userManager.FindByNameAsync(username);

            // return null if user not found
            if (user == null)
            {
                var email = await _userManager.FindByEmailAsync(username);
                if (email == null)
                {
                    return null;
                }
                else
                {
                    user = email;
                }
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
                    Expires = DateTime.UtcNow.AddYears(7),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                user.Token = tokenHandler.WriteToken(token);
                if (await _checkCanAccessMain.ExecuteAsync(user.UserName) == true)
                {
                    user.Status = Model.Enums.Status.Active;
                }
                else
                {
                    user.Status = Model.Enums.Status.InActive;
                }
                var checkIsProvider = await _checkUserIsProviderQuery.ExecuteAsync(user.Id.ToString());
                if (checkIsProvider.isValid == true)
                {
                    user.IsProvider = true;
                }
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
    }
}