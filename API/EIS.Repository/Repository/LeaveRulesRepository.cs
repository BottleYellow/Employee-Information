using System;
using System.Collections.Generic;
using System.Linq;
using EIS.Data.Context;
using EIS.Entities.Employee;
using EIS.Entities.Leave;
using EIS.Repositories.IRepository;

namespace EIS.Repositories.Repository
{
    public class LeaveRulesRepository: RepositoryBase<LeaveRules>, ILeaveRulesRepository
    {
        public LeaveRulesRepository(ApplicationDbContext dbContext): base(dbContext)
        {

        }

        public void CreateLeaveRuleAndSave(LeaveRules LeaveType)
        {
            _dbContext.LeaveRules.Add(LeaveType);
            Save();
        }

        public void DeleteLeaveRuleAndSave(LeaveRules LeaveType)
        {
            LeaveType.IsActive = false;
            Save();
        }

        public void EditLeaveRuleAndSave(LeaveRules LeaveType)
        {
            _dbContext.LeaveRules.Update(LeaveType);
            Save();
        }        
    }
}
