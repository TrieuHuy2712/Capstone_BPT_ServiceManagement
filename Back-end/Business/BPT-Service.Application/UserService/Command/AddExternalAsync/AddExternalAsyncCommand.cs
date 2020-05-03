using BPT_Service.Application.EmailService.Query.GetAllEmailService;
using BPT_Service.Application.UserService.ViewModel;
using BPT_Service.Common.Constants;
using BPT_Service.Common.Constants.EmailConstant;
using BPT_Service.Common.Dtos;
using BPT_Service.Common.Helpers;
using BPT_Service.Common.Support;
using BPT_Service.Model.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BPT_Service.Application.UserService.Command.AddExternalAsync
{
    public class AddExternalAsyncCommand : IAddExternalAsyncCommand
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RandomSupport _randomSupport;
        private readonly RemoveSupport _removeSupport;
        private readonly IGetAllEmailServiceQuery _getAllEmailServiceQuery;
        private readonly IOptions<EmailConfigModel> _configEmail;

        public AddExternalAsyncCommand(
            UserManager<AppUser> userManager, 
            RandomSupport randomSupport, 
            RemoveSupport removeSupport,
            IGetAllEmailServiceQuery getAllEmailServiceQuery,
            IOptions<EmailConfigModel> configEmail)
        {
            _userManager = userManager;
            _randomSupport = randomSupport;
            _removeSupport = removeSupport;
            _getAllEmailServiceQuery = getAllEmailServiceQuery;
            _configEmail = configEmail;
        }

        public async Task<CommandResult<AppUserViewModelinUserService>> ExecuteAsync(AppUserViewModelinUserService socialUserViewModel)
        {
            try
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
                            await _userManager.AddToRoleAsync(appUser, ConstantRoles.Customer);
                        //Set content for email
                        var getAllEmail = await _getAllEmailServiceQuery.ExecuteAsync();
                        var getFirstEmail = getAllEmail.Where(x => x.Name == EmailName.Reject_News).FirstOrDefault();
                        getFirstEmail.Message = getFirstEmail.Message.Replace(EmailKey.UserNameKey, user.UserName).
                            Replace(EmailKey.PasswordKey, newPassword);

                        ContentEmail(_configEmail.Value.SendGridKey, getFirstEmail.Subject,
                                    getFirstEmail.Message, socialUserViewModel.Email).Wait();
                    }
                    var getNewEmail = await _userManager.Users.Where(x => x.Email == socialUserViewModel.Email).FirstOrDefaultAsync();
                    var getToken = SetToken(getNewEmail);
                    return new CommandResult<AppUserViewModelinUserService>
                    {
                        isValid = true,
                        myModel = new AppUserViewModelinUserService
                        {
                            Email = getNewEmail.Email,
                            Avatar = getNewEmail.Avatar,
                            UserName = getNewEmail.UserName,
                            FullName = getNewEmail.FullName,
                            Token = getToken.Token
                        }
                    };
                }
                var getExistToken = SetToken(getExistEmail);
                return new CommandResult<AppUserViewModelinUserService>
                {
                    isValid = true,
                    myModel = new AppUserViewModelinUserService
                    {
                        Email = getExistEmail.Email,
                        Avatar = getExistEmail.Avatar,
                        UserName = getExistEmail.UserName,
                        FullName = getExistEmail.FullName,
                        Token = getExistToken.Token
                    }
                };
            }
            catch (System.Exception ex)
            {
                return new CommandResult<AppUserViewModelinUserService>
                {
                    isValid = false,
                    errorMessage = ex.InnerException.ToString()
                };
            }
        }

        private async Task ContentEmail(string apiKey, string subject1, string message, string email)
        {
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress(_configEmail.Value.FromUserEmail, _configEmail.Value.FullUserName);
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