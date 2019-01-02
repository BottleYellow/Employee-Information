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

<<<<<<< HEAD
        public void AddDesignationAndSave(Designation designation)
        {
            AddDesignation(designation);
            Save();
        }

        public void UpdateDesignationAndSave(Designation designation)
        {
            UpdateDesignation(designation);
            Save();
        }
        public void AddDesignation(Designation designation)
        {
            _dbContext.DesignationMaster.Add(designation);
=======
        public void AddDesignation(Role designation)
        {
            _dbContext.Roles.Add(designation);
>>>>>>> eab0133b5e8f6e86eb09bb18611280e9b8dcee1c
        }

        public bool DesignationExists(string DesignationName)
        {
<<<<<<< HEAD
            var role = _dbContext.DesignationMaster.Where(x => x.Name == DesignationName).FirstOrDefault();           
=======
            var role = _dbContext.Roles.Where(x => x.Name == DesignationName).FirstOrDefault();
>>>>>>> eab0133b5e8f6e86eb09bb18611280e9b8dcee1c
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
<<<<<<< HEAD
            return _dbContext.DesignationMaster.Where(x => x.Id == id).FirstOrDefault();
=======
            return _dbContext.Roles.Where(x => x.Id == id).FirstOrDefault();
>>>>>>> eab0133b5e8f6e86eb09bb18611280e9b8dcee1c
        }

        public IEnumerable<Role> GetDesignations()
        {
<<<<<<< HEAD
            return _dbContext.DesignationMaster;
=======
            return _dbContext.Roles;
>>>>>>> eab0133b5e8f6e86eb09bb18611280e9b8dcee1c
        }

        public void UpdateDesignation(Role designation)
        {
<<<<<<< HEAD
            _dbContext.DesignationMaster.Update(designation);
=======
            _dbContext.Roles.Update(designation);
>>>>>>> eab0133b5e8f6e86eb09bb18611280e9b8dcee1c
        }
    }
}
