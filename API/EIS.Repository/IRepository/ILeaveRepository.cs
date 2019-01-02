using EIS.Entities.Employee;
using EIS.Entities.Leave;
using System.Collections;
using System.Collections.Generic;

namespace EIS.Repositories.IRepository
{
   public interface ILeaveRepository : IRepositorybase<LeaveRequest>
   {
<<<<<<< HEAD
        IEnumerable<LeaveMaster> GetAllPolicies();
        void CreateLeaveTypeAndSave(LeaveMaster LeaveType);
        void EditLeaveTypeAndSave(LeaveMaster LeaveType);
        void DeleteLeaveTypeAndSave(LeaveMaster LeaveType);
        IEnumerable<LeaveCredit> GetCredits();
        float GetAvailableLeaves(int PersonId, int LeaveId);
        void AddCreditAndSave(LeaveCredit Credit);
        void AddCreditsAndSave(LeaveMaster Leave);
=======
        IEnumerable<LeaveRules> GetAllLeaveRules();
        void CreateLeaveRule(LeaveRules LeaveRule);
        void EditLeaveRule(LeaveRules LeaveRule);
        void DeleteLeaveRule(LeaveRules LeaveRule);
        IEnumerable<LeaveCredit> GetCredits();
        float GetAvailableLeaves(int PersonId, int LeaveId);
        void AddCredit(LeaveCredit Credit);
        void AddCredits(LeaveRules Leave);
>>>>>>> eab0133b5e8f6e86eb09bb18611280e9b8dcee1c
        void UpdateRequestStatus(int RequestId, string Status);
    }
}
