using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace EIS.Entities.SP
{

    public class Admin_Dashboard
    {
        public List<SP_AdminDashboard> sP_AdminDashboards { get; set; }
        public SP_AdminDashboardCount sP_AdminDashboardCount { get; set; }
        public List<Sp_AdminDashboardLeave> sp_AdminDashboardLeaves { get; set; }
    }

    public class SP_AdminDashboard
    {
        [Key]
        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }
        public string LocationName { get; set; }
        public Nullable<System.DateTime> DateIn { get; set; }
        public Nullable<System.DateTime> DateOut { get; set; }
        public Nullable<System.TimeSpan> TimeIn { get; set; }
        public Nullable<System.TimeSpan> TimeOut { get; set; }
        public Nullable<System.TimeSpan> TotalHours { get; set; }
        public string Status { get; set; }
    }
    public class SP_AdminDashboardCount
    {
        [Key]
        public int Id { get; set; }
        public int AllEmployeesCount { get; set; }
        public int PresentEmployees { get; set; }
        public int OnLeaveEmployee { get; set; }
        public int PendingLeavesCount { get; set; }
        public int AbsentEmployees { get; set; }

    }
    public class Sp_AdminDashboardLeave
    {
        [Key]
        public int Id { get; set; }
        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }
        public string LeaveType { get; set; }
        public System.DateTime FromDate { get; set; }
        public System.DateTime ToDate { get; set; }
        public double RequestedDays { get; set; }
        public string ApprovedBy { get; set; }
        public System.DateTime AppliedDate { get; set; }
        public string LocationName { get; set; }
        public double ApprovedDays { get; set; }
        public string Reason { get; set; }
        public string Status { get; set; }
    }

    public class SP_GetAttendanceCountReport
    {
        [Key]
        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }
        public string LocationName { get; set; }
        public Nullable<int> PresentDays { get; set; }
        public Nullable<int> WorkingDay { get; set; }
        public Nullable<int> NoLeave { get; set; }
    }
    public class Attendance_Report
    {
        public List<SP_GetAttendanceCountReport> sP_GetAttendanceCountReports { get; set; }
    }

    public class GetAdminHrManager
    {
        [Key]
        public string EmailAddress { get; set; }
        public string Name { get; set; }
    }


}
