using EIS.Entities.Employee;
using EIS.Entities.Generic;
using System;
using System.Collections.Generic;
using System.Text;

namespace EIS.Entities.Leave
{
    public class EmployeeLeaves:BaseEntity<int>
    {
        public string LeaveType { get; set; }
        public float AllotedDays { get; set; }
        public float Available { get; set; }
        public int PersonId { get; set; }
        public int TypeId { get; set; }
        public virtual Person Person { get; set; }
        public LeaveRules TypeOfLeave { get; set; }
    }
}
