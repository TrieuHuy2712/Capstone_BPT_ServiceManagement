using BPT_Service.Model.Entities;
using System;
using System.Collections.Generic;

namespace BPT_Service.Application.PostService.Query.Extension.GetListTagInformation
{
    public interface IGetListTagInformationQuery
    {
        string ExecuteAsync(Guid idService, IEnumerable<Model.Entities.ServiceModel.TagService> tagServices, IEnumerable<Tag> allTag);
    }
}