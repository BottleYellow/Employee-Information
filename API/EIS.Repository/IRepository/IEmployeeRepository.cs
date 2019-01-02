using EIS.Entities.Employee;
using System;
using System.Collections.Generic;
using System.Text;

namespace EIS.Repositories.IRepository
{
    public interface IEmployeeRepository : IRepositorybase<Person>
    {
<<<<<<< HEAD
        IEnumerable<Designation> GetDesignations();
        Designation GetDesignationById(int id);
        bool DesignationExists(string DesignationName);
        void UpdateDesignationAndSave(Designation designation);

        void AddDesignationAndSave(Designation designation);
=======
        IEnumerable<Role> GetDesignations();
        Role GetDesignationById(int id);
        void AddDesignation(Role designation);
        bool DesignationExists(string DesignationName);
        void UpdateDesignation(Role designation);
>>>>>>> eab0133b5e8f6e86eb09bb18611280e9b8dcee1c
    }
}
