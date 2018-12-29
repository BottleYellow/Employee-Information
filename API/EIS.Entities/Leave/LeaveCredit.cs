using EIS.Entities.Employee;
using EIS.Entities.Generic;
using System;
using System.Collections.Generic;
using System.Text;

namespace EIS.Entities.Leave
{
    public class LeaveCredit : BaseEntity<int>
    {
        public string EmployeeName { get; set; }
        public string LeaveType { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public float Days { get; set; }
        public float Available { get; set; }
        public int PersonId { get; set; }
        public int LeaveId { get; set; }
        public virtual Person Person { get; set; }
        public virtual LeaveMaster LeaveMaster{ get; set; }
    }
}
