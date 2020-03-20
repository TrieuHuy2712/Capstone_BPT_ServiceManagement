using BPT_Service.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPT_Service.Application.PostService.Query.Extension.GetListTagInformation
{
    public class GetListTagInformationQuery : IGetListTagInformationQuery
    {
        public GetListTagInformationQuery()
        {
        }

        public string ExecuteAsync(Guid idService, IEnumerable<Model.Entities.ServiceModel.TagService> tagServices, IEnumerable<Tag> allTag)
        {
            var getAllServiceTag = tagServices.Where(x => x.ServiceId == idService).ToList();
            var query = (from serviceTag in getAllServiceTag.ToList()
                         join tag in allTag.ToList()
                         on serviceTag.TagId equals tag.Id
                         select new
                         {
                             tag.TagName
                         }).ToString();
            return query;
        }
    }
}
