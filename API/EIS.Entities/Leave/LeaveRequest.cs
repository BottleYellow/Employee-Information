﻿using EIS.Entities.Employee;
using EIS.Entities.Enums;
using EIS.Entities.Generic;
using System;
using System.Collections.Generic;
using System.Text;

namespace EIS.Entities.Leave
{
    public class LeaveRequest:BaseEntity<int>
    {
        public string EmployeeName { get; set; }
        public string LeaveType { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public float TotalRequestedDays { get; set; }
        public float Available { get; set; }
        public string Status { get; set; }
        public string Reason { get; set; }
        public DateTime AppliedDate { get; set; }
        public int TypeId { get; set; }
        public int PersonId { get; set; }
        public virtual Person Person { get; set; }
        public LeaveMaster TypeOfLeave { get; set; }
    }
}
