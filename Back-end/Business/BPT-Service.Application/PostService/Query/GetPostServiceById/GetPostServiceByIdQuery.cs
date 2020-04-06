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
        private readonly IRepository<Provider, Guid> _providerRepository;
        private readonly UserManager<AppUser> _userManager;
        private readonly IHttpContextAccessor _httpContext;

        public GetPostServiceByIdQuery(
            IRepository<Service, Guid> serviceRepository,
            UserManager<AppUser> userManager,
        IRepository<Provider, Guid> providerRepository,
        IHttpContextAccessor httpContext
        )
        {
            _serviceRepository = serviceRepository;
            _providerRepository = providerRepository;
            _userManager = userManager;
            _httpContext = httpContext;
        }

        public async Task<CommandResult<PostServiceViewModel>> ExecuteAsync(string idService)
        {
            try
            {
                var service = await _serviceRepository.FindByIdAsync(Guid.Parse(idService));
                if (service != null && service.Status== Model.Enums.Status.Active)
                {
                    return new CommandResult<PostServiceViewModel>
                    {
                        isValid = true,
                        myModel = MapViewModel(service)
                    };
                }
                else
                {
                    return new CommandResult<PostServiceViewModel>
                    {
                        isValid = true,
                        errorMessage = ErrorMessageConstant.ERROR_CANNOT_FIND_ID
                    };
                }
            }
            catch (System.Exception ex)
            {
                return new CommandResult<PostServiceViewModel>
                {
                    isValid = true,
                    errorMessage = ex.InnerException.ToString()
                };
            }
        }

        //private async Task<CommandResult<Service>> IsOwnService(Guid idService)
        //{
        //    var checkOwnProvider = await _providerRepository.FindAllAsync(x => x.UserId == Guid.Parse(getCurrentId));
        //    var checkOwnService = await _serviceRepository.FindSingleAsync(x => x.UserServices.ServiceId == idService);
        //    foreach (var item in checkOwnProvider)
        //    {
        //        if (item.Id == checkOwnService.ProviderServices.ProviderId)
        //        {
        //            return new CommandResult<Service>
        //            {
        //                isValid = true,
        //                myModel = checkOwnService
        //            };
        //        }
        //    }

        //    if (checkOwnService.UserServices.UserId == Guid.Parse(getCurrentId))
        //    {
        //        return new CommandResult<Service>
        //        {
        //            isValid = true,
        //            myModel = checkOwnService
        //        };
        //    }

        //    if (checkOwnService == null)
        //    {
        //        return new CommandResult<Service>
        //        {
        //            isValid = false,
        //            errorMessage = "Cannot find service"
        //        };
        //    }
        //    return new CommandResult<Service>
        //    {
        //        isValid = false,
        //        errorMessage = "You don't have permission"
        //    };
        //}

        private PostServiceViewModel MapViewModel(Service serv)
        {
            PostServiceViewModel postServiceView = new PostServiceViewModel();
            postServiceView.Id = serv.Id.ToString();
            postServiceView.listImages = serv.ServiceImages.Select(x => new PostServiceImageViewModel
            {
                Path = x.Path
            }).ToList();
            postServiceView.PriceOfService = serv.PriceOfService;
            postServiceView.ServiceName = serv.ServiceName;
            postServiceView.Status = serv.Status;
            postServiceView.tagofServices = serv.TagServices.Select(x => new TagofServiceViewModel
            {
                TagName = x.Tag.TagName
            }).ToList();
            postServiceView.CategoryId = serv.CategoryId;
            return postServiceView;
        }
    }
}