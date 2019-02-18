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
}
