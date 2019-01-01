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

        public void AddDesignation(Role designation)
        {
            _dbContext.Roles.Add(designation);
        }

        public bool DesignationExists(string DesignationName)
        {
            var role = _dbContext.Roles.Where(x => x.Name == DesignationName).FirstOrDefault();
            if (role != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public Role GetDesignationById(int id)
        {
            return _dbContext.Roles.Where(x => x.Id == id).FirstOrDefault();
        }

        public IEnumerable<Role> GetDesignations()
        {
            return _dbContext.Roles;
        }

        public void UpdateDesignation(Role designation)
        {
            _dbContext.Roles.Update(designation);
        }
    }
}
