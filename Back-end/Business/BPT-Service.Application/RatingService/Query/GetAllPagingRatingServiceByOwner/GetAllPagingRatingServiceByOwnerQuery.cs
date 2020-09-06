using BPT_Service.Application.PermissionService.Query.CheckOwnService;
using BPT_Service.Application.RatingService.ViewModel;
using BPT_Service.Common.Dtos;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Entities.ServiceModel;
using BPT_Service.Model.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BPT_Service.Application.RatingService.Query.GetAllPagingRatingServiceByOwner
{
    public class GetAllPagingRatingServiceByOwnerQuery : IGetAllPagingRatingServiceByOwnerQuery
    {
        private readonly IRepository<ServiceRating, int> _serviceRatingRepository;
        private readonly UserManager<AppUser> _userRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICheckOwnService _checkOwnService;

        public GetAllPagingRatingServiceByOwnerQuery(IRepository<ServiceRating, int> serviceRatingRepository,
            IHttpContextAccessor httpContextAccessor,
            ICheckOwnService checkOwnService,
            UserManager<AppUser> userRepository)
        {
            _serviceRatingRepository = serviceRatingRepository;
            _httpContextAccessor = httpContextAccessor;
            _checkOwnService = checkOwnService;
            _userRepository = userRepository;
        }

        public async Task<PagedResult<ServiceRatingViewModel>> ExecuteAsync(string keyword, int page, int pageSize, string idService)
        {
            var getUserId = _httpContextAccessor.HttpContext.User.Identity.Name;
            if (await _checkOwnService.ExecuteAsync(getUserId, idService) == false)
            {
                return new PagedResult<ServiceRatingViewModel>
                {
                    CurrentPage = 0,
                    PageSize = 0
                };
            }
            var query = await _serviceRatingRepository.FindAllAsync(x => x.ServiceId == Guid.Parse(idService));

            int totalRow = query.Count();
            if (pageSize != 0)
            {
                query = query.Skip((page - 1) * pageSize).Take(pageSize);
            }

            var data = query.Select(x => new ServiceRatingViewModel
            {
                Id = x.Id,
                DateCreated = x.DateCreated,
                NumberOfRating = x.NumberOfRating,
                DateModified = x.DateModified,
                UserNameWithEmail = GetUserAndEmail(x.UserId.ToString())
            }).ToList();

            var paginationSet = new PagedResult<ServiceRatingViewModel>()
            {
                Results = data,
                CurrentPage = page,
                RowCount = totalRow,
                PageSize = pageSize
            };

            return paginationSet;
        }

        private string GetUserAndEmail(string userId)
        {
            var getUser = _userRepository.FindByIdAsync(userId).Result;
            var userMail = getUser.UserName + " (" + getUser.Email + ")";
            return userMail;
        }
    }
}