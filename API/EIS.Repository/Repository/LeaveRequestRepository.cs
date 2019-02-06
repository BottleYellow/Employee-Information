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

        public void AddPastLeave(PastLeaves pastLeave)
        {
            _dbContext.PastLeaves.Add(pastLeave);
            _dbContext.SaveChanges();
        }

        public IQueryable<LeaveRequest> GetLeaveRequestUnderMe(int PersonId, int TenantId)
        {
            var results = from Requests in _dbContext.LeaveRequests
                          join Person in _dbContext.Person on Requests.PersonId equals Person.Id
                          where Person.ReportingPersonId == PersonId && Requests.TenantId == TenantId
                          select Requests;
            return results;
        }

        public IQueryable<PastLeaves> GetPastLeaves(int PersonId,int TenantId)
        {
            var result = PersonId == 0 ? _dbContext.PastLeaves.Where(x => x.TenantId == TenantId) : _dbContext.PastLeaves.Where(x => x.TenantId == TenantId && x.PersonId == PersonId);
            return result;
        }

        public void UpdateRequestStatus(int RequestId, string Status)
        {
            var leaveCredit = new LeaveCredit();
            LeaveRequest leaveRequest = _dbContext.LeaveRequests.Where(x => x.Id == RequestId).FirstOrDefault();
            if (Status == "Approve")
            {
                leaveRequest.Status = "Approved";
                var requests = _dbContext.LeaveRequests.Where(x => x.PersonId == leaveRequest.PersonId);

            }
            else if (Status == "Reject")
            {
                leaveRequest.Status = "Rejected";
                leaveRequest.Available = leaveRequest.Available + leaveRequest.RequestedDays;
                leaveCredit = _dbContext.LeaveCredit.Where(c => c.LeaveId == leaveRequest.TypeId && c.PersonId == leaveRequest.PersonId).FirstOrDefault();
                leaveCredit.Available = leaveCredit.Available + leaveRequest.RequestedDays;
            }
            else if (Status == "Pending")
            {
                if (leaveRequest.Status == null || leaveRequest.Status == "Rejected")
                {
                    leaveRequest.Available = leaveRequest.Available - leaveRequest.RequestedDays;
                    leaveCredit = _dbContext.LeaveCredit.Where(c => c.LeaveId == leaveRequest.TypeId && c.PersonId == leaveRequest.PersonId).FirstOrDefault();
                    leaveCredit.Available = leaveCredit.Available - leaveRequest.RequestedDays;
                }
                leaveRequest.Status = "Pending";
            }
            else if (Status == "Cancel")
            {
                if (leaveRequest.Status == "Pending")
                {
                    leaveRequest.Status = "Cancelled";
                    leaveRequest.Available = leaveRequest.Available + leaveRequest.RequestedDays;
                    leaveCredit = _dbContext.LeaveCredit.Where(c => c.LeaveId == leaveRequest.TypeId && c.PersonId == leaveRequest.PersonId).FirstOrDefault();
                    leaveCredit.Available = leaveCredit.Available + leaveRequest.RequestedDays;
                }
                else if (leaveRequest.Status == "Approved")
                {
                    leaveRequest.Status = "Requested For Cancel";
                }
            }
            else if (Status == "Accept Cancel")
            {
                leaveRequest.Status = "Cancelled";
                leaveRequest.Available = leaveRequest.Available + leaveRequest.RequestedDays;
                leaveCredit = _dbContext.LeaveCredit.Where(c => c.LeaveId == leaveRequest.TypeId && c.PersonId == leaveRequest.PersonId).FirstOrDefault();
                leaveCredit.Available = leaveCredit.Available + leaveRequest.RequestedDays;
            }
            else if (Status == "Reject Cancel")
            {
                leaveRequest.Status = "Approved(Rejected Cancel Request)";
            }
            else if (Status == null)
            {
                leaveCredit = _dbContext.LeaveCredit.Where(c => c.LeaveId == leaveRequest.TypeId && c.PersonId == leaveRequest.PersonId).FirstOrDefault();
                leaveCredit.Available = leaveRequest.Available - leaveRequest.RequestedDays;
                leaveRequest.Available = leaveRequest.Available - leaveRequest.RequestedDays;
            }
            Save();
        }
    }
}
