using EIS.Entities.Employee;
using EIS.Entities.Hoildays;
using EIS.Entities.SP;
using System.Collections.Generic;

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
        List<GetAdminHrManager> getAdminHrManager();
        List<SP_GetEmployee> getEmployees(int location);
        List<WeeklyOffs> GetWeeklyOffs();
        string GetWeeklyOffByPerson(int PersonId);
        void UpdateProbation(int PersonId, int ProbationPeriod);
    }
}
