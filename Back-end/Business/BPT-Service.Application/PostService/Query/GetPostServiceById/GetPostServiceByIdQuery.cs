using BPT_Service.Application.CategoryService.Query.GetByIDCategoryService;
using BPT_Service.Application.PostService.Query.Extension.GetProviderInformation;
using BPT_Service.Application.PostService.Query.Extension.GetUserInformation;
using BPT_Service.Application.PostService.ViewModel;
using BPT_Service.Common;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Entities.ServiceModel;
using BPT_Service.Model.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BPT_Service.Application.PostService.Query.GetPostServiceById
{
    public class GetPostServiceByIdQuery : IGetPostServiceByIdQuery
    {
        private readonly IRepository<Service, Guid> _serviceRepository;
        private readonly UserManager<AppUser> _userManager;
        private readonly IHttpContextAccessor _httpContext;
        private readonly IRepository<Tag, Guid> _tagRepository;
        private readonly IRepository<Model.Entities.ServiceModel.TagService, int> _tagServiceRepository;
        private readonly IRepository<ServiceImage, int> _imageRepository;
        private readonly IRepository<Provider, Guid> _providerRepository;
        private readonly IGetByIDCategoryServiceQuery _getByIDCategoryServiceQuery;
        private readonly IGetProviderInformationQuery _getProviderInformationQuery;
        private readonly IGetUserInformationQuery _getUserInformationQuery;
        private readonly IRepository<Model.Entities.ServiceModel.UserServiceModel.UserService, int> _userServiceRepository;
        private readonly IRepository<Model.Entities.ServiceModel.ProviderServiceModel.ProviderService, int> _providerServiceRepository;

        public GetPostServiceByIdQuery(
            IRepository<Service, Guid> serviceRepository,
            UserManager<AppUser> userManager,
            IHttpContextAccessor httpContext,
            IRepository<Tag, Guid> tagRepository,
            IRepository<Model.Entities.ServiceModel.TagService, int> tagServiceRepository,
            IRepository<ServiceImage, int> imageRepository,
            IGetByIDCategoryServiceQuery getByIDCategoryServiceQuery,
            IRepository<Model.Entities.ServiceModel.UserServiceModel.UserService, int> userServiceRepository,
            IRepository<Model.Entities.ServiceModel.ProviderServiceModel.ProviderService, int> providerServiceRepository,
            IRepository<Provider, Guid> providerRepository,
            IGetProviderInformationQuery getProviderInformationQuery,
            IGetUserInformationQuery getUserInformationQuery)
        {
            _serviceRepository = serviceRepository;
            _userManager = userManager;
            _httpContext = httpContext;
            _tagRepository = tagRepository;
            _tagServiceRepository = tagServiceRepository;
            _imageRepository = imageRepository;
            _getByIDCategoryServiceQuery = getByIDCategoryServiceQuery;
            _userServiceRepository = userServiceRepository;
            _providerServiceRepository = providerServiceRepository;
            _providerRepository = providerRepository;
            _getProviderInformationQuery = getProviderInformationQuery;
            _getUserInformationQuery = getUserInformationQuery;
        }

        public async Task<PostServiceViewModel> ExecuteAsync(string idService)
        {
            try
            {
                var service = await _serviceRepository.FindByIdAsync(Guid.Parse(idService));
                if (service != null && service.Status == Model.Enums.Status.Active)
                {
                    var findUserService = await _userServiceRepository.FindSingleAsync(x => x.ServiceId == service.Id);
                    if (findUserService == null)
                    {
                        var findProviderService = await _providerServiceRepository.FindSingleAsync(x => x.ServiceId == service.Id);
                        if (findProviderService != null)
                        {
                            var findProvider = await _providerRepository.FindSingleAsync(x => x.Id == findProviderService.ProviderId);
                            return MapViewModel(service, null, findProvider).Result;
                        }
                        else
                        {
                            return MapViewModel(service, null, null).Result;
                        }
                    }
                    else
                    {
                        var findUser = await _userManager.FindByIdAsync(findUserService.UserId.ToString());
                        return MapViewModel(service, findUser, null).Result;
                    }
                }
                else
                {
                    return null;
                }
            }
            catch (System.Exception ex)
            {
                return null;
            }
        }

        private async Task<PostServiceViewModel> MapViewModel(
            Service serv, AppUser user, Provider provider)
        {
            var query = await _serviceRepository.FindAllAsync();
            var providerService = await _providerServiceRepository.FindAllAsync();
            var userService = await _userServiceRepository.FindAllAsync();
            var providers = await _providerRepository.FindAllAsync();


            var getTag = await _tagRepository.FindAllAsync();
            var getUserTag = await _tagServiceRepository.FindAllAsync(x => x.ServiceId == serv.Id);
            var getListTag = (from tag in getTag.ToList()
                              join serviceTag in getUserTag.ToList()
                              on tag.Id equals serviceTag.TagId
                              select new
                              {
                                  tag.TagName
                              }).ToList();

            var getImage = await _imageRepository.FindAllAsync(x => x.ServiceId == serv.Id);
            PostServiceViewModel postServiceView = new PostServiceViewModel();
            postServiceView.Id = serv.Id.ToString();
            postServiceView.listImages = getImage.Select(x => new PostServiceImageViewModel
            {
                Path = x.Path,
                ImageId = x.Id
            }).ToList();
            postServiceView.PriceOfService = serv.PriceOfService;
            postServiceView.CategoryName = _getByIDCategoryServiceQuery.ExecuteAsync(serv.CategoryId).Result.CategoryName;
            postServiceView.ServiceName = serv.ServiceName;
            postServiceView.Status = serv.Status;
            postServiceView.tagofServices = getListTag.Select(x => new TagofServiceViewModel
            {
                TagName = x.TagName
            }).ToList();
            postServiceView.Description = serv.Description;
            postServiceView.ProviderId = provider.Id.ToString();
            postServiceView.CategoryId = serv.CategoryId;
            postServiceView.Author =
                _getProviderInformationQuery.ExecuteAsync(serv.Id, query, providers, providerService).NameProvider
                               == "" ? _getUserInformationQuery.ExecuteAsync(serv.Id, query, userService) :
                               _getProviderInformationQuery.ExecuteAsync(serv.Id, query, providers, providerService).NameProvider;
            return postServiceView;
        }
    }
}