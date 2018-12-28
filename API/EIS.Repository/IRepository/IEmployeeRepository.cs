using EIS.Entities.Employee;
using System;
using System.Collections.Generic;
using System.Text;

namespace EIS.Repositories.IRepository
{
    public interface IEmployeeRepository : IRepositorybase<Person>
    {
        IEnumerable<Designation> GetDesignations();
        Designation GetDesignationById(int id);
        void AddDesignation(Designation designation);
        bool DesignationExists(string DesignationName);
        void UpdateDesignation(Designation designation);
    }
}
