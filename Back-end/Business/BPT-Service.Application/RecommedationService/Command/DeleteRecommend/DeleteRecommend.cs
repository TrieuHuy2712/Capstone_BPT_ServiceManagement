using BPT_Service.Application.RecommedationService.ViewModel;
using BPT_Service.Common.Helpers;
using BPT_Service.Common.Logging;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Threading.Tasks;

namespace BPT_Service.Application.RecommedationService.Command.RecommendLocation.DeleteRecommendLocation
{
    public class DeleteRecommend : IDeleteRecommend
    {
        private readonly IRepository<Recommendation, int> _recommendRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<AppUser> _userManager;

        public DeleteRecommend(
            IRepository<Recommendation, int> recommendRepository,
            IHttpContextAccessor httpContextAccessor,
            UserManager<AppUser> userManager)
        {
            _recommendRepository = recommendRepository;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }

        public async Task<CommandResult<LocationRecommendationViewModel>> ExecuteAsync(int id)
        {
            var userId = _httpContextAccessor.HttpContext.User.Identity.Name;
            var userName = _userManager.FindByIdAsync(userId).Result.UserName;
            try
            {
                var findId = await _recommendRepository.FindByIdAsync(id);
                if (findId == null)
                {
                    await Logging<DeleteRecommend>.
                        WarningAsync(ActionCommand.COMMAND_ADD, userName, "Cannot find this id");
                    return new CommandResult<LocationRecommendationViewModel>
                    {
                        isValid = false,
                        errorMessage = "Cannot find this id"
                    };
                }
                _recommendRepository.Remove(findId);
                await _recommendRepository.SaveAsync();
                await Logging<LocationRecommendationViewModel>.InformationAsync(ActionCommand.COMMAND_DELETE, userName, "This recommend has delete");
                return new CommandResult<LocationRecommendationViewModel>
                {
                    isValid = true,
                    errorMessage = "This recommend has deleted successfully"
                };
            }
            catch (Exception ex)
            {
                await Logging<LocationRecommendationViewModel>.ErrorAsync(ex, ActionCommand.COMMAND_DELETE, userName, "Has error");
                return new CommandResult<LocationRecommendationViewModel>
                {
                    errorMessage = ex.Message.ToString(),
                    isValid = false
                };
            }
        }
    }
}