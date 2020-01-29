using System.Threading.Tasks;
using BPT_Service.Application.RoleService.ViewModel;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace BPT_Service.Application.RoleService.Command.AddRoleAsync
{
    public class AddRoleAsyncCommand : IAddRoleAsyncCommand
    {
        private readonly RoleManager<AppRole> _roleManager;
        private readonly IUnitOfWork _unitOfWork;
        public AddRoleAsyncCommand(RoleManager<AppRole> roleManager,
        IUnitOfWork unitOfWork)
        {
            _roleManager = roleManager;
            _unitOfWork = unitOfWork;
        }
        public async Task<CommandResult<AppRoleViewModel>> ExecuteAync(AppRoleViewModel roleVm)
        {
            try
            {
                var role = new AppRole()
                {
                    Name = roleVm.Name,
                    Description = roleVm.Description
                };
                var result = await _roleManager.CreateAsync(role);
                _unitOfWork.Commit();
                return new CommandResult<AppRoleViewModel>
                {
                    isValid = result.Succeeded,
                    myModel = new AppRoleViewModel
                    {
                        Description = role.Description,
                        Name = role.Name,
                        Id = role.Id
                    }
                };
            }
            catch (System.Exception ex)
            {
                return new CommandResult<AppRoleViewModel>
                {
                    isValid = false,
                    errorMessage = ex.InnerException.ToString()
                };
            }
        }
    }
}