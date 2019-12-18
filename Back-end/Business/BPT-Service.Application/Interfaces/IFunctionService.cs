using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BPT_Service.Application.ViewModels.System;

namespace BPT_Service.Application.Interfaces
{
    public interface IFunctionService : IDisposable
    {
        void Add(FunctionViewModel function);

        Task<List<FunctionViewModel>> GetAll(string filter);

        IEnumerable<FunctionViewModel> GetAllWithParentId(string parentId);

        FunctionViewModel GetById(string id);

        Task<List<FunctionViewModel>> GetListFunctionWithPermission(string userName);

        void Update(FunctionViewModel function);

        void Delete(string id);

        void Save();

        bool CheckExistedId(string id);

        void UpdateParentId(string sourceId, string targetId, Dictionary<string, int> items);

        void ReOrder(string sourceId, string targetId);
    }
}