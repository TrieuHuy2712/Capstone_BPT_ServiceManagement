using BPT_Service.Model.Entities;
using BPT_Service.Model.IRepositories;

namespace BPT_Service.Data.Repositories
{
    public class PermissionRepository : EFRepository<Permission, int>, IPermissionRepository
    {
        public PermissionRepository(AppDbContext context) : base(context)
        {
        }
    }
}