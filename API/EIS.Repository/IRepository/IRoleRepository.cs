using EIS.Entities.User;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EIS.Repositories.IRepository
{
    public interface IRoleRepository : IRepositorybase<Role>
    {
        void CreateRole(Role role);
        void UpdateRole(int id,Role role);
        void MapRole(int UserId, int RoleId);
        string GetRole(int UserId);
        bool RoleExists(string RoleName);
        Task CreateRoleAsync(Role role);
        Task<bool> RoleExistsAsync(string RoleName);
    }
}
