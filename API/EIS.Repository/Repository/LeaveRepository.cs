using System;
using System.Collections.Generic;
using System.Linq;
using EIS.Data.Context;
using EIS.Entities.Employee;
using EIS.Entities.Leave;
using EIS.Repositories.IRepository;

namespace EIS.Repositories.Repository
{
   public class LeaveRepository: RepositoryBase<LeaveRequest>, ILeaveRepository
    {
        public LeaveRepository(ApplicationDbContext dbContext): base(dbContext)
        {

        }

        public void AddCredit(LeaveCredit Credit)
        {
            _dbcontext.LeaveCredit.Add(Credit);
        }

        public void AddCredits(LeaveMaster Leave)
        {
            int id = Leave.Id;
            List<Person> List = _dbcontext.Person.ToList();
            foreach (var item in List)
            {
                LeaveCredit Credit = new LeaveCredit
                {
                    EmployeeName = item.FirstName +" "+ item.LastName,
                    PersonId = item.Id,
                    LeaveType = Leave.LeaveType,
                    Days = Leave.Days,
                    Available = Leave.Days,
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

        public void CreateLeaveType(LeaveMaster LeaveType)
        {
            _dbcontext.LeaveMaster.Add(LeaveType);
        }

        public void DeleteLeaveType(LeaveMaster LeaveType)
        {
            LeaveType.IsActive = false;
        }

        public void EditLeaveType(LeaveMaster LeaveType)
        {
            _dbcontext.LeaveMaster.Update(LeaveType);
        }

        public IEnumerable<LeaveMaster> GetAllPolicies()
        {
            return _dbcontext.LeaveMaster;
        }

        public float GetAvailableLeaves(int PersonId, int LeaveId)
        {
            float n = _dbcontext.LeaveCredit.Where(x => x.PersonId == PersonId && x.LeaveId == LeaveId).Select(x => x.Available).FirstOrDefault();
            return n;
        }

        public IEnumerable<LeaveCredit> GetCredits()
        {
            return _dbcontext.LeaveCredit;
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
                leaveRequest.Available = leaveRequest.Available + leaveRequest.TotalRequestedDays;
                Save();
                LeaveCredit leaveCredit = _dbcontext.LeaveCredit.Where(c => c.LeaveId == leaveRequest.TypeId && c.PersonId == leaveRequest.PersonId).FirstOrDefault();
                leaveCredit.Available = leaveCredit.Available + leaveRequest.TotalRequestedDays;
                Save();
            }
            else if(Status=="Pending")
            {
                if (leaveRequest.Status == null || leaveRequest.Status == "Rejected")
                {
                    leaveRequest.Status = "Pending";
                    leaveRequest.Available = leaveRequest.Available - leaveRequest.TotalRequestedDays;
                    Save();
                    LeaveCredit leaveCredit = _dbcontext.LeaveCredit.Where(c => c.LeaveId == leaveRequest.TypeId && c.PersonId == leaveRequest.PersonId).FirstOrDefault();
                    leaveCredit.Available = leaveCredit.Available - leaveRequest.TotalRequestedDays;
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
