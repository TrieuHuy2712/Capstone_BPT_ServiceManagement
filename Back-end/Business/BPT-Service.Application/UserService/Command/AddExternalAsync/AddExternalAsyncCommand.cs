using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BPT_Service.Application.UserService.ViewModel;
using BPT_Service.Common.Helpers;
using BPT_Service.Common.Support;
using BPT_Service.Model.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace BPT_Service.Application.UserService.Command.AddExternalAsync
{
    public class AddExternalAsyncCommand : IAddExternalAsyncCommand
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RandomSupport _randomSupport;
        private readonly RemoveSupport _removeSupport;
        public AddExternalAsyncCommand(UserManager<AppUser> userManager, RandomSupport randomSupport, RemoveSupport removeSupport)
        {
            _userManager = userManager;
            _randomSupport = randomSupport;
            _removeSupport = removeSupport;
        }
        public async Task<AppUserViewModelinUserService> ExecuteAsync(AppUserViewModelinUserService socialUserViewModel)
        {
            var getExistEmail = await _userManager.Users.Where(x => x.Email == socialUserViewModel.Email).FirstOrDefaultAsync();
            if (getExistEmail == null)
            {
                var user = new AppUser
                {
                    Email = socialUserViewModel.Email,
                    Avatar = socialUserViewModel.Avatar,
                    UserName = _removeSupport.RemoveUnicode(socialUserViewModel.UserName).Replace(" ", "_"),
                    FullName = socialUserViewModel.UserName,
                    DateCreated = DateTime.Now
                };
                var newPassword = _randomSupport.RandomString(12);
                var result = await _userManager.CreateAsync(user, newPassword);
                if (result.Succeeded)
                {
                    var appUser = await _userManager.FindByNameAsync(user.UserName);
                    if (appUser != null)
                        await _userManager.AddToRoleAsync(appUser, "Customer");

                    ContentEmail(KeySetting.SENDGRIDKEY, ExternalLoginEmailSetting.Subject,
                                ExternalLoginEmailSetting.Content + newPassword, socialUserViewModel.Email).Wait();
                }
                var getNewEmail = await _userManager.Users.Where(x => x.Email == socialUserViewModel.Email).FirstOrDefaultAsync();
                var getToken = SetToken(getNewEmail);
                return new AppUserViewModelinUserService
                {
                    Email = getNewEmail.Email,
                    Avatar = getNewEmail.Avatar,
                    UserName = getNewEmail.UserName,
                    FullName = getNewEmail.FullName,
                    Token = getToken.Token
                };
            }
            var getExistToken = SetToken(getExistEmail);
            return new AppUserViewModelinUserService
            {
                Email = getExistEmail.Email,
                Avatar = getExistEmail.Avatar,
                UserName = getExistEmail.UserName,
                FullName = getExistEmail.FullName,
                Token = getExistToken.Token
            };
        }
        private async Task ContentEmail(string apiKey, string subject1, string message, string email)
        {
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress(ExternalLoginEmailSetting.FromUserEmail, ExternalLoginEmailSetting.FullNameUser);
            var subject = subject1;
            var to = new EmailAddress(email);
            var plainTextContent = message;
            var htmlContent = "<strong>" + message + "</strong>";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);
        }
         private AppUser SetToken(AppUser appUser)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(KeySetting.JWTSetting);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
            {
                    new Claim(ClaimTypes.Name, appUser.Id.ToString()),
            }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            appUser.Token = tokenHandler.WriteToken(token);
            return appUser;
        }
    }
}