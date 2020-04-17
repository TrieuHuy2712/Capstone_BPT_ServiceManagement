using System;

namespace BPT_Service.Model.IRepositories
{
    public interface IUserRoleRepository
    {
        void DeleteUserRole(Guid userId, Guid roleId);
    }
}