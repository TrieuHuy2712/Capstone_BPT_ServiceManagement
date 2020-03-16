using BPT_Service.Model.Entities.ServiceModel;
using System;
using System.Collections.Generic;

namespace BPT_Service.Application.PostService.Query.Extension.GetServiceRating
{
    public interface IGetServiceRatingQuery
    {
        double ExecuteAsync(Guid IdService, IEnumerable<ServiceRating> serviceRatings);
    }
}