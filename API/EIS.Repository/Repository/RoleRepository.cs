using EIS.Data.Context;
using EIS.Entities.User;
using EIS.Repositories.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EIS.Repositories.Repository
{
    public class RoleRepository : RepositoryBase<Role>, IRoleRepository
    {
        ApplicationDbContext dbContext;
        public RoleRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            this.dbContext = dbContext;
        }

        public void CreateRole(string RoleName)
        {
            Role role = new Role();
            role.Name = RoleName;
            role.IsActive = true;
            role.CreatedDate = DateTime.Now;
            role.UpdatedDate = DateTime.Now;
            dbContext.Roles.Add(role);
        }

        public async Task CreateRoleAsync(string RoleName)
        {
            Role role = new Role();
            role.Name = RoleName;
            role.IsActive = true;
            role.CreatedDate = DateTime.Now;
            role.UpdatedDate = DateTime.Now;
            await dbContext.Roles.AddAsync(role);
        }

        public string GetRole(Users user)
        {
            string role=null;
            role = dbContext.Roles.Where(x => x.Id == (dbContext.UserRoles.Where(u => u.Id == user.Id).Select(u => u.RoleId).FirstOrDefault())).Select(x => x.Name).FirstOrDefault();
            if (role != null)
            {
                return role;
            }
            return role;
        }

        public void MapRole(int UserId, int RoleId)
        {
            UserRoles u = new UserRoles();
            u.UserId = UserId;
            u.RoleId = RoleId;
            u.IsActive = true;
            u.CreatedDate = DateTime.Now;
            u.UpdatedDate = DateTime.Now;
            dbContext.UserRoles.Add(u);
        }

        public bool RoleExists(string RoleName)
        {
            var role = dbContext.Roles.Where(x => x.Name == RoleName).FirstOrDefault();
            if (role != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> RoleExistsAsync(string RoleName)
        {
            var id = dbContext.Roles.Where(x => x.Name == RoleName).Select(x => x.Id).FirstOrDefault();
            if (id != 0)
            {
                var role =await dbContext.Roles.FindAsync(id);
                if (role != null)
                {
                    return true;
                }
            }
            return false;
            
        }

        public void UpdateRole(int id, Role role)
        {
            throw new NotImplementedException();
        }
    }
}
