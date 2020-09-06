namespace BPT_Service.Application.RecommedationService.ViewModel
{
    public class ServiceRecommendationViewModel
    {
        public int Id { get; set; }
        public string IdService { get; set; }
        public string NameService { get; set; }
        public int Order { get; set; }
        public double Rating { get; set; }
        public string ImgService { get; set; }
    }
}