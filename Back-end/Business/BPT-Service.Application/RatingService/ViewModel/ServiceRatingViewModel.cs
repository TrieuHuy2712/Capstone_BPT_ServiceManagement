using System;

namespace BPT_Service.Application.RatingService.ViewModel
{
    public class ServiceRatingViewModel
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string ServiceId { get; set; }
        public string ServiceName { get; set; }
        public string UserNameWithEmail { get; set; }
        public int NumberOfRating { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
    }
}