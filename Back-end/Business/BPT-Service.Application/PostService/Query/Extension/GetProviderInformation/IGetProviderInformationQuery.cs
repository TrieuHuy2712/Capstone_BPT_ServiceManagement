using BPT_Service.Model.Entities.ServiceModel;
using System;
using System.Collections.Generic;

namespace BPT_Service.Application.PostService.Query.Extension.GetProviderInformation
{
    public interface IGetProviderInformationQuery
    {
        string ExecuteAsync(Guid idService, IEnumerable<Service> service,IEnumerable<Provider> provider,
                IEnumerable<Model.Entities.ServiceModel.ProviderServiceModel.ProviderService> providerService);
    }
}