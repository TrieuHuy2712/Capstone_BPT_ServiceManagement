using BPT_Service.Application.CategoryService.ViewModel;
using BPT_Service.Application.PermissionService.Query.CheckUserIsAdmin;
using BPT_Service.Application.PermissionService.Query.GetPermissionAction;
using BPT_Service.Common;
using BPT_Service.Common.Constants;
using BPT_Service.Common.Helpers;
using BPT_Service.Common.Logging;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace BPT_Service.Application.CategoryService.Command.UpdateCategoryService
{
    public class UpdateCategoryServiceCommand : IUpdateCategoryServiceCommand
    {
        private readonly IRepository<Category, int> _categoryRepository;
        private readonly ICheckUserIsAdminQuery _checkUserIsAdminQuery;
        private readonly IGetPermissionActionQuery _getPermissionActionQuery;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<AppUser> _userManager;

        public UpdateCategoryServiceCommand(IRepository<Category, int> categoryRepository,
            ICheckUserIsAdminQuery checkUserIsAdminQuery,
            IGetPermissionActionQuery getPermissionActionQuery,
            IHttpContextAccessor httpContextAccessor,
            UserManager<AppUser> userManager)
        {
            _categoryRepository = categoryRepository;
            _checkUserIsAdminQuery = checkUserIsAdminQuery;
            _getPermissionActionQuery = getPermissionActionQuery;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }

        public async Task<CommandResult<CategoryServiceViewModel>> ExecuteAsync(CategoryServiceViewModel userVm)
        {
            var userId = _httpContextAccessor.HttpContext.User.Identity.Name;
            var userName = _userManager.FindByIdAsync(userId).Result.UserName;
            try
            {
                //Check user has permission first
                if (await _checkUserIsAdminQuery.ExecuteAsync(userId) || await _getPermissionActionQuery.ExecuteAsync(userId, ConstantFunctions.CATEGORY, ActionSetting.CanUpdate))
                {
                    var categoryUpdate = await _categoryRepository.FindByIdAsync(userVm.Id);
                    if (categoryUpdate != null)
                    {
                        categoryUpdate.CategoryName = userVm.CategoryName;
                        categoryUpdate.Description = userVm.Description;
                        categoryUpdate.ImgPath = userVm.ImgPath;
                        _categoryRepository.Update(categoryUpdate);
                        await _categoryRepository.SaveAsync();
                        await Logging<UpdateCategoryServiceCommand>.
                            InformationAsync(ActionCommand.COMMAND_UPDATE, userName, JsonConvert.SerializeObject(userVm));
                        return new CommandResult<CategoryServiceViewModel>
                        {
                            isValid = true,
                            myModel = userVm,
                        };
                    }
                    else
                    {
                        await Logging<UpdateCategoryServiceCommand>.
                          WarningAsync(ActionCommand.COMMAND_UPDATE, userName, ErrorMessageConstant.ERROR_CANNOT_FIND_ID);
                        return new CommandResult<CategoryServiceViewModel>
                        {
                            isValid = false,
                            myModel = userVm,
                            errorMessage = ErrorMessageConstant.ERROR_CANNOT_FIND_ID
                        };
                    }
                }
                else
                {
                    await Logging<UpdateCategoryServiceCommand>.
                          WarningAsync(ActionCommand.COMMAND_UPDATE, userName, ErrorMessageConstant.ERROR_UPDATE_PERMISSION);
                    return new CommandResult<CategoryServiceViewModel>
                    {
                        isValid = false,
                        errorMessage = ErrorMessageConstant.ERROR_UPDATE_PERMISSION
                    };
                }
            }
            catch (Exception ex)
            {
                await Logging<UpdateCategoryServiceCommand>.
                         ErrorAsync(ex, ActionCommand.COMMAND_UPDATE, userName, "Has error: ");
                return new CommandResult<CategoryServiceViewModel>
                {
                    isValid = false,
                    myModel = userVm,
                    errorMessage = ex.InnerException.ToString()
                };
            }
        }
    }
}