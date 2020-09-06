using BPT_Service.Model.Enums;

namespace BPT_Service.Application.RecommedationService.ViewModel
{
    public class AddRecommendationViewModel
    {
        public int Id { get; set; }
        public string IdType { get; set; }
        public TypeRecommendation Type { get; set; }
        public int Order { get; set; }
    }
}