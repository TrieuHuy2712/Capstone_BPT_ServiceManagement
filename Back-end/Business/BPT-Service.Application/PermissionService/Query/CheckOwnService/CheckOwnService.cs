using BPT_Service.Model.Infrastructure.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BPT_Service.Application.PermissionService.Query.CheckOwnService
{
    public class CheckOwnService : ICheckOwnService
    {
        private readonly IRepository<Model.Entities.ServiceModel.UserServiceModel.UserService, int> _userServiceRepository;
        private readonly IRepository<Model.Entities.ServiceModel.ProviderServiceModel.ProviderService, int> _providerServiceRepository;

        public CheckOwnService(IRepository<Model.Entities.ServiceModel.UserServiceModel.UserService, int> userServiceRepository,
            IRepository<Model.Entities.ServiceModel.ProviderServiceModel.ProviderService, int> providerServiceRepository)
        {
            _userServiceRepository = userServiceRepository;
            _providerServiceRepository = providerServiceRepository;
        }

        public async Task<bool> ExecuteAsync(string stringUserId, string serviceId)
        {
            var isOwner = false;
            var getFollowUserId = await _userServiceRepository.FindAllAsync(x => x.UserId == Guid.Parse(stringUserId) && x.ServiceId == Guid.Parse(serviceId));
            if (getFollowUserId.Count() == 0)
            {
                return isOwner;
            }
            var getFollowProvider = await _providerServiceRepository.FindAllAsync(x => x.ProviderId == Guid.Parse(stringUserId) && x.ServiceId == Guid.Parse(serviceId));
            if (getFollowUserId.Count() == 0)
            {
                return isOwner;
            }
            return true;
        }
    }
}