using EIS.Data.Context;
using EIS.Entities.Employee;
using EIS.Repositories.IRepository;

namespace EIS.Repositories.Repository
{
   public class LeaveRepository: RepositoryBase<Leaves>,ILeaveRepository
    {
        public LeaveRepository(DbContext dbContext): base(dbContext)
        {

        }
    }
}
