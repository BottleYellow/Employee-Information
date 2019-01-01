﻿using System.Collections.Generic;
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

        public void AddDesignation(Designation designation)
        {
            _dbContext.DesignationMaster.Add(designation);
        }

        public bool DesignationExists(string DesignationName)
        {
            var role = _dbContext.DesignationMaster.Where(x => x.Name == DesignationName).FirstOrDefault();
            if (role != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public Designation GetDesignationById(int id)
        {
            return _dbContext.DesignationMaster.Where(x => x.Id == id).FirstOrDefault();
        }

        public IEnumerable<Designation> GetDesignations()
        {
            return _dbContext.DesignationMaster;
        }

        public void UpdateDesignation(Designation designation)
        {
            _dbContext.DesignationMaster.Update(designation);
        }
    }
}
