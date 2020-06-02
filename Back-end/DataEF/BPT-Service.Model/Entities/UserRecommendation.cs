using BPT_Service.Model.Interfaces;
using System;

namespace BPT_Service.Model
{
    public class UserRecommendation : IDateTracking
    {
        public int Id { get; set; }
        public Guid UserId { get; set; }
        public Guid ServiceId { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
    }
}