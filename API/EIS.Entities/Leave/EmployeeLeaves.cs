using EIS.Entities.Employee;
using EIS.Entities.Generic;

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
