using EIS.Entities.Employee;
using System;
using System.Collections.Generic;

namespace EIS.Repositories.IRepository
{
    public interface IAttendanceRepository : IRepositorybase<Attendance>
    {
        IEnumerable<Person> GetAttendanceYearly(int year);
<<<<<<< HEAD
        IEnumerable<Person> GetAttendanceMonthly(int month, int year);
        IEnumerable<Person> GetAttendanceWeekly(DateTime startOfWeek, DateTime endOfWeek);
        
=======
        IEnumerable<Person> GetAttendanceMonthly(int month,int year);
        IEnumerable<Person> GetAttendanceWeekly(DateTime startOfWeek, DateTime endOfWeek);
>>>>>>> eab0133b5e8f6e86eb09bb18611280e9b8dcee1c
    }
}
    
