using System.Collections.Generic;

namespace BPT_Service.Application.RatingService.ViewModel
{
    public class ListRatingByServiceViewModel
    {
        public string ServiceName { get; set; }
        public double AverageRating { get; set; }
        public List<ServiceRatingViewModel> listRating { get; set; }
    }
}