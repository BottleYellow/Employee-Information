using System;
using System.Collections.Generic;
using System.Linq;
using EIS.Data.Context;
using EIS.Entities.Employee;
using EIS.Entities.Leave;
using EIS.Repositories.IRepository;
using Microsoft.EntityFrameworkCore;

namespace EIS.Repositories.Repository
{
   public class LeaveRepository: RepositoryBase<LeaveRequest>, ILeaveRepository
    {
        public LeaveRepository(ApplicationDbContext dbContext): base(dbContext)
        {

        }

        public void AddCredit(LeaveCredit Credit)
        {
            _dbContext.LeaveCredit.Add(Credit);
        }

        public void AddCredits(LeaveRules Leave)
        {
            int id = Leave.Id;
            List<Person> List = _dbContext.Person.ToList();
            foreach (var item in List)
            {
                LeaveCredit Credit = new LeaveCredit
                {
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
                AddCredit(Credit);
                Save();
            }
        }

        public void CreateLeaveRule(LeaveRules LeaveType)
        {
            _dbContext.LeaveRules.Add(LeaveType);
        }

        public void DeleteLeaveRule(LeaveRules LeaveType)
        {
            LeaveType.IsActive = false;
        }

        public void EditLeaveRule(LeaveRules LeaveType)
        {
            _dbContext.LeaveRules.Update(LeaveType);
        }

        public IEnumerable<LeaveRules> GetAllLeaveRules()
        {
            return _dbContext.LeaveRules;
        }

        public float GetAvailableLeaves(int PersonId, int LeaveId)
        {
            float n = _dbContext.LeaveCredit.Where(x => x.PersonId == PersonId && x.LeaveId == LeaveId).Select(x => x.Available).FirstOrDefault();
            return n;
        }

        public IEnumerable<LeaveCredit> GetCredits()
        {
            var results = _dbContext.LeaveCredit
                .Select(l => new
                {
                    l,
                    person = l.Person
                })
                .ToList();
            foreach (var x in results)
            {
                x.l.Person = x.person;
            }
            var result = results.Select(x => x.l).ToList();
            return result;
        }

        public void UpdateRequestStatus(int RequestId, string Status)
        {
            LeaveRequest leaveRequest = FindByCondition(l => l.Id == RequestId);
            if (Status == "Approve")
            {
                leaveRequest.Status = "Approved";
                Save();
                
            }
            else if (Status == "Reject")
            {
                leaveRequest.Status = "Rejected";
                leaveRequest.Available = leaveRequest.Available + leaveRequest.RequestedDays;
                Save();
                LeaveCredit leaveCredit = _dbContext.LeaveCredit.Where(c => c.LeaveId == leaveRequest.TypeId && c.PersonId == leaveRequest.PersonId).FirstOrDefault();
                leaveCredit.Available = leaveCredit.Available + leaveRequest.RequestedDays;
                Save();
            }
            else if(Status=="Pending")
            {
                if (leaveRequest.Status == null || leaveRequest.Status == "Rejected")
                {
                    leaveRequest.Status = "Pending";
                    leaveRequest.Available = leaveRequest.Available - leaveRequest.RequestedDays;
                    Save();
                    LeaveCredit leaveCredit = _dbContext.LeaveCredit.Where(c => c.LeaveId == leaveRequest.TypeId && c.PersonId == leaveRequest.PersonId).FirstOrDefault();
                    leaveCredit.Available = leaveCredit.Available - leaveRequest.RequestedDays;
                    Save();
                }
                else
                {
                    leaveRequest.Status = "Pending";
                    Save();
                }
            }
        }
    }
}
