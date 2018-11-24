using EIS.Entities.User;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EIS.Repositories.IRepository
{
    public interface IRoleRepository : IRepositorybase<Role>
    {
        void CreateRole(string RoleName);
        void UpdateRole(int id,Role role);
        void MapRole(int UserId, string RoleId);
        string GetRole(Users user);
        bool RoleExists(string RoleName);
        Task CreateRoleAsync(string RoleName);
        Task<bool> RoleExistsAsync(string RoleName);
    }
}
