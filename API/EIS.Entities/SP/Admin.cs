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
        public Nullable<TimeSpan> WorkingHours { get; set; }
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
        public int ApprovedLeavesCount { get; set; }
        public int RejectedLeavesCount { get; set; }

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

    public class Attendance_Report
    {
        public List<SP_GetAttendanceCountReport> sP_GetAttendanceCountReports { get; set; }
        public List<SP_GetMonthlyAttendanceData> SP_GetMonthlyAttendanceData { get; set; }
        public List<SP_GetAttendanceLeaveData> sP_GetAttendanceLeaveDatas { get; set; }

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
        public Nullable<int> AbsentDay { get; set; }
        public Nullable<int> ExtraWorkingDays { get; set; }
    }
    public class SP_GetMonthlyAttendanceData
    {
        [Key]
        public Nullable<long> SrId { get; set; }
        public string EmployeeCode { get; set; }
        public Nullable<System.DateTime> DateIn { get; set; }
        public Nullable<System.TimeSpan> TimeIn { get; set; }
        public Nullable<System.TimeSpan> TimeOut { get; set; }
        public Nullable<System.TimeSpan> TotalHours { get; set; }
        public string Status { get; set; }

    }
    //New Implementation
    public class SP_GetAttendanceCountReport_New
    {
        [Key]
        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }
        public string LocationName { get; set; }
        public Nullable<int> WorkingDay { get; set; }
        public Nullable<int> PresentDays { get; set; }
        public Nullable<int> TotalGrantedLeaves { get; set; }
        public Nullable<int> TotalLeavesTaken { get; set; }
        public Nullable<int> TotalAbsentDays { get; set; }
        public Nullable<int> ProposedLeaves { get; set; }
        public string BalanceLeaves { get; set; } 
        public Nullable<int> AdjustedLeaves { get; set; }
        public Nullable<int> LeavesWithoutPay { get; set; }
    }


    public class SP_GetAttendanceLeaveData
    {   [Key]
        public int Id { get; set; }
        public string EmployeeCode { get; set; }
        public Nullable<DateTime> Date { get; set; }
        public string Reason { get; set; }
        public string DateStatus { get; set; }
        public string HrStatus { get; set; }

    }
    public class Attendance_Report_New
    {
        public List<SP_GetAttendanceCountReport_New> sP_GetAttendanceCountReportsNew { get; set; }
        public List<SP_GetAttendanceLeaveData> sP_GetAttendanceLeaveDatas { get; set; }
        public List<SP_GetMonthlyAttendanceData> SP_GetMonthlyAttendanceData { get; set; }
    }


    public class GetAdminHrManager
    {
        [Key]
        public string EmailAddress { get; set; }
        public string Name { get; set; }
    }

    public class SP_GetEmployee
    {
        [Key]
        public string EmployeeCode { get; set; }
        public string FullName { get; set; }
        public string Image { get; set; }
        public string Gender { get; set; }
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }
        public string MobileNumber { get; set; }
        public string EmailAddress { get; set; }
        [DataType(DataType.Date)]
        public DateTime JoinDate { get; set; }
        public string CreatedBy { get; set; }
        public string LocationName { get; set; }
        public string Role { get; set; }
        public bool IsActive { get; set; }
    }

    public class SP_GetDateWiseAttendance
    {
        [Key]
        public Nullable<long> SrId { get; set; }
        public string EmployeeCode { get; set; }
        public string FullName { get; set; }
        public Nullable<System.DateTime> date { get; set; }
        public Nullable<System.TimeSpan> TimeIn { get; set; }
        public Nullable<System.TimeSpan> TimeOut { get; set; }
        public Nullable<System.TimeSpan> TotalHours { get; set; }
        public string Status { get; set; }
    }
    public class SP_GetLeavesInDetail
    {
        [Key]
        public Nullable<int> Id { get; set; }
        public Nullable<System.DateTime> Date { get; set; }
        public string Reason { get; set; }
        public string PaidStatus { get; set; }
        public string HrStatus { get; set; }
    }
    public class LeavesInDetail
    {
        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }
        public List<SP_GetLeavesInDetail> sP_GetLeavesInDetail { get; set; }
    }
}
