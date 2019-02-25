using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using EIS.Data.Context;
using EIS.Entities.Employee;
using EIS.Entities.SP;
using EIS.Repositories.IRepository;
using Microsoft.EntityFrameworkCore;

namespace EIS.Repositories.Repository
{
    public class EmployeeRepository :RepositoryBase<Person>,IEmployeeRepository
    {
        public EmployeeRepository(ApplicationDbContext dbContext) : base(dbContext)
        {

        }

        public Person ActivatePerson(string EmployeeCode)
        {
            Person person = _dbContext.Person.Include(x => x.User).Where(x => x.EmployeeCode == EmployeeCode).FirstOrDefault();
            person.IsActive = true;
            person.User.IsActive = true;
            Save();
            return (person);
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
            var MaxId = _dbContext.Person.Where(x=>x.TenantId==TenantId).Max(x => x.EmployeeCode);
            return Convert.ToInt32(MaxId) + 1;
        }

        public List<GetAdminHrManager> getAdminHrManager()
        {
            return _dbContext._sp_GetAdminHrManager.FromSql("LMS.usp_GetAdminHRManager").ToList();
        }

        public Role GetDesignationById(int id)
        {
            return _dbContext.Roles.Where(x => x.Id == id).FirstOrDefault();
        }

        public IEnumerable<Role> GetDesignations(int TenantId)
        {
            return _dbContext.Roles.AsNoTracking().Where(x => x.TenantId == TenantId);
        }

        public string GetEmployeeCode(int PersonId)
        {
            string code = null;
            Person emp = _dbContext.Person.Where(x => x.Id == PersonId).FirstOrDefault();
            if (emp != null)
            {
                code = emp.EmployeeCode;
            }
            return code;
        }

        public List<SP_GetEmployee> getEmployees(int location)
        {
            List<SP_GetEmployee> _GetEmployee = new List<SP_GetEmployee>();
            var LId= new SqlParameter("@LId", location);
            string usp = "LMS.usp_GetEmployeeDetails @LId";
            _GetEmployee = _dbContext._sp_GetEmployee.FromSql(usp,LId).ToList();
            return _GetEmployee;
        }

        public Person GetProfile(string EmployeeCode)
        {
            return _dbContext.Person.Include(x => x.PermanentAddress).Include(x => x.CurrentAddress).Include(x => x.EmergencyAddress).Include(x=>x.OtherAddress).Include(x=>x.Role).Include(x=>x.Attendance).Where(x => x.EmployeeCode == EmployeeCode).FirstOrDefault();
        }

        public void UpdateDesignationAndSave(Role designation)
        {
            _dbContext.Roles.Update(designation);
            Save();
        }
    }
}
