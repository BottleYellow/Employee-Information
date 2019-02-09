using EIS.Entities.Employee;
using EIS.Entities.Leave;
using System;
using System.Collections.Generic;
using System.Text;

namespace EIS.Entities.Dashboard
{
    public class AdminDashboard
    {
        public int AllEmployeesCount { get; set; }
        public int PendingLeavesCount { get; set; }
        public int PresentEmployees { get; set; }
        public int AbsentEmployees { get; set; }
        public IEnumerable<Person> Employees { get; set; }
        public IEnumerable<LeaveRequest> LeaveRequests { get; set; }
    }
}
