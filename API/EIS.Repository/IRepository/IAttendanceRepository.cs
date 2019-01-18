using EIS.Entities.Employee;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EIS.Repositories.IRepository
{
    public interface IAttendanceRepository : IRepositorybase<Attendance>
    {
        IQueryable<Person> GetAttendanceYearly(int year);
        IQueryable<Person> GetAttendanceMonthly(int month,int year);
        IQueryable<Person> GetAttendanceWeekly(DateTime startOfWeek, DateTime endOfWeek);
    }

    
}
