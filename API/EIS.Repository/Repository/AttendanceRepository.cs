using EIS.Data.Context;
using EIS.Entities.Employee;
using EIS.Repositories.IRepository;

namespace EIS.Repositories.Repository
{
    public class AttendanceRepository: RepositoryBase<Attendance>,IAttendanceRepository
    {
        public AttendanceRepository(ApplicationDbContext dbContext) : base(dbContext)
        {

        }
    }
}
