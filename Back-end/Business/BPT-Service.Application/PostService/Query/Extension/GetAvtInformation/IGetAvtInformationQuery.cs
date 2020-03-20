using BPT_Service.Model.Entities.ServiceModel;
using System;
using System.Collections.Generic;

namespace BPT_Service.Application.PostService.Query.Extension.GetAvtInformation
{
    public interface IGetAvtInformationQuery
    {
        string ExecuteAsync(Guid idService, IEnumerable<ServiceImage> image);
    }
}