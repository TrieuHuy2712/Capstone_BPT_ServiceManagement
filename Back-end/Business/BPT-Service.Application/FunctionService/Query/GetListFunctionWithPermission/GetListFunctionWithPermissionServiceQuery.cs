using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BPT_Service.Application.FunctionService.ViewModel;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BPT_Service.Application.FunctionService.Query.GetListFunctionWithPermission
{
    public class GetListFunctionWithPermissionServiceQuery : IGetListFunctionWithPermissionQuery
    {
        private readonly IRepository<Function, string> _functionRepository;
        private readonly IRepository<Permission, int> _permissionRepository;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;

        public GetListFunctionWithPermissionServiceQuery(
            IRepository<Function, string> functionRepository,
            IRepository<Permission, int> permissionRepository,
            RoleManager<AppRole> roleManager,
            IUnitOfWork unitOfWork,
            UserManager<AppUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _functionRepository = functionRepository;
            _permissionRepository = permissionRepository;
            _roleManager = roleManager;
            _userManager = userManager;
        }
        public async Task<List<FunctionViewModelinFunctionService>> ExecuteAsync(string userName)
        {
            var listPermission = await _permissionRepository.FindAllAsync();
            var listRole = _roleManager.Roles.Where(x => x.Name != "admin").ToList();
            var listFunction = await _functionRepository.FindAllAsync();
            var getUser = await _userManager.FindByNameAsync(userName);
            var role = await _userManager.GetRolesAsync(getUser);
            List<AppRoleViewModelinFunctionService> listRoleUser = new List<AppRoleViewModelinFunctionService>();
            if (role.Count > 0)
            {
                foreach (var item in role)
                {
                    var roleId = await _roleManager.Roles.Select(x => new AppRoleViewModelinFunctionService
                    {
                        Id = x.Id,
                        Description = x.Description,
                        Name = x.Name
                    }).Where(x => x.Name == item).FirstOrDefaultAsync();
                    listRoleUser.Add(roleId);
                }
            }

            List<FunctionViewModelinFunctionService> functions = new List<FunctionViewModelinFunctionService>();
            List<FunctionViewModelinFunctionService> newFunctions = new List<FunctionViewModelinFunctionService>();
            foreach (var item in listRoleUser)
            {
                var getListFunction = (from f in listFunction
                                       join p in listPermission on f.Id equals p.FunctionId
                                       where p.RoleId == item.Id && p.CanRead == true
                                       select new FunctionViewModelinFunctionService
                                       {
                                           Id = f.Id,
                                           IconCss = f.IconCss,
                                           Name = f.Name,
                                           ParentId = f.ParentId,
                                           SortOrder = f.SortOrder,
                                           Status = f.Status,
                                           URL = f.URL,
                                       }).ToList();

                functions.AddRange(getListFunction);
            }
            var query = functions.GroupBy(x => new { x.Id, x.Name, x.ParentId, x.SortOrder, x.Status }).Where(x => x.Skip(1).Any()).ToArray();

            foreach (var item in query)
            {
                var findItem = functions.Where(x => x.Name == item.Key.Name && x.Id == item.Key.Id).FirstOrDefault();
                newFunctions.Add(findItem);
            }
            return newFunctions;

        }
    }
}