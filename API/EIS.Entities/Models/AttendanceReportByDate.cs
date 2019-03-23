

using System;

namespace EIS.Entities.Models
{
   public class AttendanceReportByDate
    {
        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }
        public string Date { get; set; }
        public string Status { get; set; }
        public string TimeIn { get; set; }
        public string  TimeOut { get; set; }
        public string TotalHours { get; set; }
    }

    public class AttendanceUpdateData
    {
        public int personId { get; set; }
        public string EmployeeName { get; set; }
        public DateTime DateIn { get; set; }
        public TimeSpan TimeIn { get; set; }
        public TimeSpan TimeOut { get; set; }
        public string Message { get; set; }
        public TimeSpan WorkingHours { get; set; }
    }
}