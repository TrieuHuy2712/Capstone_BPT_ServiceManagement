using System.Threading.Tasks;
using BPT_Service.Application.FunctionService.ViewModel;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Infrastructure.Interfaces;

namespace BPT_Service.Application.FunctionService.Command.AddFunctionService
{
    public class AddFunctionServiceCommand : IAddFunctionServiceCommand
    {
        private readonly IRepository<Function, string> _functionRepository;

        public AddFunctionServiceCommand(
            IRepository<Function, string> functionRepository)
        {
            _functionRepository = functionRepository;
        }
        public async Task<bool> ExecuteAsync(FunctionViewModelinFunctionService function)
        {
            Function newfunction = new Function();
            newfunction.Id = function.Id;
            newfunction.IconCss = function.IconCss;
            newfunction.Name = function.Name;
            newfunction.ParentId = function.ParentId;
            newfunction.SortOrder = function.SortOrder;
            newfunction.Status = function.Status;
            newfunction.URL = function.URL;
            newfunction.NameVietNamese = function.NameVietNamese;
            _functionRepository.Add(newfunction);
            return true;
        }
    }
}