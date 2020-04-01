using BPT_Service.Application.CategoryService.Query.GetAllAsyncCategoryService;
using BPT_Service.Application.PostService.Query.Extension.GetAvtInformation;
using BPT_Service.Application.PostService.Query.Extension.GetListTagInformation;
using BPT_Service.Application.PostService.Query.Extension.GetProviderInformation;
using BPT_Service.Application.PostService.Query.Extension.GetServiceRating;
using BPT_Service.Application.PostService.Query.Extension.GetUserInformation;
using BPT_Service.Application.PostService.ViewModel;
using BPT_Service.Common.Dtos;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Entities.ServiceModel;
using BPT_Service.Model.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BPT_Service.Application.PostService.Query.GetAllPagingPostService
{
    public class GetAllPagingPostServiceQuery : IGetAllPagingPostServiceQuery
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

        public GetAllPagingPostServiceQuery(
            IGetAllAsyncCategoryServiceQuery getAllAsyncCategoryServiceQuery,
            IRepository<CityProvince, int> locationRepository,
            IRepository<Model.Entities.ServiceModel.ProviderServiceModel.ProviderService, int> providerServiceRepository,
            IRepository<Model.Entities.ServiceModel.TagService, int> tagServiceRepository,
            IRepository<Model.Entities.ServiceModel.UserServiceModel.UserService, int> userServiceRepository,
            IRepository<Provider, Guid> providerRepository,
            IRepository<Service, Guid> serviceRepository,
            IRepository<ServiceRating, int> ratingRepository,
            IRepository<ServiceImage, int> imageRepository,
            IRepository<Tag, Guid> tagRepository,
            IRepository<ServiceImage, int> imageRepository,
            UserManager<AppUser> userManager,
            IGetAvtInformationQuery getAvtInformationQuery,
            IGetListTagInformationQuery getListTagInformationQuery,
            IGetProviderInformationQuery getProviderInformationQuery,
            IGetServiceRatingQuery getServiceRatingQuery,
            IGetUserInformationQuery getUserInformationQuery)
        {
            _getAllAsyncCategoryServiceQuery = getAllAsyncCategoryServiceQuery;
            _locationRepository = locationRepository;
            _providerRepository = providerRepository;
            _providerServiceRepository = providerServiceRepository;
            _ratingRepository = ratingRepository;
            _serviceRepository = serviceRepository;
            _tagRepository = tagRepository;
            _imageRepository = imageRepository;
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

        public async Task<PagedResult<ListServiceViewModel>> ExecuteAsync(string keyword, int page, int pageSize, bool isAdminPage)
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

                //Get Category
                var getAllCateogry = await _getAllAsyncCategoryServiceQuery.ExecuteAsync();

                if (!string.IsNullOrEmpty(keyword))
                {
                    var listViewModel = await MappingTagService(query, keyword, provider, provideService, userService, getAvatar, getAllTag, getAllServiceTag, allRating);
                    int totalRowSearch = listViewModel.Count();
                    listViewModel = listViewModel.Skip((page - 1) * pageSize).Take(pageSize).ToList();

                    return new PagedResult<ListServiceViewModel>
                    {
                        Results = isAdminPage == true ? listViewModel : listViewModel.Where(x => x.status == Model.Enums.Status.Active).ToList(),
                        CurrentPage = page,
                        RowCount = totalRowSearch,
                        PageSize = pageSize
                    };
                }

                int totalRow = query.Count();
                query = query.Skip((page - 1) * pageSize)
                   .Take(pageSize);

                var data = query.Where(x => x.Status == Model.Enums.Status.Active).Select(x => new ListServiceViewModel
                {
                    Id = x.Id,
                    CategoryName = getAllCateogry.Where(t => t.Id == x.CategoryId).Select(x => x.CategoryName).FirstOrDefault(),
                    Author = _getProviderInformationQuery.ExecuteAsync(x.Id, query, provider, provideService)
                                == "" ? _getUserInformationQuery.ExecuteAsync(x.Id, query, userService) : _getProviderInformationQuery.ExecuteAsync(x.Id, query, provider, provideService),
                    status = x.Status,
                    isProvider = _getProviderInformationQuery.ExecuteAsync(x.Id, query, provider, provideService) == "" ? false : true,
                    AvtService = _getAvtInformationQuery.ExecuteAsync(x.Id, getAvatar),
                    PriceOfService = x.PriceOfService.ToString(),
                    TagList = _getListTagInformationQuery.ExecuteAsync(x.Id, getAllServiceTag, getAllTag).ToString(),
                    ServiceName = x.ServiceName,
                    Rating = _getServiceRatingQuery.ExecuteAsync(x.Id, allRating)
                }).OrderByDescending(x => x.Rating).ToList();

                var paginationSet = new PagedResult<ListServiceViewModel>()
                {
                    Results = isAdminPage == true ? data : data.Where(x => x.status == Model.Enums.Status.Active).ToList(),
                    CurrentPage = page,
                    RowCount = totalRow,
                    PageSize = pageSize
                };

                return paginationSet;
            }
            catch (System.Exception ex)
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

        private async Task<List<ListServiceViewModel>> MappingTagService(IEnumerable<Service> services, string keyword,
            IEnumerable<Provider> provider,
            IEnumerable<Model.Entities.ServiceModel.ProviderServiceModel.ProviderService> provideService,
            IEnumerable<Model.Entities.ServiceModel.UserServiceModel.UserService> userService,
            IEnumerable<ServiceImage> getAvatar, IEnumerable<Tag> getAllTag,
            IEnumerable<Model.Entities.ServiceModel.TagService> getAllServiceTag,
            IEnumerable<ServiceRating> allRating)
        {
            var getAllCateogry = await _getAllAsyncCategoryServiceQuery.ExecuteAsync();
            var data = (from category in getAllCateogry.ToList()
                        join serv in services.ToList()
                        on category.Id equals serv.CategoryId
                        join usertag in getAllServiceTag.ToList()
                        on serv.Id equals usertag.ServiceId
                        join tag in getAllTag.ToList()
                        on usertag.TagId equals tag.Id
                        where (tag.TagName != null && tag.TagName.Contains(keyword)) && (serv.Status == Model.Enums.Status.Active)
                                || (category.CategoryName != null && category.CategoryName.Contains(keyword))
                                || (serv.ServiceName != null && serv.ServiceName.Contains(keyword))
                                || (serv.Description != null && serv.Description.Contains(keyword))
                        select new ListServiceViewModel
                        {
                            Id = serv.Id,
                            CategoryName = category.CategoryName,
                            Author = _getProviderInformationQuery.ExecuteAsync(serv.Id, services, provider, provideService)
                             == "" ? _getUserInformationQuery.ExecuteAsync(serv.Id, services, userService) : _getProviderInformationQuery.ExecuteAsync(serv.Id, services, provider, provideService),
                            status = serv.Status,
                            isProvider = _getProviderInformationQuery.ExecuteAsync(serv.Id, services, provider, provideService) == "" ? false : true,
                            ServiceName = serv.ServiceName,
                            PriceOfService = serv.PriceOfService.ToString(),
                            AvtService = _getAvtInformationQuery.ExecuteAsync(serv.Id, getAvatar),
                            TagList = _getListTagInformationQuery.ExecuteAsync(serv.Id, getAllServiceTag, getAllTag),
                            Rating = _getServiceRatingQuery.ExecuteAsync(serv.Id, allRating)
                        }).OrderByDescending(x => x.Rating).ToList();
            return data;
        }
    }
}