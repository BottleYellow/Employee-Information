using EIS.Entities.Employee;
using EIS.Entities.Models;
using EIS.Entities.SP;
using System;
using System.Collections.Generic;

namespace EIS.Repositories.IRepository
{
    public interface IAttendanceRepository : IRepositorybase<Attendance>
    {
        EmployeeAttendanceReport GetAttendanceReportSummary(string type, int PersonId, int year, int? month);
        List<AttendanceReportByDate> GetAttendanceReportByDate(DateTime startDate, DateTime endDate, IEnumerable<Attendance> attendanceData, string id, int? loc);
        Attendance_Report GetAttendanceCountReport(string SearchFor, string InputOne, string InputTwo, int locationId);
        string CalculateDate(DateTime date);
        List<SP_GetDateWiseAttendance> dateWiseAttendances(int PersonId, int LocationId, string fromDate, string toDate);
    }


}
