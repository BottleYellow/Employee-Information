using System;
using System.Collections.Generic;
using System.Text;

namespace EIS.Entities.Models
{
   public class LeaveRequestViewModel
    {
        public int Id { get; set; }
        public string LocationName { get; set; }
        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeRole { get; set; }
        public string LeaveType { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public float RequestedDays { get; set; }
        public float Available { get; set; }
        public DateTime AppliedDate { get; set; }
        public string Status { get; set; }
        public string ApprovedName { get; set; }
        public string Reason { get; set; }

    }
}
