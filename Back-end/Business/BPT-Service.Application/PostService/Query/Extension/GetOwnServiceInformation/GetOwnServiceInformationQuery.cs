using BPT_Service.Model.Entities.ServiceModel;
using BPT_Service.Model.Infrastructure.Interfaces;
using System;
using System.Threading.Tasks;

namespace BPT_Service.Application.PostService.Query.Extension.GetOwnServiceInformation
{
    public class GetOwnServiceInformationQuery : IGetOwnServiceInformationQuery
    {
        private readonly IRepository<Model.Entities.ServiceModel.ProviderServiceModel.ProviderService, int> _providerServiceRepository;
        private readonly IRepository<Model.Entities.ServiceModel.UserServiceModel.UserService, int> _userServiceRepository;
        private readonly IRepository<Provider, Guid> _providerRepository;

        public GetOwnServiceInformationQuery(
            IRepository<Model.Entities.ServiceModel.ProviderServiceModel.ProviderService, int> providerServiceRepository,
            IRepository<Model.Entities.ServiceModel.UserServiceModel.UserService, int> userServiceRepository,
            IRepository<Provider, Guid> providerRepository)
        {
            _providerServiceRepository = providerServiceRepository;
            _userServiceRepository = userServiceRepository;
            _providerRepository = providerRepository;
        }

        public async Task<string> ExecuteAsync(string idService)
        {
            var findServiceByUserService = await _userServiceRepository.FindSingleAsync(x => x.ServiceId == Guid.Parse(idService));
            if (findServiceByUserService == null)
            {
                var findServiceByProvider = await _providerServiceRepository.FindSingleAsync(x => x.ServiceId == Guid.Parse(idService));
                if (findServiceByProvider == null)
                {
                    return "";
                }
                else
                {
                    var findUserId = await _providerRepository.FindSingleAsync(x => x.Id == findServiceByProvider.ProviderId);
                    return findUserId.UserId.ToString();
                }
            }
            else
            {
                return findServiceByUserService.UserId.ToString();
            }
        }
    }
}