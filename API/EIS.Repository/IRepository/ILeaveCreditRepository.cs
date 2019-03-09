using EIS.Entities.Leave;
using System.Linq;

namespace EIS.Repositories.IRepository
{
    public interface ILeaveCreditRepository : IRepositorybase<LeaveCredit>
    { 
        float GetAvailableLeaves(int PersonId, int LeaveId);
        void AddCreditAndSave(LeaveCredit Credit);
        void AddCreditsAndSave(LeaveRulesWithEmp Leave);
    }
}
