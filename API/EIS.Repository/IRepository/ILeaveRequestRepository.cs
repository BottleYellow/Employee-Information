using EIS.Entities.Leave;
using EIS.Entities.Models;
using EIS.Entities.SP;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EIS.Repositories.IRepository
{
    public interface ILeaveRequestRepository : IRepositorybase<LeaveRequest>
    {
        IQueryable<PastLeaves> GetPastLeaves(int PersonId,int TenantId,int? LocationId);
        void AddPastLeave(PastLeaves pastLeave);
        LeaveRequestForEdit GetLeaveRequestForEdit(int id);
        string UpdateRequestStatus(int RequestId, string Status,int PersonId,float? OldCountForEdit);
        IQueryable<LeaveRequest> GetLeaveRequestUnderMe(int PersonId, int TenantId);
        string CheckForScheduledLeave(int PersonId, DateTime FromDate, DateTime ToDate,int? LeaveId);
        string CheckForScheduledPastLeave(int PersonId, DateTime FromDate, DateTime ToDate);
        List<LeaveRequestViewModel> GetLeaveData(int locationId, string employeeId, int month, int year,int TenantId,string leaveType,bool status);
        List<SP_EmployeeLeaveRequest> GetEmployeeLeaveData(int PersonId);
    }
}
