
namespace EIS.Entities.Models
{
    public class AttendanceReport
    {
            public string AverageTime { get; set; }
            public string TimeIn { get; set; }
            public string TimeOut { get; set; }
            public int PresentDays { get; set; }
            public int AbsentDays { get; set; }
            public int TotalDays { get; set; }
    }
}
