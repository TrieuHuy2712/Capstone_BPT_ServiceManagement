using BPT_Service.Application.LocationService.ViewModel;
using BPT_Service.Application.PermissionService.Query.CheckUserIsAdmin;
using BPT_Service.Application.PermissionService.Query.GetPermissionAction;
using BPT_Service.Common;
using BPT_Service.Common.Helpers;
using BPT_Service.Common.Logging;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BPT_Service.Application.LocationService.Command.AddCityProvinceService
{
    public class AddCityProvinceServiceCommand : IAddCityProvinceServiceCommand
    {
        private readonly IRepository<CityProvince, int> _cityProvinceRepository;
        private readonly ICheckUserIsAdminQuery _checkUserIsAdminQuery;
        private readonly IGetPermissionActionQuery _getPermissionActionQuery;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<AppUser> _userManager;

        public AddCityProvinceServiceCommand(IRepository<CityProvince, int> cityProvinceRepository,
            ICheckUserIsAdminQuery checkUserIsAdminQuery,
            IGetPermissionActionQuery getPermissionActionQuery,
            IHttpContextAccessor httpContextAccessor,
            UserManager<AppUser> userManager)
        {
            _cityProvinceRepository = cityProvinceRepository;
            _checkUserIsAdminQuery = checkUserIsAdminQuery;
            _getPermissionActionQuery = getPermissionActionQuery;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }

        public async Task<CommandResult<CityProvinceViewModel>> ExecuteAsync(CityProvinceViewModel listAddViewModel)
        {
            var userId = _httpContextAccessor.HttpContext.User.Identity.Name;
            var userName = _userManager.FindByIdAsync(userId).Result.UserName;
            try
            {//Check user has permission first
                if (await _checkUserIsAdminQuery.ExecuteAsync(userId) || await _getPermissionActionQuery.ExecuteAsync(userId, "LOCATION", ActionSetting.CanCreate))
                {
                    //Check User Has Permission Create
                    var listModel = MappingCityProvince(listAddViewModel);
                    await _cityProvinceRepository.Add(listModel);
                    await _cityProvinceRepository.SaveAsync();
                    await Logging<AddCityProvinceServiceCommand>.InformationAsync(ActionCommand.COMMAND_ADD, userName, JsonConvert.SerializeObject(listModel));
                    return new CommandResult<CityProvinceViewModel>
                    {
                        isValid = true,
                        myModel = new CityProvinceViewModel()
                        {
                            Id = listModel.Id,
                            City = listModel.City,
                            Province = listModel.Province,
                            ImagePath= listModel.ImgPath
                        }
                    };
                }
                else
                {
                    await Logging<AddCityProvinceServiceCommand>.WarningAsync(ActionCommand.COMMAND_ADD, userName, ErrorMessageConstant.ERROR_ADD_PERMISSION);
                    return new CommandResult<CityProvinceViewModel>
                    {
                        isValid = false,
                        errorMessage = ErrorMessageConstant.ERROR_ADD_PERMISSION
                    };
                }
            }
            catch (Exception ex)
            {
                await Logging<AddCityProvinceServiceCommand>.ErrorAsync(ex, ActionCommand.COMMAND_ADD, userName, "Has error");
                return new CommandResult<CityProvinceViewModel>
                {
                    isValid = false,
                    errorMessage = ex.InnerException.Message.ToString()
                };
            }
        }

        private CityProvince MappingCityProvince(CityProvinceViewModel listAddViewModel)
        {
            return new CityProvince()
            {
                Id = listAddViewModel.Id,
                City = listAddViewModel.City,
                ImgPath = listAddViewModel.ImagePath,
                Province = listAddViewModel.Province
            };
        }
    }
}