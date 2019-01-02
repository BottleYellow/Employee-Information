using EIS.Entities.Leave;
using System.Collections.Generic;

namespace EIS.Repositories.IRepository
{
    public interface ILeaveRepository : IRepositorybase<LeaveRequest>
   {
        IEnumerable<LeaveMaster> GetAllPolicies();
        void CreateLeaveType(LeaveMaster LeaveType);
        void EditLeaveType(LeaveMaster LeaveType);
        void DeleteLeaveType(LeaveMaster LeaveType);
        IEnumerable<LeaveCredit> GetCredits();
        float GetAvailableLeaves(int PersonId, int LeaveId);
        void AddCredit(LeaveCredit Credit);
        void AddCredits(LeaveMaster Leave);
        void UpdateRequestStatus(int RequestId, string Status);
    }
}
