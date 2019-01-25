﻿using EIS.Entities.Leave;
using System.Collections.Generic;
using System.Linq;

namespace EIS.Repositories.IRepository
{
    public interface ILeaveRequestRepository : IRepositorybase<LeaveRequest>
    {
        IQueryable<PastLeaves> GetPastLeaves(int TenantId);
        void AddPastLeave(PastLeaves pastLeave);
        void UpdateRequestStatus(int RequestId, string Status);
        IQueryable<LeaveRequest> GetLeaveRequestUnderMe(int PersonId, int TenantId);
    }
}
