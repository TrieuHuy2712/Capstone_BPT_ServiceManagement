using BPT_Service.Application.NewsProviderService.Query.GetByIdProviderNewsService;
using BPT_Service.Application.PostService.Query.GetPostServiceById;
using BPT_Service.Application.RecommedationService.ViewModel;
using BPT_Service.Common.Dtos;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Entities.ServiceModel;
using BPT_Service.Model.Entities.ServiceModel.ProviderServiceModel;
using BPT_Service.Model.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BPT_Service.Application.RecommedationService.Query.RecommendService
{
    public class RecommendService : IRecommendService
    {
        private readonly IGetByIdProviderNewsServiceQuery _getByIdProviderNewService;
        private readonly IGetPostServiceByIdQuery _getPostServiceByIdQuery;
        private readonly IRepository<Model.Entities.ServiceModel.ProviderServiceModel.ProviderService, int> _providerServiceRepository;
        private readonly IRepository<Provider, Guid> _providerRepository;
        private readonly IRepository<ProviderFollowing, int> _providerFollowingRepository;
        private readonly IRepository<ProviderNew, int> _providerNewRepository;
        private readonly IRepository<Service, Guid> _serviceRepository;
        private readonly UserManager<AppUser> _userManager;
        private readonly IOptions<EmailConfigModel> _configEmail;

        public RecommendService(
            IRepository<Provider, Guid> providerRepository,
            IRepository<ProviderFollowing, int> providerFollowingRepository,
            IRepository<ProviderNew, int> providerNewRepository,
            IRepository<Service, Guid> serviceRepository,
            IRepository<Model.Entities.ServiceModel.ProviderServiceModel.ProviderService, int> providerServiceRepository,
            IGetByIdProviderNewsServiceQuery getByIdProviderNewService,
            IGetPostServiceByIdQuery getPostServiceByIdQuery,
            UserManager<AppUser> userManager,
            IOptions<EmailConfigModel> configEmail)
        {
            _getByIdProviderNewService = getByIdProviderNewService;
            _providerFollowingRepository = providerFollowingRepository;
            _providerNewRepository = providerNewRepository;
            _providerRepository = providerRepository;
            _providerServiceRepository = providerServiceRepository;
            _serviceRepository = serviceRepository;
            _getPostServiceByIdQuery = getPostServiceByIdQuery;
            _userManager = userManager;
            _configEmail = configEmail;
        }

        public async Task ExecuteAsync()
        {
            try
            {
                #region GetAll Data

                var getAllProviderNew = await _providerNewRepository.FindAllAsync(x => x.DateCreated < DateTime.Now
                && x.DateCreated >= DateTime.Now.AddDays(-1) && x.Status == Model.Enums.Status.Active);

                var getAllService = await _serviceRepository.FindAllAsync(x => x.DateCreated < DateTime.Now
                && x.DateCreated > DateTime.Now.AddDays(-1) && x.Status == Model.Enums.Status.Active);

                var getAllFollowing = await _providerFollowingRepository.FindAllAsync();
                var getAllUser = await _userManager.Users.ToListAsync();
                var getAllProvider = await _providerRepository.FindAllAsync(x => x.Status == Model.Enums.Status.Active);
                var getAllProviderService = await _providerServiceRepository.FindAllAsync();

                #endregion GetAll Data

                // User-->UserFollowing-->Provider-->ProviderNews
                var joinAllInformation = (from user in getAllUser.ToList()
                                          join following in getAllFollowing.ToList()
                                          on user.Id equals following.UserId
                                          join provider in getAllProvider.ToList()
                                          on following.ProviderId equals provider.Id
                                          join news in getAllProviderNew.ToList()
                                          on provider.Id equals news.ProviderId
                                          where (following.IsReceiveEmail == true && news.Status == Model.Enums.Status.Active)
                                          orderby user.Id
                                          select new
                                          {
                                              IdUser = user.Id,
                                              Information = news.Id.ToString()
                                          }).ToList().
                                    Union(from user in getAllUser.ToList()
                                          join following in getAllFollowing.ToList()
                                          on user.Id equals following.UserId
                                          join providerService in getAllProviderService.ToList()
                                          on following.ProviderId equals providerService.ProviderId
                                          join service in getAllService.ToList()
                                          on providerService.ServiceId equals service.Id
                                          where (following.IsReceiveEmail == true && service.Status == Model.Enums.Status.Active)
                                          select new
                                          {
                                              IdUser = user.Id,
                                              Information = service.Id.ToString()
                                          }).ToList();

                if (joinAllInformation.Count > 0)
                {
                    var groupInformatin = joinAllInformation.GroupBy(p => p.IdUser).Select(x => new
                    {
                        IdUser = x.Key,
                        Information = x.Select(t => new { t.Information }).ToList()
                    });

                    //Create List for add.
                    List<RecommendationViewModel> listRecommendService = new List<RecommendationViewModel>();
                    foreach (var item in groupInformatin)
                    {
                        RecommendationViewModel recommendService = new RecommendationViewModel();
                        recommendService.EmailUser = _userManager.FindByIdAsync(item.IdUser.ToString()).Result.Email;
                        foreach (var detail in item.Information)
                        {
                            var number = 0;
                            if (Int32.TryParse(detail.Information, out number))
                            {
                                var resultNews = await _getByIdProviderNewService.ExecuteAsync(number);
                                recommendService.NewsProviderViewModel.Add(resultNews.myModel);
                            }
                            else
                            {
                                var resultProvider = await _getPostServiceByIdQuery.ExecuteAsync(detail.Information);
                                recommendService.PostServiceViewModel.Add(resultProvider);
                            }
                        }
                        listRecommendService.Add(recommendService);
                    }
                    foreach (var item in listRecommendService)
                    {
                        ContentEmail(_configEmail.Value.SendGridKey, "Email recommendation",
                    item.NewsProviderViewModel.ToString() + "</br>" + item.PostServiceViewModel.ToString(), item.EmailUser).Wait();
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task ContentEmail(string apiKey, string subject1, string message, string email)
        {
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress(_configEmail.Value.FromUserEmail, _configEmail.Value.FullUserName);
            var subject = subject1;
            var to = new EmailAddress(email);
            var plainTextContent = message;
            var htmlContent = "<strong>" + message + "</strong>";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);
        }
    }
}