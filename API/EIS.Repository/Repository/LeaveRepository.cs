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

        public void AddCreditAndSave(LeaveCredit Credit)
        {
            _dbContext.LeaveCredit.Add(Credit);
<<<<<<< HEAD
            Save();
        }

        public void AddCreditsAndSave(LeaveMaster Leave)
=======
        }

        public void AddCredits(LeaveRules Leave)
>>>>>>> eab0133b5e8f6e86eb09bb18611280e9b8dcee1c
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
                AddCreditAndSave(Credit);
            }
        }

<<<<<<< HEAD
        public void CreateLeaveTypeAndSave(LeaveMaster LeaveType)
        {
            _dbContext.LeaveMaster.Add(LeaveType);
            Save();
        }


        public void DeleteLeaveTypeAndSave(LeaveMaster LeaveType)
=======
        public void CreateLeaveRule(LeaveRules LeaveType)
        {
            _dbContext.LeaveRules.Add(LeaveType);
        }

        public void DeleteLeaveRule(LeaveRules LeaveType)
>>>>>>> eab0133b5e8f6e86eb09bb18611280e9b8dcee1c
        {
            LeaveType.IsActive = false;
            Save();
        }

<<<<<<< HEAD

        public void EditLeaveTypeAndSave(LeaveMaster LeaveType)
        {
            _dbContext.LeaveMaster.Update(LeaveType);
            Save();
=======
        public void EditLeaveRule(LeaveRules LeaveType)
        {
            _dbContext.LeaveRules.Update(LeaveType);
>>>>>>> eab0133b5e8f6e86eb09bb18611280e9b8dcee1c
        }

        public IEnumerable<LeaveRules> GetAllLeaveRules()
        {
<<<<<<< HEAD
            return _dbContext.LeaveMaster;
=======
            return _dbContext.LeaveRules;
>>>>>>> eab0133b5e8f6e86eb09bb18611280e9b8dcee1c
        }

        public float GetAvailableLeaves(int PersonId, int LeaveId)
        {
            float n = _dbContext.LeaveCredit.Where(x => x.PersonId == PersonId && x.LeaveId == LeaveId).Select(x => x.Available).FirstOrDefault();
            return n;
        }

        public IEnumerable<LeaveCredit> GetCredits()
        {
<<<<<<< HEAD
            return _dbContext.LeaveCredit;
=======
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
>>>>>>> eab0133b5e8f6e86eb09bb18611280e9b8dcee1c
        }

        public void UpdateRequestStatus(int RequestId, string Status)
        {
            LeaveRequest leaveRequest = FindByCondition(l => l.Id == RequestId);
            if (Status == "Approve")
            {
                leaveRequest.Status = "Approved";                            
            }
            else if (Status == "Reject")
            {
                leaveRequest.Status = "Rejected";
<<<<<<< HEAD
                leaveRequest.Available = leaveRequest.Available + leaveRequest.TotalRequestedDays;
                LeaveCredit leaveCredit = _dbContext.LeaveCredit.Where(c => c.LeaveId == leaveRequest.TypeId && c.PersonId == leaveRequest.PersonId).FirstOrDefault();
                leaveCredit.Available = leaveCredit.Available + leaveRequest.TotalRequestedDays;          
=======
                leaveRequest.Available = leaveRequest.Available + leaveRequest.RequestedDays;
                Save();
                LeaveCredit leaveCredit = _dbContext.LeaveCredit.Where(c => c.LeaveId == leaveRequest.TypeId && c.PersonId == leaveRequest.PersonId).FirstOrDefault();
                leaveCredit.Available = leaveCredit.Available + leaveRequest.RequestedDays;
                Save();
>>>>>>> eab0133b5e8f6e86eb09bb18611280e9b8dcee1c
            }
            else if(Status=="Pending")
            {
                if (leaveRequest.Status == null || leaveRequest.Status == "Rejected")
                {
                    leaveRequest.Status = "Pending";
<<<<<<< HEAD
                    leaveRequest.Available = leaveRequest.Available - leaveRequest.TotalRequestedDays;
                    LeaveCredit leaveCredit = _dbContext.LeaveCredit.Where(c => c.LeaveId == leaveRequest.TypeId && c.PersonId == leaveRequest.PersonId).FirstOrDefault();
                    leaveCredit.Available = leaveCredit.Available - leaveRequest.TotalRequestedDays;                
=======
                    leaveRequest.Available = leaveRequest.Available - leaveRequest.RequestedDays;
                    Save();
                    LeaveCredit leaveCredit = _dbContext.LeaveCredit.Where(c => c.LeaveId == leaveRequest.TypeId && c.PersonId == leaveRequest.PersonId).FirstOrDefault();
                    leaveCredit.Available = leaveCredit.Available - leaveRequest.RequestedDays;
                    Save();
>>>>>>> eab0133b5e8f6e86eb09bb18611280e9b8dcee1c
                }
                else
                {
                    leaveRequest.Status = "Pending";                   
                }
            }
            Save();
        }
    }
}
