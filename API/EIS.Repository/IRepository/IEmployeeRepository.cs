using EIS.Entities.Employee;
using EIS.Entities.Generic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace EIS.Repositories.IRepository
{
    public interface IEmployeeRepository : IRepositorybase<Person>
    {
        int GenerateNewIdCardNo(int TenantId);
        IEnumerable<Role> GetDesignations(int TenantId);
        Role GetDesignationById(int id);
        void AddDesignationAndSave(Role designation);
        bool DesignationExists(string DesignationName,int TenantId);
        void UpdateDesignationAndSave(Role designation);
        Person GetProfile(string EmployeeCode);
        Person ActivatePerson(string EmployeeCode);
        string GetEmployeeCode(int PersonId);
    }
}
