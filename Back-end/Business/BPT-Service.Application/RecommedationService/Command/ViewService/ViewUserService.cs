using BPT_Service.Common.Helpers;
using BPT_Service.Common.Logging;
using BPT_Service.Model;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BPT_Service.Application.RecommedationService.Command.ViewService
{
    public class ViewUserService : IViewUserService
    {
        private readonly IHttpContextAccessor _httpConextAccessor;
        private readonly IRepository<UserRecommendation, int> _userRecommendation;

        public ViewUserService(IHttpContextAccessor httpConextAccessor, IRepository<UserRecommendation, int> userRecommendation)
        {
            _httpConextAccessor = httpConextAccessor;
            _userRecommendation = userRecommendation;
        }

        public async Task<CommandResult<UserRecommendation>> ExecuteAsync(string idService)
        {
            try
            {
                var current = DateTime.Now;
                var userId = _httpConextAccessor.HttpContext.User.Identity.Name;
                if (string.IsNullOrEmpty(userId))
                {
                    return new CommandResult<UserRecommendation> {
                        isValid = false,
                        errorMessage="Cannot find this user"
                    };
                }
                var findLatestTime = await _userRecommendation.
                    FindAllAsync(x => x.UserId == Guid.Parse(userId) && x.ServiceId == Guid.Parse(idService));

                var getNearestTime = findLatestTime.OrderByDescending(x => x.DateCreated).FirstOrDefault().DateCreated;
                var subtractLatestDate = current.Subtract(getNearestTime);
                var subtractDefaultDate = current.Subtract(current.AddMinutes(-15));
                if(subtractLatestDate < subtractDefaultDate)
                {
                    return new CommandResult<UserRecommendation>
                    {
                        isValid = false,
                    };
                }
                var addInformation = new UserRecommendation()
                {
                    DateCreated = DateTime.Now,
                    ServiceId = Guid.Parse(idService),
                    UserId = Guid.Parse(userId)
                };
                await _userRecommendation.Add(addInformation);
                await _userRecommendation.SaveAsync();
                await Logging<ViewUserService>.InformationAsync(ActionCommand.COMMAND_ADD, userId, JsonConvert.SerializeObject(addInformation));
                return new CommandResult<UserRecommendation>
                {
                    isValid = true,
                    myModel = addInformation
                };

            }
            catch (System.Exception ex)
            {
                return new CommandResult<UserRecommendation>
                {
                    isValid = false,
                    errorMessage = ex.Message.ToString()
                };
            }
        }   
    }
}