﻿using BPT_Service.Application.RecommedationService.ViewModel;
using BPT_Service.Common.Helpers;
using BPT_Service.Common.Logging;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Enums;
using BPT_Service.Model.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace BPT_Service.Application.RecommedationService.Command.RecommendLocation.AddRecommendLocation
{
    public class AddRecommendLocation : IAddRecommendLocation
    {
        private readonly IRepository<Recommendation, int> _recommendRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<AppUser> _userManager;

        public AddRecommendLocation(
            IRepository<Recommendation, int> recommendRepository,
            IHttpContextAccessor httpContextAccessor,
            UserManager<AppUser> userManager)
        {
            _recommendRepository = recommendRepository;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }

        public async Task<CommandResult<AddRecommendationViewModel>> ExecuteAsync(AddRecommendationViewModel vm)
        {
            var userId = _httpContextAccessor.HttpContext.User.Identity.Name;
            var userName = _userManager.FindByIdAsync(userId).Result.UserName;
            try
            {
                //Check Order Available
                var findAvailableOrder = await _recommendRepository.FindSingleAsync(x => x.Order == vm.Order && x.Type == TypeRecommendation.Location);
                if (findAvailableOrder != null)
                {
                    _recommendRepository.Remove(findAvailableOrder);
                }

                //Check Location has been set
                var findIdAvaiable = await _recommendRepository.FindSingleAsync(x => x.IdType == vm.IdType.ToString() && x.Type == TypeRecommendation.Location);
                if (findIdAvaiable != null)
                {
                    await Logging<AddRecommendLocation>.WarningAsync(ActionCommand.COMMAND_ADD, userName, "This location has been order at position " + findIdAvaiable.Order);
                    return new CommandResult<AddRecommendationViewModel>
                    {
                        isValid = false,
                        errorMessage = "This location has been order at position " + findIdAvaiable.Order
                    };
                }

                //Add new recommendation
                var addNewOrder = new Recommendation()
                {
                    IdType = vm.IdType.ToString(),
                    Order = vm.Order,
                    Type = TypeRecommendation.Location
                };
                await _recommendRepository.Add(addNewOrder);
                await _recommendRepository.SaveAsync();
                await Logging<AddRecommendLocation>.InformationAsync(ActionCommand.COMMAND_ADD, userName, JsonConvert.SerializeObject(vm));
                return new CommandResult<AddRecommendationViewModel>
                {
                    isValid = true,
                    myModel = vm
                };
            }
            catch (Exception ex)
            {
                await Logging<AddRecommendLocation>.ErrorAsync(ex, ActionCommand.COMMAND_ADD, userName, "Has error");
                return new CommandResult<AddRecommendationViewModel>
                {
                    errorMessage = ex.Message.ToString(),
                    isValid = false
                };
            }
        }
    }
}