using System;
using System.Collections.Generic;
using System.Text;

namespace EIS.Entities.Models
{
    public class AttendanceData
    {
        public string Location { get; set; }
        public string Name { get; set; }
        public int PresentDays { get; set; }
        public int OnLeave { get; set; }
        public int TotalWorkingDays { get; set; }
    }
}
