using EIS.Entities.Employee;
using EIS.Entities.Generic;

namespace EIS.Entities.Leave
{
    public class EmployeeLeaves:BaseEntity<int>
    {
        public float TotalAlloted { get; set; }
        public float Available { get; set; }
        public int PersonId { get; set; }
        public virtual Person Person { get; set; }
    }
}
