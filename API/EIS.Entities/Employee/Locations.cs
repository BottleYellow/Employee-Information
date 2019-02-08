using EIS.Entities.Generic;
using EIS.Entities.Hoildays;
using EIS.Entities.Leave;
using System;
using System.Collections.Generic;
using System.Text;

namespace EIS.Entities.Employee
{
    public class Locations:BaseEntity<int>
    {
        public string LocationName { get; set; }
        public virtual ICollection<Person> Employees { get; set; }
        public virtual ICollection<LeaveRules> LeaveRules { get; set; }
        public virtual ICollection<Holiday> Holidays { get; set; }
    }
}
