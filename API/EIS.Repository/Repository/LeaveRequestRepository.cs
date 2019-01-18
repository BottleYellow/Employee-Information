using System;
using System.Collections.Generic;
using System.Linq;
using EIS.Data.Context;
using EIS.Entities.Employee;
using EIS.Entities.Leave;
using EIS.Repositories.IRepository;

namespace EIS.Repositories.Repository
{
    public class LeaveRequestRepository: RepositoryBase<LeaveRequest>, ILeaveRequestRepository
    {
        public LeaveRequestRepository(ApplicationDbContext dbContext): base(dbContext)
        {

        }
        public IEnumerable<LeaveRequest> GetLeaveRequestUnderMe(int PersonId, int TenantId)
        {
            var results = from Requests in _dbContext.LeaveRequests
                          join Person in _dbContext.Person on Requests.PersonId equals Person.Id
                          where Person.ReportingPersonId == PersonId && Requests.TenantId == TenantId
                          select Requests;
            return results;
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
                leaveRequest.Available = leaveRequest.Available + leaveRequest.RequestedDays;
                LeaveCredit leaveCredit = _dbContext.LeaveCredit.Where(c => c.LeaveId == leaveRequest.TypeId && c.PersonId == leaveRequest.PersonId).FirstOrDefault();
                leaveCredit.Available = leaveCredit.Available + leaveRequest.RequestedDays;
            }
            else if(Status=="Pending")
            {
                if (leaveRequest.Status == null || leaveRequest.Status == "Rejected")
                {
                    leaveRequest.Available = leaveRequest.Available - leaveRequest.RequestedDays;
                    LeaveCredit leaveCredit = _dbContext.LeaveCredit.Where(c => c.LeaveId == leaveRequest.TypeId && c.PersonId == leaveRequest.PersonId).FirstOrDefault();
                    leaveCredit.Available = leaveCredit.Available - leaveRequest.RequestedDays;
                }
                leaveRequest.Status = "Pending";
            }
            Save();
        }
    }
}
