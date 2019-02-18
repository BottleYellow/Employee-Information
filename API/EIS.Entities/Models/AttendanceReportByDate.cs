

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
}