using BPT_Service.Application.FollowingProviderService.ViewModel;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Entities.ServiceModel.ProviderServiceModel;
using BPT_Service.Model.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPT_Service.Application.FollowingProviderService.Command.UnFollowProviderService
{
    public class UnFollowProviderServiceCommand : IUnFollowProviderServiceCommand
    {
        private readonly IRepository<ProviderFollowing, int> _providerFollowingRepository;
        public async Task<CommandResult<FollowingProviderServiceViewModel>> ExecuteAsync(FollowingProviderServiceViewModel vm)
        {
            try
            {
                var checkUserHasBeenFollow = await _providerFollowingRepository.FindAllAsync(x =>
                    x.ProviderId == Guid.Parse(vm.ProviderId) &&
                    x.UserId == Guid.Parse(vm.UserId));

                if (checkUserHasBeenFollow.Count() == 0)
                {
                    return new CommandResult<FollowingProviderServiceViewModel>
                    {
                        isValid = false,
                        errorMessage = "You has not register this service"
                    };
                }
                 _providerFollowingRepository.RemoveMultiple(checkUserHasBeenFollow.ToList());
                await _providerFollowingRepository.SaveAsync();
                return new CommandResult<FollowingProviderServiceViewModel>
                {
                    isValid = true,
                    errorMessage = "You had delete"
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
