using EIS.Entities.Employee;
using EIS.Entities.Leave;
using System.Collections;
using System.Collections.Generic;

namespace EIS.Repositories.IRepository
{
   public interface ILeaveRepository : IRepositorybase<LeaveRequest>
   {
        IEnumerable<LeaveRules> GetAllLeaveRules();
        void CreateLeaveRuleAndSave(LeaveRules LeaveRule);
        void EditLeaveRuleAndSave(LeaveRules LeaveRule);
        void DeleteLeaveRuleAndSave(LeaveRules LeaveRule);
        IEnumerable<LeaveCredit> GetCredits();
        float GetAvailableLeaves(int PersonId, int LeaveId);
        void AddCreditAndSave(LeaveCredit Credit);
        void AddCreditsAndSave(LeaveRules Leave);
        void UpdateRequestStatus(int RequestId, string Status);
    }
}
