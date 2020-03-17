using BPT_Service.Application.FollowingProviderService.ViewModel;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Entities.ServiceModel.ProviderServiceModel;
using BPT_Service.Model.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BPT_Service.Application.FollowingProviderService.Command.RegisterEmailProviderService
{
    public class RegisterEmailProviderServiceCommand : IRegisterEmailProviderServiceCommand
    {
        private readonly IRepository<ProviderFollowing, int> _providerFollowingRepository;
        public RegisterEmailProviderServiceCommand(IRepository<ProviderFollowing, int> providerFollowingRepository)
        {
            _providerFollowingRepository = providerFollowingRepository;
        }

        public async Task<CommandResult<FollowingProviderServiceViewModel>> ExecuteAsync(int idRegister)
        {
            try
            {
                var findIdRegister = await _providerFollowingRepository.FindByIdAsync(idRegister);
                if(findIdRegister == null)
                {
                    return new CommandResult<FollowingProviderServiceViewModel>
                    {
                        isValid = false,
                        errorMessage = "Cannot find your following"
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

                return new CommandResult<FollowingProviderServiceViewModel>
                {
                    isValid = false,
                    errorMessage = ex.InnerException.Message.ToString()
                };
            }
        }
    }
}
