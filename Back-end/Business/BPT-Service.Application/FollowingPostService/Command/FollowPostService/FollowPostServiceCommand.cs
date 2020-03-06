using BPT_Service.Application.FollowingPostService.ViewModel;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Entities.ServiceModel;
using BPT_Service.Model.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BPT_Service.Application.FollowingPostService.Command.FollowPostService
{
    public class FollowPostServiceCommand : IFollowPostServiceCommand
    {
        private readonly IRepository<ServiceFollowing, int> _serviceFollowingRepository;
        public FollowPostServiceCommand(IRepository<ServiceFollowing, int> serviceFollowingRepository)
        {
            _serviceFollowingRepository = serviceFollowingRepository;
        }

        public async Task<CommandResult<ServiceFollowingViewModel>> ExecuteAsync(ServiceFollowingViewModel serviceFollowingViewModel)
        {
            try
            {
                var checkUserHasFollow = await _serviceFollowingRepository.FindSingleAsync(x =>
                    x.ServiceId == Guid.Parse(serviceFollowingViewModel.ServiceId) &&
                    x.UserId == Guid.Parse(serviceFollowingViewModel.UserId));

                if(checkUserHasFollow== null)
                {
                    return new CommandResult<ServiceFollowingViewModel>
                    {
                        isValid = false,
                        errorMessage = "You had follow this service"
                    };
                }
                var data = MappingData(serviceFollowingViewModel);
                await _serviceFollowingRepository.Add(data);
                await _serviceFollowingRepository.SaveAsync();
                return new CommandResult<ServiceFollowingViewModel>
                {
                    isValid = true,
                    myModel = serviceFollowingViewModel
                };
                
            }
            catch (Exception ex)
            {

                return new CommandResult<ServiceFollowingViewModel>
                {
                    isValid = false,
                    errorMessage = ex.InnerException.Message.ToString()
                };
            }
        }
        private ServiceFollowing MappingData(ServiceFollowingViewModel vm)
        {
            ServiceFollowing serviceFollowing = new ServiceFollowing();
            serviceFollowing.UserId = Guid.Parse(vm.UserId);
            serviceFollowing.ServiceId = Guid.Parse(vm.ServiceId);
            return serviceFollowing;
        }
    }
}
