using BPT_Service.Application.FollowingProviderService.ViewModel;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Entities.ServiceModel.ProviderServiceModel;
using BPT_Service.Model.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPT_Service.Application.FollowingProviderService.Command.FollowProviderService
{
    public class FollowProviderServiceCommand : IFollowProviderServiceCommand
    {
        private readonly IRepository<ProviderFollowing, int> _providerFollowingRepository;
        public FollowProviderServiceCommand(IRepository<ProviderFollowing, int> providerFollowingRepository)
        {
            _providerFollowingRepository = providerFollowingRepository;
        }

        public async Task<CommandResult<FollowingProviderServiceViewModel>> ExecuteAsync(FollowingProviderServiceViewModel vm)
        {
            try
            {
                var checkUserHasFollowProvider = await _providerFollowingRepository.FindAllAsync(x =>
                    x.ProviderId == Guid.Parse(vm.ProviderId) && x.UserId == Guid.Parse(vm.UserId));
                if (checkUserHasFollowProvider.Count() != 0)
                {
                    return new CommandResult<FollowingProviderServiceViewModel>
                    {
                        isValid = false,
                        errorMessage = "You had been follow this provider"
                    };
                }
                var data = MappingData(vm);
                await _providerFollowingRepository.Add(data);
                await _providerFollowingRepository.SaveAsync();
                return new CommandResult<FollowingProviderServiceViewModel>
                {
                    isValid = true,
                    myModel = vm
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
        private ProviderFollowing MappingData(FollowingProviderServiceViewModel vm)
        {
            ProviderFollowing providerFollowing = new ProviderFollowing();
            providerFollowing.UserId = Guid.Parse(vm.UserId);
            providerFollowing.ProviderId = Guid.Parse(vm.ProviderId);
            providerFollowing.IsReceiveEmail = vm.IsReceiveEmail;
            providerFollowing.DateCreated = DateTime.Now;
            return providerFollowing;
        }
    }
}
