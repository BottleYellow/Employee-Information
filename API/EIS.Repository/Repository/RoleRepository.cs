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

        public void CreateRole(Role role)
        {
            dbContext.Roles.Add(role);
        }

        public async Task CreateRoleAsync(Role role)
        {
            await dbContext.Roles.AddAsync(role);
        }

        public string GetRole(int UserId)
        {
            //string role=null;
            var role = dbContext.UserRoles.Where(u => u.UserId == UserId).Select(u => u.Role).FirstOrDefault();
            //role = dbContext.Roles.Where(x => x.Id == dbContext.UserRoles.Where(u => u.UserId == UserId).Select(u => u.RoleId).FirstOrDefault()).Select(x => x.Name).FirstOrDefault();
            if (role != null)
            {
                return role.Name;
            }
            return role.Name;
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
