using BPT_Service.Application.FollowingPostService.ViewModel;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Entities.ServiceModel;
using BPT_Service.Model.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BPT_Service.Application.FollowingPostService.Command.UnFollowPostService
{
    public class UnFollowPostServiceCommand : IUnFollowPostServiceCommand
    {
        private readonly IRepository<ServiceFollowing, int> _serviceFollowingRepository;
        public UnFollowPostServiceCommand(IRepository<ServiceFollowing, int> serviceFollowingRepository)
        {
            _serviceFollowingRepository = serviceFollowingRepository;
        }

        public async Task<CommandResult<ServiceFollowingViewModel>> ExecuteAsync(ServiceFollowingViewModel model)
        {
            try
            {
                var checkUserHasFollow = await _serviceFollowingRepository.FindByIdAsync(model.Id);

                if (checkUserHasFollow == null)
                {
                    return new CommandResult<ServiceFollowingViewModel>
                    {
                        isValid = false,
                        errorMessage = "Cannot find this service"
                    };
                }
                 _serviceFollowingRepository.Remove(checkUserHasFollow);
                await _serviceFollowingRepository.SaveAsync();
                return new CommandResult<ServiceFollowingViewModel>
                {
                    isValid = true,
                    myModel = model
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
    }
}
