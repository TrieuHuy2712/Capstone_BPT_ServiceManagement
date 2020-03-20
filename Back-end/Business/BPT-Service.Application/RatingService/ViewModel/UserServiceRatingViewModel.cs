using System;

namespace BPT_Service.Application.RatingService.ViewModel
{
    public class UserServiceRatingViewModel
    {
        public int IdRating { get; set; }
        public string IdService { get; set; }
        public int MyRating { get; set; }
        public double AverageOfRating { get; set; }
        public DateTime CreateDate { get; set; }
        public string IdUser { get; set; }
    }
}