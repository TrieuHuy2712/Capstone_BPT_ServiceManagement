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
using System.Security.Claims;
using System.Threading.Tasks;

namespace BPT_Service.Application.CategoryService.Command.AddCategoryService
{
    public class AddCategoryServiceCommand : IAddCategoryServiceCommand
    {
        private readonly IRepository<Category, int> _categoryRepository;
        private readonly ICheckUserIsAdminQuery _checkUserIsAdminQuery;
        private readonly IGetPermissionActionQuery _getPermissionActionQuery;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<AppUser> _userManager;

        public AddCategoryServiceCommand(IRepository<Category, int> categoryRepository,
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
                if (await _checkUserIsAdminQuery.ExecuteAsync(userId) || await _getPermissionActionQuery.ExecuteAsync(userId, ConstantFunctions.CATEGORY, ActionSetting.CanCreate))
                {
                    var mappingCate = mappingCategory(userVm);
                    await _categoryRepository.Add(mappingCate);
                    await _categoryRepository.SaveAsync();
                    var createReturn = new CommandResult<CategoryServiceViewModel>
                    {
                        isValid = true,
                        myModel = new CategoryServiceViewModel
                        {
                            Id = mappingCate.Id,
                            CategoryName = mappingCate.CategoryName,
                            Description = mappingCate.Description,
                            ImgPath = mappingCate.ImgPath
                        },
                    };
                    await Logging<AddCategoryServiceCommand>.
                        InformationAsync(ActionCommand.COMMAND_ADD, userName, JsonConvert.SerializeObject(createReturn.myModel));
                    return createReturn;
                }
                else
                {
                    await Logging<AddCategoryServiceCommand>.WarningAsync(ActionCommand.COMMAND_ADD,userName, ErrorMessageConstant.ERROR_ADD_PERMISSION);
                    return new CommandResult<CategoryServiceViewModel>
                    {
                        isValid = false,
                        errorMessage = ErrorMessageConstant.ERROR_ADD_PERMISSION
                    };
                }
            }
            catch (System.Exception ex)
            {
                await Logging<AddCategoryServiceCommand>.ErrorAsync(ex, ActionCommand.COMMAND_ADD, userName, "You have error");
                return new CommandResult<CategoryServiceViewModel>
                {
                    isValid = false,
                    myModel = userVm,
                    errorMessage = ex.InnerException.ToString()
                };
            }
        }

        private Category mappingCategory(CategoryServiceViewModel userVm)
        {
            Category category = new Category();
            category.Id = userVm.Id;
            category.CategoryName = userVm.CategoryName;
            category.Description = userVm.Description;
            category.ImgPath = userVm.ImgPath;
            return category;
        }
    }
}