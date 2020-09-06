using BPT_Service.Application.PostService.ViewModel;
using BPT_Service.Model.Entities.ServiceModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BPT_Service.Application.PostService.Query.Extension.GetProviderInformation
{
    public class GetProviderInformationQuery : IGetProviderInformationQuery
    {
        public GetProviderInformationQuery()
        {
        }

        public ExtensionProviderViewModel ExecuteAsync(Guid idService, IEnumerable<Service> service, IEnumerable<Provider> provider, IEnumerable<Model.Entities.ServiceModel.ProviderServiceModel.ProviderService> providerService)
        {
            var ser = service.Where(x => x.Id == idService).FirstOrDefault();
            if (ser == null)
            {
                return new ExtensionProviderViewModel()
                {
                    idProvider = "",
                    NameProvider = ""
                };
            }
            var findProvider = providerService.Where(x => x.ServiceId == ser.Id).FirstOrDefault();
            if (findProvider == null)
            {
                return new ExtensionProviderViewModel()
                {
                    idProvider = "",
                    NameProvider = ""
                };
            }
            var getProvider = provider.Where(x => x.Id == findProvider.ProviderId).FirstOrDefault();
            if (getProvider != null)
            {
                return new ExtensionProviderViewModel()
                {
                    idProvider = getProvider.Id.ToString(),
                    NameProvider = "Provider: " + getProvider.ProviderName
                };
            }
            return new ExtensionProviderViewModel()
            {
                idProvider = "",
                NameProvider = ""
            };
        }
    }
}