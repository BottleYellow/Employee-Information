﻿using System;
using System.Collections.Generic;
using System.Linq;
using EIS.Data.Context;
using EIS.Entities.Employee;
using EIS.Entities.Leave;
using EIS.Repositories.IRepository;
using Microsoft.EntityFrameworkCore;

namespace EIS.Repositories.Repository
{
    public class LeaveCreditRepository: RepositoryBase<LeaveCredit>, ILeaveCreditRepository
    {
        public LeaveCreditRepository(ApplicationDbContext dbContext): base(dbContext)
        {

        }

        public void AddCreditAndSave(LeaveCredit Credit)
        {
            _dbContext.LeaveCredit.Add(Credit);
            Save();
        }

        public void AddCreditsAndSave(LeaveRulesWithEmp Leave)
        {
            int id = Leave.Id;
            List<Person> List = _dbContext.Person.Where(x => x.TenantId == Leave.TenantId && x.LocationId==Leave.LocationId).ToList();
            foreach (var item in Leave.Employees)
            {
                LeaveCredit Credit = new LeaveCredit
                {
                    TenantId = 0,
                    PersonId = item,
                    LeaveType = Leave.LeaveType,
                    AllotedDays = Leave.Validity,
                    Available = Leave.Validity,
                    ValidFrom = Leave.ValidFrom,
                    ValidTo = Leave.ValidTo,
                    LeaveId = Leave.Id,
                    CreatedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now,
                    IsActive = true
                };
                AddCreditAndSave(Credit);
            }
        }
        public float GetAvailableLeaves(int PersonId, int LeaveId)
        {
            float n;
            bool isPaid = _dbContext.LeaveCredit.Include(x => x.LeaveRule).Where(x => x.Id == LeaveId).FirstOrDefault().LeaveRule.IsPaid;
            if (isPaid == true)
            {
                n = _dbContext.LeaveCredit.Where(x => x.PersonId == PersonId && x.Id == LeaveId).Select(x => x.Available).FirstOrDefault();
            }
            else
            {
                n = -2;
            }
            return n;
        }

    }
}
