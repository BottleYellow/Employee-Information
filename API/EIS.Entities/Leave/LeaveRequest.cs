using EIS.Entities.Employee;
using EIS.Entities.Generic;
using System;

namespace EIS.Entities.Leave
{
    public class LeaveRequest:BaseEntity<int>
    {
        public string EmployeeName { get; set; }
        public string LeaveType { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public float RequestedDays { get; set; }
        public float ApprovedDays { get; set; }
        public float Available { get; set; }
        public string Status { get; set; }
        public string Reason { get; set; }
        public DateTime AppliedDate { get; set; }
        public int TypeId { get; set; }
        public int PersonId { get; set; }
        public virtual Person Person { get; set; }
        public LeaveRules TypeOfLeave { get; set; }
    }
}
