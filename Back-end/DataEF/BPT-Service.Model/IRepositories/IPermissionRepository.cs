using BPT_Service.Model.Entities;
using BPT_Service.Model.Infrastructure.Interfaces;
using System.Collections.Generic;

namespace BPT_Service.Model.IRepositories
{
    public interface IPermissionRepository : IRepository<Permission, int>
    {
    }
    
}