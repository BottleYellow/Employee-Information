using System;
using System.Collections.Generic;
using System.Linq;
using EIS.Data.Context;
using EIS.Entities.Employee;
using EIS.Entities.Leave;
using EIS.Repositories.IRepository;

namespace EIS.Repositories.Repository
{
    public class PastLeavesRepository: RepositoryBase<PastLeaves>, IPastLeavesRepository
    {
        public PastLeavesRepository(ApplicationDbContext dbContext): base(dbContext)
        {

        }

        public void AddPastLeave(PastLeaves pastLeave)
        {
            _dbContext.PastLeaves.Add(pastLeave);
            _dbContext.SaveChanges();
        }

        public IQueryable<PastLeaves> GetPastLeaves(int TenantId)
        {
            var result = _dbContext.PastLeaves.Where(x => x.TenantId == TenantId);
            return result;
        }
    }
}
