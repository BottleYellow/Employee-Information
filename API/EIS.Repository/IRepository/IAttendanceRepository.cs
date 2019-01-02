using EIS.Entities.Employee;
using System;
using System.Collections.Generic;

namespace EIS.Repositories.IRepository
{
    public interface IAttendanceRepository : IRepositorybase<Attendance>
    {
        IEnumerable<Person> GetAttendanceYearly(int year);
        IEnumerable<Person> GetAttendanceMonthly(int month,int year);
        IEnumerable<Person> GetAttendanceWeekly(DateTime startOfWeek, DateTime endOfWeek);
    }

    
}
