using EIS.Entities.Employee;
using EIS.Entities.Generic;
using System;

namespace EIS.Entities.Leave
{
    public class LeaveCredit : BaseEntity<int>
    {
        public string LeaveType { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public float AllotedDays { get; set; }
        public float Available { get; set; }
        public int PersonId { get; set; }
        public int LeaveId { get; set; }
        public virtual Person Person { get; set; }
        public virtual LeaveRules LeaveRule{ get; set; }
    }
}
