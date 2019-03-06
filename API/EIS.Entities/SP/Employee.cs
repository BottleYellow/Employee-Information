using EIS.Entities.Leave;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace EIS.Entities.SP
{
    public class Employee_Dashboard
    {
        public List<SP_EmployeeDashboard> SP_EmployeeDashboards { get; set; }
        public SP_EmployeeDashboardCount SP_EmployeeDashboardCount { get; set; }
        
        public List<SP_EmployeeLeaveRequest> SP_EmployeeLeaveRequest { get; set; }
    }
    public class SP_EmployeeDashboard
    {
        [Key]
        public Nullable<System.DateTime> DateIn { get; set; }
        public Nullable<System.TimeSpan> TimeIn { get; set; }
        public Nullable<System.TimeSpan> TimeOut { get; set; }
        public Nullable<System.TimeSpan> TotalHours { get; set; }
        public string Status { get; set; }
    }
    public class SP_EmployeeDashboardCount
    {
        [Key]
        public int Id { get; set; }
        public int PresentDays { get; set; }
        public int OnLeaveDays { get; set; }
        public int TotalLeavesTaken { get; set; }
        public int AvailableLeaves { get; set; }
    }

    public class SP_EmployeeLeaveRequest
    {
        [Key]
        public int Id { get; set; }
        public string LeaveType { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public Double RequestedDays { get; set; }
        public string Status { get; set; }
        public string Reason { get; set; }
        public DateTime AppliedDate { get; set; }
        public string ApprovedBy { get; set; }

    }
}
