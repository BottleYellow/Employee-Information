using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EIS.WebApp.Models
{
    //avg timeinout present ab tot
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
