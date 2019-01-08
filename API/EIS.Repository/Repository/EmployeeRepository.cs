using System;
using System.Collections.Generic;
using System.Linq;
using EIS.Data.Context;
using EIS.Entities.Employee;
using EIS.Repositories.IRepository;

namespace EIS.Repositories.Repository
{
    public class EmployeeRepository :RepositoryBase<Person>,IEmployeeRepository
    {
        public EmployeeRepository(ApplicationDbContext dbContext) : base(dbContext)
        {

        }
        public void AddDesignationAndSave(Role designation)
        {
            _dbContext.Roles.Add(designation);
            Save();
        }

        public bool DesignationExists(string DesignationName,int TenantId)
        {
            var role = _dbContext.Roles.Where(x => x.Name == DesignationName && x.TenantId==TenantId).FirstOrDefault();
            if (role != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public int GenerateNewIdCardNo(int TenantId)
        {
            var MaxId = _dbContext.Person.Where(x=>x.TenantId==TenantId).Max(x => x.IdCard);
            return Convert.ToInt32(MaxId) + 1;
        }

        public Role GetDesignationById(int id)
        {
            return _dbContext.Roles.Where(x => x.Id == id).FirstOrDefault();
        }

        public IEnumerable<Role> GetDesignations(int TenantId)
        {
            return _dbContext.Roles.Where(x => x.TenantId == TenantId);
        }

        public void UpdateDesignationAndSave(Role designation)
        {
            _dbContext.Roles.Update(designation);
            Save();
        }
    }
}
