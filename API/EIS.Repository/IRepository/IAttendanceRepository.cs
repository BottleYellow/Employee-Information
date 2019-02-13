using EIS.Entities.Employee;
using EIS.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EIS.Repositories.IRepository
{
    public interface IAttendanceRepository : IRepositorybase<Attendance>
    {
        IList<AttendanceData> GetAttendanceYearly(int year, int loc);
        IList<AttendanceData> GetAttendanceMonthly(int month,int year,int loc);
        IList<AttendanceData> GetAttendanceWeekly(DateTime startOfWeek, DateTime endOfWeek, int loc);
        AttendanceReport GetAttendanceReportSummary(int totalDays,int totalWorkingDays, IEnumerable<Attendance> attendanceData);
        IEnumerable<Attendance> GetAttendanceReportByDate(DateTime startDate, DateTime endDate, IQueryable<Attendance> attendanceData);
    }   
}
