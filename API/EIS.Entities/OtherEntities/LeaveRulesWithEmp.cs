using EIS.Entities.Employee;
using EIS.Entities.Enums;
using EIS.Entities.Generic;
using System;
using System.Collections.Generic;

namespace EIS.Entities.Leave
{
    public class LeaveRulesWithEmp:BaseEntity<int>
    {
        public int? LocationId { get; set; }
        public string LeaveType { get; set; }
        public string Description { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public int Validity { get; set; }
        public int[] Employees { get; set; }
    }
}
