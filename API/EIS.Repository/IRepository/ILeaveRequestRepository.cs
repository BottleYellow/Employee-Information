using EIS.Entities.Leave;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EIS.Repositories.IRepository
{
    public interface ILeaveRequestRepository : IRepositorybase<LeaveRequest>
    {
        IQueryable<PastLeaves> GetPastLeaves(int PersonId,int TenantId,int? LocationId);
        void AddPastLeave(PastLeaves pastLeave);
        string UpdateRequestStatus(int RequestId, string Status,int PersonId);
        IQueryable<LeaveRequest> GetLeaveRequestUnderMe(int PersonId, int TenantId);
        string CheckForScheduledLeave(int PersonId, DateTime FromDate, DateTime ToDate);
    }
}
