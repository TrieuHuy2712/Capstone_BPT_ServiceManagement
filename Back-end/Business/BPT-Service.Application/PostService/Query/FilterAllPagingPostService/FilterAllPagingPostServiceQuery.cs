using BPT_Service.Application.CategoryService.Query.GetAllAsyncCategoryService;
using BPT_Service.Application.PostService.Query.Extension.GetAvtInformation;
using BPT_Service.Application.PostService.Query.Extension.GetListTagInformation;
using BPT_Service.Application.PostService.Query.Extension.GetProviderInformation;
using BPT_Service.Application.PostService.Query.Extension.GetServiceRating;
using BPT_Service.Application.PostService.Query.Extension.GetUserInformation;
using BPT_Service.Application.PostService.ViewModel;
using BPT_Service.Common.Constants;
using BPT_Service.Common.Dtos;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Entities.ServiceModel;
using BPT_Service.Model.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BPT_Service.Application.PostService.Query.FilterAllPagingPostService
{
    public class FilterAllPagingPostServiceQuery : IFilterAllPagingPostServiceQuery
    {
        private readonly IGetAllAsyncCategoryServiceQuery _getAllAsyncCategoryServiceQuery;
        private readonly IRepository<CityProvince, int> _locationRepository;
        private readonly IRepository<Model.Entities.ServiceModel.ProviderServiceModel.ProviderService, int> _providerServiceRepository;
        private readonly IRepository<Model.Entities.ServiceModel.TagService, int> _tagServiceRepository;
        private readonly IRepository<Model.Entities.ServiceModel.UserServiceModel.UserService, int> _userServiceRepository;
        private readonly IRepository<Provider, Guid> _providerRepository;
        private readonly IRepository<Service, Guid> _serviceRepository;
        private readonly IRepository<ServiceImage, int> _imageRepository;
        private readonly IRepository<Tag, Guid> _tagRepository;
        private readonly UserManager<AppUser> _userManager;
        private readonly IRepository<ServiceRating, int> _ratingRepository;
        private readonly IGetAvtInformationQuery _getAvtInformationQuery;
        private readonly IGetListTagInformationQuery _getListTagInformationQuery;
        private readonly IGetProviderInformationQuery _getProviderInformationQuery;
        private readonly IGetServiceRatingQuery _getServiceRatingQuery;
        private readonly IGetUserInformationQuery _getUserInformationQuery;

        public FilterAllPagingPostServiceQuery(
            IGetAllAsyncCategoryServiceQuery getAllAsyncCategoryServiceQuery,
            IRepository<CityProvince, int> locationRepository,
            IRepository<Model.Entities.ServiceModel.ProviderServiceModel.ProviderService, int> providerServiceRepository,
            IRepository<Model.Entities.ServiceModel.TagService, int> tagServiceRepository,
            IRepository<Model.Entities.ServiceModel.UserServiceModel.UserService, int> userServiceRepository,
            IRepository<Provider, Guid> providerRepository,
            IRepository<Service, Guid> serviceRepository,
            IRepository<ServiceRating, int> ratingRepository,
            IRepository<Tag, Guid> tagRepository,
            UserManager<AppUser> userManager,
            IGetAvtInformationQuery getAvtInformationQuery,
            IGetListTagInformationQuery getListTagInformationQuery,
            IGetProviderInformationQuery getProviderInformationQuery,
            IGetServiceRatingQuery getServiceRatingQuery,
            IGetUserInformationQuery getUserInformationQuery,
            IRepository<ServiceImage, int> imageRepository)
        {
            _getAllAsyncCategoryServiceQuery = getAllAsyncCategoryServiceQuery;
            _locationRepository = locationRepository;
            _providerRepository = providerRepository;
            _providerServiceRepository = providerServiceRepository;
            _ratingRepository = ratingRepository;
            _serviceRepository = serviceRepository;
            _tagRepository = tagRepository;
            _tagServiceRepository = tagServiceRepository;
            _userManager = userManager;
            _userServiceRepository = userServiceRepository;
            _getAvtInformationQuery = getAvtInformationQuery;
            _getListTagInformationQuery = getListTagInformationQuery;
            _getProviderInformationQuery = getProviderInformationQuery;
            _getServiceRatingQuery = getServiceRatingQuery;
            _getUserInformationQuery = getUserInformationQuery;
            _imageRepository = imageRepository;
        }

        public async Task<PagedResult<ListServiceViewModel>> ExecuteAsync(int page, int pageSize, string typeFilter, string filterName)
        {
            try
            {
                var query = await _serviceRepository.FindAllAsync();

                //Get provider information
                var provider = await _providerRepository.FindAllAsync();
                var provideService = await _providerServiceRepository.FindAllAsync();

                //Get user information
                var userService = await _userServiceRepository.FindAllAsync();

                //Get image
                var getAvatar = await _imageRepository.FindAllAsync(x => x.isAvatar == true);

                //Get all tag
                var getAllTag = await _tagRepository.FindAllAsync();
                var getAllServiceTag = await _tagServiceRepository.FindAllAsync();

                //Get allRating
                var allRating = await _ratingRepository.FindAllAsync();

                if(typeFilter == KindOfDetailService.FILTER_BY_LOCATION)
                {
                    var listViewModelLocation = await FilterByLocation(query, filterName, provider, provideService, userService, getAvatar, getAllTag, getAllServiceTag, allRating);
                    int totalRowSearch = listViewModelLocation.Count();
                    listViewModelLocation = pageSize==0 ? listViewModelLocation.ToList() : listViewModelLocation.Skip((page - 1) * pageSize).Take(pageSize).ToList();
                    return new PagedResult<ListServiceViewModel>
                    {
                        Results = listViewModelLocation,
                        CurrentPage = page,
                        RowCount = totalRowSearch,
                        PageSize = pageSize
                    };
                }
                else if(typeFilter == KindOfDetailService.FILTER_BY_CATEGORY)
                {
                    
                    var listViewModelCategory = await FilterByCategory(query, filterName, provider, provideService, userService, getAvatar, getAllTag, getAllServiceTag, allRating);
                    int totalRowCategorySearch = listViewModelCategory.Count();
                    listViewModelCategory = listViewModelCategory.Skip((page - 1) * pageSize).Take(pageSize).ToList();
                    return new PagedResult<ListServiceViewModel>
                    {
                        Results = listViewModelCategory,
                        CurrentPage = page,
                        RowCount = totalRowCategorySearch,
                        PageSize = pageSize
                    };
                }
                else if(typeFilter == KindOfDetailService.FILTER_BY_TAG)
                {
                    var listViewModelTag = await FilterByTag(query, filterName, provider, provideService, userService, getAvatar, getAllTag, getAllServiceTag, allRating);
                    int totalRowTagSearch = listViewModelTag.Count();
                    listViewModelTag = listViewModelTag.Skip((page - 1) * pageSize).Take(pageSize).ToList();
                    return new PagedResult<ListServiceViewModel>
                    {
                        Results = listViewModelTag,
                        CurrentPage = page,
                        RowCount = totalRowTagSearch,
                        PageSize = pageSize
                    };
                }
                else
                {
                    return new PagedResult<ListServiceViewModel>()
                    {
                        Results = null,
                        CurrentPage = page,
                        RowCount = 0,
                        PageSize = pageSize
                    };
                }
            }
            catch (Exception)
            {
                return new PagedResult<ListServiceViewModel>()
                {
                    Results = null,
                    CurrentPage = page,
                    RowCount = 0,
                    PageSize = pageSize
                };
            }
        }

        private async Task<List<ListServiceViewModel>> FilterByTag(IEnumerable<Service> service, string filterName,
           IEnumerable<Provider> provider,
           IEnumerable<Model.Entities.ServiceModel.ProviderServiceModel.ProviderService> provideService,
           IEnumerable<Model.Entities.ServiceModel.UserServiceModel.UserService> userService,
           IEnumerable<ServiceImage> getAvatar, IEnumerable<Tag> getAllTag,
           IEnumerable<Model.Entities.ServiceModel.TagService> getAllServiceTag,
           IEnumerable<ServiceRating> allRating)
        {
            var category = await _getAllAsyncCategoryServiceQuery.ExecuteAsync();
            var query = (from tag in getAllTag.ToList()
                         join userTag in getAllServiceTag.ToList()
                         on tag.Id equals userTag.TagId
                         join serv in service
                         on userTag.ServiceId equals serv.Id
                         where tag.TagName != null && tag.TagName == filterName && serv.Status== Model.Enums.Status.Active
                         select new ListServiceViewModel
                         {
                             Id = serv.Id,
                             CategoryName = category.Where(x => x.Id == serv.CategoryId).Select(x => x.CategoryName).FirstOrDefault(),
                             Author = _getProviderInformationQuery.ExecuteAsync(serv.Id, service, provider, provideService).NameProvider
                            == "" ? _getUserInformationQuery.ExecuteAsync(serv.Id, service, userService) : _getProviderInformationQuery.ExecuteAsync(serv.Id, service, provider, provideService).NameProvider,
                             Status = serv.Status,
                             isProvider = _getProviderInformationQuery.ExecuteAsync(serv.Id, service, provider, provideService).NameProvider == "" ? false : true,
                             AvtService = _getAvtInformationQuery.ExecuteAsync(serv.Id, getAvatar),
                             PriceOfService = serv.PriceOfService.ToString(),
                             TagList = _getListTagInformationQuery.ExecuteAsync(serv.Id, getAllServiceTag, getAllTag),
                             ServiceName = serv.ServiceName,
                             Rating = _getServiceRatingQuery.ExecuteAsync(serv.Id, allRating)
                         }).OrderByDescending(x=>x.Rating).ToList();
            return query;
        }

        private async Task<List<ListServiceViewModel>> FilterByCategory(IEnumerable<Service> service, string filterName,
            IEnumerable<Provider> provider,
            IEnumerable<Model.Entities.ServiceModel.ProviderServiceModel.ProviderService> provideService,
            IEnumerable<Model.Entities.ServiceModel.UserServiceModel.UserService> userService,
            IEnumerable<ServiceImage> getAvatar, IEnumerable<Tag> getAllTag,
            IEnumerable<Model.Entities.ServiceModel.TagService> getAllServiceTag,
            IEnumerable<ServiceRating> allRating)
        {
            var getAllCategory = await _getAllAsyncCategoryServiceQuery.ExecuteAsync();
            var query = (from serv in service.ToList()
                         join category in getAllCategory.ToList()
                         on serv.CategoryId equals category.Id
                         where category.CategoryName != null && category.CategoryName == filterName && serv.Status == Model.Enums.Status.Active
                         select new ListServiceViewModel
                         {
                             Id = serv.Id,
                             CategoryName = category.CategoryName,
                             Author = _getProviderInformationQuery.ExecuteAsync(serv.Id, service, provider, provideService).NameProvider
                            == "" ? _getUserInformationQuery.ExecuteAsync(serv.Id, service, userService) : _getProviderInformationQuery.ExecuteAsync(serv.Id, service, provider, provideService).NameProvider,
                             Status = serv.Status,
                             isProvider = _getProviderInformationQuery.ExecuteAsync(serv.Id, service, provider, provideService).NameProvider == "" ? false : true,
                             AvtService = _getAvtInformationQuery.ExecuteAsync(serv.Id, getAvatar),
                             PriceOfService = serv.PriceOfService.ToString(),
                             TagList = _getListTagInformationQuery.ExecuteAsync(serv.Id, getAllServiceTag, getAllTag),
                             ServiceName = serv.ServiceName,
                             Rating = _getServiceRatingQuery.ExecuteAsync(serv.Id, allRating)
                         }).OrderByDescending(x=>x.Rating).ToList();
            return query;
        }

        private async Task<List<ListServiceViewModel>> FilterByLocation(IEnumerable<Service> service, string filterName,
            IEnumerable<Provider> provider,
            IEnumerable<Model.Entities.ServiceModel.ProviderServiceModel.ProviderService> provideService,
            IEnumerable<Model.Entities.ServiceModel.UserServiceModel.UserService> userService,
            IEnumerable<ServiceImage> getAvatar, IEnumerable<Tag> getAllTag,
            IEnumerable<Model.Entities.ServiceModel.TagService> getAllServiceTag,
            IEnumerable<ServiceRating> allRating)
        {
            var location = await _locationRepository.FindAllAsync();
            var getAllCategory = await _getAllAsyncCategoryServiceQuery.ExecuteAsync();
            var query = (from serv in service.ToList()
                         join providerService in provideService.ToList()
                         on serv.Id equals providerService.ServiceId
                         join pro in provider
                         on providerService.ProviderId equals pro.Id
                         join loc in location.ToList()
                         on pro.CityId equals loc.Id
                         where (loc.City != null && loc.City == filterName) || (loc.Province != null && loc.Province == filterName) 
                         && serv.Status == Model.Enums.Status.Active
                         select new ListServiceViewModel
                         {
                             Id = serv.Id,
                             CategoryId = serv.CategoryId,
                             CategoryName = getAllCategory.Where(x => x.Id == serv.CategoryId).Select(x => x.CategoryName).FirstOrDefault(),
                             Author = _getProviderInformationQuery.ExecuteAsync(serv.Id, service, provider, provideService).NameProvider
                            == "" ? _getUserInformationQuery.ExecuteAsync(serv.Id, service, userService) : _getProviderInformationQuery.ExecuteAsync(serv.Id, service, provider, provideService).NameProvider,
                             Status = serv.Status,
                             isProvider = _getProviderInformationQuery.ExecuteAsync(serv.Id, service, provider, provideService).NameProvider == "" ? false : true,
                             AvtService = _getAvtInformationQuery.ExecuteAsync(serv.Id, getAvatar),
                             PriceOfService = serv.PriceOfService.ToString(),
                             TagList = _getListTagInformationQuery.ExecuteAsync(serv.Id, getAllServiceTag, getAllTag),
                             ServiceName = serv.ServiceName,
                             
                             Rating = _getServiceRatingQuery.ExecuteAsync(serv.Id, allRating)
                         }).OrderByDescending(x=>x.Rating).ToList();
            return query;
        }
    }
}