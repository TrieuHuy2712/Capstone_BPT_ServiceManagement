using BPT_Service.Model.Entities.ServiceModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BPT_Service.Application.PostService.Query.Extension.GetServiceRating
{
    public class GetServiceRatingQuery : IGetServiceRatingQuery
    {
        public GetServiceRatingQuery()
        {
        }

        public double ExecuteAsync(Guid IdService, IEnumerable<ServiceRating> serviceRatings)
        {
            var getAllRatingOfService = serviceRatings.Where(x => x.ServiceId == IdService).ToList();
            if (getAllRatingOfService.Count() == 0)
            {
                return 0;
            }
            return getAllRatingOfService.Average(x => x.NumberOfRating);
        }
    }
}