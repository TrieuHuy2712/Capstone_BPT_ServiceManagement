using BPT_Service.Model.Entities;
using BPT_Service.Model.IRepositories;

namespace BPT_Service.Data.Repositories
{
    public class FunctionRepository : EFRepository<Function, string>, IFunctionRepository
    {
        public FunctionRepository(AppDbContext context) : base(context)
        {
        }
    }
}