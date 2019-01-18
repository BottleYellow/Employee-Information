using EIS.Entities.Leave;
using System.Collections.Generic;
using System.Linq;

namespace EIS.Repositories.IRepository
{
    public interface ILeaveRequestRepository : IRepositorybase<LeaveRequest>
    {
        void UpdateRequestStatus(int RequestId, string Status);
    }
}
