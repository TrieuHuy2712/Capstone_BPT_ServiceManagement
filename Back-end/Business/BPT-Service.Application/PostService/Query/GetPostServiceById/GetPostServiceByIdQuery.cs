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

        public GetPostServiceByIdQuery(
            IRepository<Service, Guid> serviceRepository, 
            UserManager<AppUser> userManager, 
            IHttpContextAccessor httpContext, 
            IRepository<Tag, Guid> tagRepository, 
            IRepository<Model.Entities.ServiceModel.TagService, int> tagServiceRepository, 
            IRepository<ServiceImage, int> imageRepository)
        {
            _serviceRepository = serviceRepository;
            _userManager = userManager;
            _httpContext = httpContext;
            _tagRepository = tagRepository;
            _tagServiceRepository = tagServiceRepository;
            _imageRepository = imageRepository;
        }

        public async Task<CommandResult<PostServiceViewModel>> ExecuteAsync(string idService)
        {
            try
            {
                var service = await _serviceRepository.FindByIdAsync(Guid.Parse(idService));
                if (service != null && service.Status== Model.Enums.Status.Active)
                {
                    return new  CommandResult<PostServiceViewModel>
                    {
                        isValid = true,
                        myModel = MapViewModel(service).Result
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

        private async Task<PostServiceViewModel> MapViewModel(Service serv)
        {
            var getTag = await _tagRepository.FindAllAsync();
            var getUserTag = await _tagServiceRepository.FindAllAsync(x => x.ServiceId == serv.Id);
            var getListTag = (from tag in getTag.ToList()
                              join serviceTag in getUserTag.ToList()
                              on tag.Id equals serviceTag.TagId
                              select new
                              {
                                  tag.TagName
                              }).ToList();

            var getImage = await _imageRepository.FindAllAsync(x=>x.ServiceId==serv.Id);
            PostServiceViewModel postServiceView = new PostServiceViewModel();
            postServiceView.Id = serv.Id.ToString();
            postServiceView.listImages = getImage.Select(x => new PostServiceImageViewModel
            {
                Path = x.Path
            }).ToList();
            postServiceView.PriceOfService = serv.PriceOfService;
            postServiceView.ServiceName = serv.ServiceName;
            postServiceView.Status = serv.Status;
            postServiceView.tagofServices = getListTag.Select(x => new TagofServiceViewModel
            {
                TagName = x.TagName
            }).ToList();
            postServiceView.CategoryId = serv.CategoryId;
            return postServiceView;
        }
    }
}