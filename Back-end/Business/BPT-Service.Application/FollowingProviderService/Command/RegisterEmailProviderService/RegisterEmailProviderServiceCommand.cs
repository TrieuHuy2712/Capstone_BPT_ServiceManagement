using BPT_Service.Application.FollowingProviderService.ViewModel;
using BPT_Service.Common;
using BPT_Service.Common.Helpers;
using BPT_Service.Common.Logging;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Entities.ServiceModel;
using BPT_Service.Model.Entities.ServiceModel.ProviderServiceModel;
using BPT_Service.Model.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BPT_Service.Application.FollowingProviderService.Command.RegisterEmailProviderService
{
    public class RegisterEmailProviderServiceCommand : IRegisterEmailProviderServiceCommand
    {
        private readonly IRepository<ProviderFollowing, int> _providerFollowingRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IRepository<Provider, Guid> _providerRepository;

        public RegisterEmailProviderServiceCommand(
            IRepository<ProviderFollowing, int> providerFollowingRepository, 
            IHttpContextAccessor httpContextAccessor, 
            IRepository<Provider, Guid> providerRepository)
        {
            _providerFollowingRepository = providerFollowingRepository;
            _httpContextAccessor = httpContextAccessor;
            _providerRepository = providerRepository;
        }

        public async Task<CommandResult<FollowingProviderServiceViewModel>> ExecuteAsync(int idRegister)
        {
            var userName = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            try
            {
                var findIdRegister = await _providerFollowingRepository.FindByIdAsync(idRegister);
                if (findIdRegister == null)
                {
                    await Logging<RegisterEmailProviderServiceCommand>
                        .ErrorAsync(ActionCommand.COMMAND_UPDATE, userName, "Cannot find your following");
                    return new CommandResult<FollowingProviderServiceViewModel>
                    {
                        isValid = false,
                        errorMessage = "Cannot find your following"
                    };
                }
                var getProvider = await _providerRepository.FindByIdAsync(findIdRegister.ProviderId);
                if (getProvider == null)
                {
                    await Logging<RegisterEmailProviderServiceCommand>.
                       ErrorAsync(ActionCommand.COMMAND_ADD, userName, ErrorMessageConstant.ERROR_CANNOT_FIND_ID);
                    return new CommandResult<FollowingProviderServiceViewModel>
                    {
                        isValid = false,
                        errorMessage = ErrorMessageConstant.ERROR_CANNOT_FIND_ID
                    };
                }
                if (findIdRegister.IsReceiveEmail)
                {
                    findIdRegister.IsReceiveEmail = !findIdRegister.IsReceiveEmail;
                }
                else
                {
                    findIdRegister.IsReceiveEmail = !findIdRegister.IsReceiveEmail;
                }
                _providerFollowingRepository.Update(findIdRegister);
                await _providerFollowingRepository.SaveAsync();
                await LoggingUser<RegisterEmailProviderServiceCommand>.
                   InformationAsync(getProvider.UserId.ToString(), userName, userName + " had follow your provider");
                await Logging<RegisterEmailProviderServiceCommand>.InformationAsync(ActionCommand.COMMAND_UPDATE, userName, JsonConvert.SerializeObject(findIdRegister));
                return new CommandResult<FollowingProviderServiceViewModel>
                {
                    isValid = true,
                    myModel = new FollowingProviderServiceViewModel
                    {
                        UserId = findIdRegister.UserId.ToString(),
                        DateCreated = DateTime.Now,
                        IsReceiveEmail = findIdRegister.IsReceiveEmail,
                        ProviderId = findIdRegister.ProviderId.ToString()
                    }
                };
            }
            catch (Exception ex)
            {
                await Logging<RegisterEmailProviderServiceCommand>.ErrorAsync(ex, ActionCommand.COMMAND_UPDATE, userName, "Has error");
                return new CommandResult<FollowingProviderServiceViewModel>
                {
                    isValid = false,
                    errorMessage = ex.InnerException.Message.ToString()
                };
            }
        }
    }
}