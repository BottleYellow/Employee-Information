using EIS.Entities.Generic;
using System;
using System.Collections.Generic;

namespace EIS.Entities.Leave
{
    public class LeaveMaster:BaseEntity<int>
    {
        public string LeaveType { get; set; }
        public string Description { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public int Days { get; set; }
        public virtual ICollection<LeaveRequest> Requests { get; set; }
        public virtual ICollection<LeaveCredit> Credits { get; set; }
    }
}
