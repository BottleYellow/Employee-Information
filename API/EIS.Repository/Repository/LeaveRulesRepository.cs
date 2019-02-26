using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using EIS.Data.Context;
using EIS.Entities.Employee;
using EIS.Entities.Leave;
using EIS.Entities.SP;
using EIS.Repositories.IRepository;
using Microsoft.EntityFrameworkCore;

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

        public int GetLeaveCount(int PersonId, int TypeId)
        {
            int result = 0;
            ActualLeaveCount Model = new ActualLeaveCount();

            var param1 = new SqlParameter("@PersonId", PersonId);
            var param2 = new SqlParameter("@TypeId", TypeId);
            string usp = "LMS.usp_GetLeaveCount @TypeId,@PersonId";
            Model = _dbContext._sp_GetLeaveCount.FromSql(usp, param2, param1).FirstOrDefault();
            if (Model != null) result = Model.LeaveCount;
            return result;
        }
    }
}
