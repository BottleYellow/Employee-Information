using EIS.Entities.Enums;
using EIS.Entities.Generic;
using System;
using System.Collections.Generic;
using System.Text;

namespace EIS.Entities.Employee
{
    public class Leaves : BaseEntity<int>
    {
        public int PersonId { get; set; }
        public float LeavesAlloted { get; set; }
        public float LeavesAvailed { get; set; }
        public virtual Person Person { get; set; }
        public virtual LeaveType LeaveTypes { get; set; }
    }
}
