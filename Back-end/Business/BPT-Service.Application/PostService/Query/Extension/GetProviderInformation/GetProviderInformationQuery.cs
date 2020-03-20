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

        public string ExecuteAsync(Guid idService, IEnumerable<Service> service, IEnumerable<Provider> provider, IEnumerable<Model.Entities.ServiceModel.ProviderServiceModel.ProviderService> providerService)
        {
            var ser = service.Where(x => x.Id == idService).FirstOrDefault();
            if (ser == null)
            {
                return "";
            }
            var findProvider = providerService.Where(x => x.ServiceId == ser.Id).FirstOrDefault();
            if (findProvider == null)
            {
                return "";
            }
            var getProvider = provider.Where(x => x.Id == findProvider.ProviderId).FirstOrDefault();
            if (getProvider != null)
            {
                return "Provider: " + getProvider.ProviderName;
            }
            return "";
        }
    }
}