using EIS.Entities.Employee;
using System;
using System.Collections.Generic;
using System.Text;

namespace EIS.Repositories.IRepository
{
    public interface IEmployeeRepository : IRepositorybase<Person>
    {
        IEnumerable<Role> GetDesignations();
        Role GetDesignationById(int id);
        void AddDesignation(Role designation);
        bool DesignationExists(string DesignationName);
        void UpdateDesignation(Role designation);
    }
}
