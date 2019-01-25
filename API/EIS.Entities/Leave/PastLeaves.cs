using EIS.Entities.Employee;
using EIS.Entities.Generic;
using System;
using System.Collections.Generic;
using System.Text;

namespace EIS.Entities.Leave
{
    public class PastLeaves : BaseEntity<int>
    {
        public string EmployeeName { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public float RequestedDays { get; set; }
        public string Reason { get; set; }
        public string Status { get; set; }
        public int PersonId { get; set; }
        public virtual Person Person { get; set; }
    }
}
