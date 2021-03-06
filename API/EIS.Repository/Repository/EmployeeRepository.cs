﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using EIS.Data.Context;
using EIS.Entities.Employee;
using EIS.Entities.Hoildays;
using EIS.Entities.SP;
using EIS.Repositories.IRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Protocols;

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
            person.UpdatedDate = DateTime.Now;
            person.User.UpdatedDate = DateTime.Now;
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
        public List<WeeklyOffs> GetWeeklyOffs()
        {
            return _dbContext.WeeklyOffs.Where(x=>x.IsActive==true).ToList();
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
            return _dbContext.Person.Include(x=>x.WeeklyOff).Include(x => x.PermanentAddress).Include(x => x.CurrentAddress).Include(x => x.EmergencyAddress).Include(x=>x.OtherAddress).Include(x=>x.Role).Include(x=>x.Attendance).Where(x => x.EmployeeCode == EmployeeCode).FirstOrDefault();
        }

        public void UpdateDesignationAndSave(Role designation)
        {
            _dbContext.Roles.Update(designation);
            Save();
        }

        public string GetWeeklyOffByPerson(int PersonId)
        {
            Person person = _dbContext.Person.Include(x => x.WeeklyOff).Where(x => x.Id == PersonId).FirstOrDefault();
            string type = "NoSaturday";
            if (person.WeeklyOff != null)
            {
                type = person.WeeklyOff.Type;
            }
            return type;
        }

        public void UpdateProbation(int PersonId, int ProbationPeriod)
        {
            string usp = "LMS.usp_UpdateProbationPeriod @PersonId,@ProbationPeriod";
            var param = new SqlParameter("@PersonId", PersonId);
            var param2 = new SqlParameter("@ProbationPeriod", ProbationPeriod);
            _dbContext.Database.ExecuteSqlCommand(usp, param, param2);
            _dbContext.SaveChanges();
        }
        public TestModel TestData(int page, int pageSize, string filterValue)
        {
            List<Person> people = new List<Person>();
            TestModel testModel = new TestModel();
            int test = pageSize * page;
            int countValue = 0;
            Person[] persons = new Person[pageSize];
            if (filterValue == "NULL")
            {
                people = _dbContext.Person.Skip(test).Take(pageSize).ToList();
                countValue = _dbContext.Person.Count();
            }
            else
            {
                countValue = _dbContext.Person.Where(x => x.EmployeeCode.Contains(filterValue) || x.FirstName.ToLower().Contains(filterValue.ToLower())).Count();
                people = _dbContext.Person.Where(x => x.EmployeeCode.Contains(filterValue) || x.FirstName.ToLower().Contains(filterValue.ToLower())).Skip(test).Take(pageSize).ToList();
            }
            int i = 0;
            foreach (var p in people)
            {
                persons[i] = p;
                i++;
            }
            testModel.people = persons;
            testModel.count = countValue;
            return testModel;
        }

        public List<SP_GetEmployee> getAdmins()
        {
            List<SP_GetEmployee> _GetAdmins = new List<SP_GetEmployee>();
            string usp = "LMS.usp_GetAdminDetails";
            _GetAdmins = _dbContext._sp_GetEmployee.FromSql(usp).ToList();
            return _GetAdmins;
        }
    }
}
