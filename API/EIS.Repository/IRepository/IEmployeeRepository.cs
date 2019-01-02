using EIS.Entities.Employee;
using System.Collections.Generic;

namespace EIS.Repositories.IRepository
{
    public interface IEmployeeRepository : IRepositorybase<Person>
    {
        IEnumerable<Role> GetDesignations();
        Role GetDesignationById(int id);
        void AddDesignationAndSave(Role designation);
        bool DesignationExists(string DesignationName);
        void UpdateDesignationAndSave(Role designation);
    }
}
