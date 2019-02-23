using EIS.Entities.Employee;
using EIS.Entities.Models;
using EIS.Entities.SP;
using System;
using System.Collections.Generic;

namespace EIS.Repositories.IRepository
{
    public interface IAttendanceRepository : IRepositorybase<Attendance>
    {
        //AttendanceReport GetAttendanceReportSummary(int totalDays,int totalWorkingDays, IEnumerable<Attendance> attendanceData);
        EmployeeAttendanceReport GetAttendanceReportSummary(string type, int PersonId, int year, int? month);
        List<AttendanceReportByDate> GetAttendanceReportByDate(DateTime startDate, DateTime endDate, IEnumerable<Attendance> attendanceData,string id,int? loc);  
        Attendance_Report GetAttendanceCountReport(string SearchFor, string InputOne, string InputTwo, int locationId);
         string CalculateDate(DateTime date);
    }


}
