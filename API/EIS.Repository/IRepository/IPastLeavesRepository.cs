using EIS.Entities.Leave;
using System.Linq;

namespace EIS.Repositories.IRepository
{
    public interface IPastLeavesRepository : IRepositorybase<PastLeaves>
    {
        IQueryable<PastLeaves> GetPastLeaves(int TenantId);
        void AddPastLeave(PastLeaves pastLeave);
    }
}
