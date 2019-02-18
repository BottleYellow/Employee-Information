using EIS.Entities.Leave;
using System.Linq;

namespace EIS.Repositories.IRepository
{
    public interface ILeaveRulesRepository : IRepositorybase<LeaveRules>
   {
        IQueryable<LeaveRules> GetAllLeaveRules();
        void CreateLeaveRuleAndSave(LeaveRules LeaveRule);
        void EditLeaveRuleAndSave(LeaveRules LeaveRule);
        void DeleteLeaveRuleAndSave(LeaveRules LeaveRule);
        
    }
}
