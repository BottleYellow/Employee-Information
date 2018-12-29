using EIS.Entities.Employee;
using System.Collections.Generic;

namespace EIS.Repositories.IRepository
{
    public interface IAttendanceRepository : IRepositorybase<Attendance>
    {
        IEnumerable<Person> GetAttendanceYearly(int year);
    }

    
}
