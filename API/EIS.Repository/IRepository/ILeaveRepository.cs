using EIS.Entities.Employee;
using EIS.Entities.Leave;
using System.Collections;
using System.Collections.Generic;

namespace EIS.Repositories.IRepository
{
   public interface ILeaveRepository : IRepositorybase<LeaveRequest>
   {
        IEnumerable<LeaveRules> GetAllLeaveRules();
        void CreateLeaveRule(LeaveRules LeaveRule);
        void EditLeaveRule(LeaveRules LeaveRule);
        void DeleteLeaveRule(LeaveRules LeaveRule);
        IEnumerable<LeaveCredit> GetCredits();
        float GetAvailableLeaves(int PersonId, int LeaveId);
        void AddCredit(LeaveCredit Credit);
        void AddCredits(LeaveRules Leave);
        void UpdateRequestStatus(int RequestId, string Status);
    }
}
