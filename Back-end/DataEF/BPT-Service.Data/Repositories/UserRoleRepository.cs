using BPT_Service.Model.IRepositories;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Data;

namespace BPT_Service.Data.Repositories
{
    public class UserRoleRepository : IUserRoleRepository
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public UserRoleRepository(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public void DeleteUserRole(Guid userId, Guid roleId)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("DeleteUserRoles", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("UserIdDelete", userId));
                    cmd.Parameters.Add(new SqlParameter("RoleIdDelete", roleId));
                    cmd.Connection.Open();
                    var result = cmd.ExecuteNonQuery();
                    cmd.Connection.Close();
                }
            }
        }
    }
}