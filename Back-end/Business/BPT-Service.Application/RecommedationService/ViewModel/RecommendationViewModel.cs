using BPT_Service.Application.NewsProviderService.ViewModel;
using BPT_Service.Application.PostService.ViewModel;
using System.Collections.Generic;

namespace BPT_Service.Application.RecommedationService.ViewModel
{
    public class RecommendationViewModel
    {
        public string EmailUser { get; set; }
        public List<NewsProviderViewModel> NewsProviderViewModel { get; set; }
        public List<PostServiceViewModel> PostServiceViewModel { get; set; }
    }
}