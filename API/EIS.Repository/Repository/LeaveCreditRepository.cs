﻿using System;
using System.Collections.Generic;
using System.Linq;
using EIS.Data.Context;
using EIS.Entities.Employee;
using EIS.Entities.Leave;
using EIS.Repositories.IRepository;

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

        public void AddCreditsAndSave(LeaveRules Leave)
        {
            int id = Leave.Id;
            List<Person> List = _dbContext.Person.ToList();
            foreach (var item in List)
            {
                LeaveCredit Credit = new LeaveCredit
                {
                    TenantId = item.TenantId,
                    PersonId = item.Id,
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
            float n = _dbContext.LeaveCredit.Where(x => x.PersonId == PersonId && x.LeaveId == LeaveId).Select(x => x.Available).FirstOrDefault();
            return n;
        }

        public IQueryable<LeaveCredit> GetCredits()
        {
            var results = _dbContext.LeaveCredit
                .Select(l => new
                {
                    l,
                    person = l.Person
                });
            foreach (var x in results)
            {
                x.l.Person = x.person;
            }
            var result = results.Select(x => x.l);
            return result;
        }

    }
}
