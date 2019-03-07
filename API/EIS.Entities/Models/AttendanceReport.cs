using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EIS.Entities.Models
{
    public class AttendanceReport
    {
        [Key]
        public string AverageTimeIn { get; set; }
        public string AverageTimeOut { get; set; }
        public string AverageHours { get; set; }
        public string AdditionalWorkingHours { get; set; }
        public int PresentDays { get; set; }
        public int LeaveDays { get; set; }
        public int TotalWorkingDays { get; set; }
        public int TotalDays { get; set; }
    }
    public class EmployeeAttendanceData
    {
        [Key]
        public Nullable<long> SrId { get; set; }
        public Nullable<System.DateTime> DateIn { get; set; }
        public Nullable<System.TimeSpan> TimeIn { get; set; }
        public Nullable<System.TimeSpan> TimeOut { get; set; }
        public Nullable<System.TimeSpan> TotalHours { get; set; }
        public string Status { get; set; }
    }
    public class EmployeeAttendanceReport
    {
        public AttendanceReport _SP_ReportCount { get; set; }
        public List<EmployeeAttendanceData> _SP_AttendanceData { get; set; }
    }

}
