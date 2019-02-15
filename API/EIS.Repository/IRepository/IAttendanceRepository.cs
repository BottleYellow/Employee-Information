using EIS.Entities.Employee;
using EIS.Entities.Models;
using EIS.Entities.SP;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EIS.Repositories.IRepository
{
    public interface IAttendanceRepository : IRepositorybase<Attendance>
    {
        IList<AttendanceData> GetAttendanceYearly(int year, int loc);
        IList<AttendanceData> GetAttendanceMonthly(int month,int year,int loc);
        AttendanceReport GetAttendanceReportSummary(int totalDays,int totalWorkingDays, IEnumerable<Attendance> attendanceData);
        List<AttendanceReportByDate> GetAttendanceReportByDate(DateTime startDate, DateTime endDate, IEnumerable<Attendance> attendanceData,string id,int? loc);  
        Attendance_Report GetAttendanceCountReport(string SearchFor, string InputOne, string InputTwo, int locationId);
    }


}
