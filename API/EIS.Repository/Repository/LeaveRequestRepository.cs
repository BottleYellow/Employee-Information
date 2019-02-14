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

        public string CheckForScheduledLeave(int PersonId,DateTime FromDate, DateTime ToDate)
        {
            string result = "success";
            IQueryable<LeaveRequest> requests = _dbContext.LeaveRequests.Where(x => ((x.FromDate <=FromDate && FromDate<=x.ToDate) || (x.FromDate <= ToDate && ToDate<=x.ToDate)||(FromDate <= x.FromDate && x.FromDate <= ToDate) || (FromDate <= x.ToDate && x.ToDate <= ToDate)) && (x.PersonId == PersonId));
            LeaveRequest request = null;
            if (requests != null && requests.Count() > 0)
            {
                result = "Leaves are already scheduled on requested dates";
                request = requests.Where(x => x.Status == "Cancelled").FirstOrDefault();
                if (request != null)
                {
                    result = "success";
                }
            }
           
            return result;
        }

        public IQueryable<LeaveRequest> GetLeaveRequestUnderMe(int PersonId, int TenantId)
        {
            var results = from Requests in _dbContext.LeaveRequests
                          join Person in _dbContext.Person on Requests.PersonId equals Person.Id
                          where Person.ReportingPersonId == PersonId && Requests.TenantId == TenantId
                          select Requests;
            return results;
        }

        public IQueryable<PastLeaves> GetPastLeaves(int PersonId,int TenantId,int? LocationId)
        {
            IQueryable<PastLeaves> result = null;
            if (PersonId == 0 && LocationId == 0)
            {
                result = _dbContext.PastLeaves.Include(x => x.Person).Include(x => x.Person.Location).Where(x => x.TenantId == TenantId && x.Person.Location.IsActive == true);
            }
            else if (PersonId != 0 && LocationId == 0)
            {
                result= _dbContext.PastLeaves.Include(x => x.Person).Include(x => x.Person.Location).Where(x => x.TenantId == TenantId && x.PersonId == PersonId && x.Person.Location.IsActive == true);
            }
            else
            {
                result = _dbContext.PastLeaves.Include(x=>x.Person).Include(x => x.Person.Location).Where(x => x.TenantId == TenantId && x.Person.LocationId==LocationId && x.Person.Location.IsActive == true);
            }
           
            return result;
        }

        public void UpdateRequestStatus(int RequestId, string Status,int PersonId)
        {
            var leaveCredit = new LeaveCredit();
            LeaveRequest leaveRequest = _dbContext.LeaveRequests.Where(x => x.Id == RequestId).FirstOrDefault();
            leaveRequest.UpdatedDate = DateTime.Now;
            if (Status == "Approve")
            {
                leaveRequest.ApprovedBy = PersonId;
                leaveRequest.Status = "Approved";
                var requests = _dbContext.LeaveRequests.Where(x => x.PersonId == leaveRequest.PersonId);

            }
            else if (Status == "Reject")
            {
                leaveRequest.UpdatedBy = PersonId;
                leaveRequest.Status = "Rejected";
                leaveRequest.Available = leaveRequest.Available + leaveRequest.RequestedDays;
                leaveCredit = _dbContext.LeaveCredit.Where(c => c.LeaveId == leaveRequest.TypeId && c.PersonId == leaveRequest.PersonId).FirstOrDefault();
                leaveCredit.Available = leaveCredit.Available + leaveRequest.RequestedDays;
            }
            else if (Status == "Pending")
            {
                leaveRequest.UpdatedBy = PersonId;
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
                leaveRequest.UpdatedBy = PersonId;
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
                leaveRequest.UpdatedBy = PersonId;
                leaveRequest.Status = "Cancelled";
                leaveRequest.Available = leaveRequest.Available + leaveRequest.RequestedDays;
                leaveCredit = _dbContext.LeaveCredit.Where(c => c.LeaveId == leaveRequest.TypeId && c.PersonId == leaveRequest.PersonId).FirstOrDefault();
                leaveCredit.Available = leaveCredit.Available + leaveRequest.RequestedDays;
            }
            else if (Status == "Reject Cancel")
            {
                leaveRequest.UpdatedBy = PersonId;
                leaveRequest.Status = "Approved(Rejected Cancel Request)";
            }
            else if (Status == null)
            {
                leaveRequest.UpdatedBy = PersonId;
                leaveCredit = _dbContext.LeaveCredit.Where(c => c.LeaveId == leaveRequest.TypeId && c.PersonId == leaveRequest.PersonId).FirstOrDefault();
                leaveCredit.Available = leaveRequest.Available - leaveRequest.RequestedDays;
                leaveRequest.Available = leaveRequest.Available - leaveRequest.RequestedDays;
            }
            Save();
        }
    }
}
