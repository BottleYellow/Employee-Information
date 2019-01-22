using System;
using System.Collections.Generic;
using System.Text;

namespace EIS.Entities.Dashboard
{
    public class ManagerDashboard
    {
        public int MonthlyPresentDays { get; set; }
        public int MonthlyAbsentDays { get; set; }
        public int TotalRequestPending { get; set; }
        public int TotalLeavesAvailable { get; set; }
    }
}
