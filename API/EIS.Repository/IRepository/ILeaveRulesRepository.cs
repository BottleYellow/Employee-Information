using EIS.Entities.Leave;
using System;
using System.Linq;

namespace EIS.Repositories.IRepository
{
    public interface ILeaveRulesRepository : IRepositorybase<LeaveRules>
   {
        void CreateLeaveRuleAndSave(LeaveRules LeaveRule);
        void EditLeaveRuleAndSave(LeaveRules LeaveRule);
        void DeleteLeaveRuleAndSave(LeaveRules LeaveRule);
        int GetLeaveCount(int PersonId,int TypeId);
    }
}
