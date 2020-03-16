using BPT_Service.Model.Entities.ServiceModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BPT_Service.Application.PostService.Query.Extension.GetAvtInformation
{
    public class GetAvtInformationQuery : IGetAvtInformationQuery
    {
        public GetAvtInformationQuery()
        {
        }

        public string ExecuteAsync(Guid idService, IEnumerable<ServiceImage> image)
        {
            var img = image.Where(x => x.ServiceId == idService).FirstOrDefault();
            if (img == null)
            {
                return "";
            }
            return img.Path;
        }
    }
}