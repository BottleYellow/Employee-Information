using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace EIS.Entities.Models
{
   public class AttendanceReportByDate
    {
        public string Date { get; set; }
        public string Status { get; set; }
        public string TimeIn { get; set; }
        public string  TimeOut { get; set; }
        public string TotalHours { get; set; }
    }
}