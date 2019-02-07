using EIS.Entities.Generic;
using System;
using System.Collections.Generic;
using System.Text;

namespace EIS.Entities.Employee
{
    public class Locations:BaseEntity<int>
    {
        public string LocationName { get; set; }

        public virtual ICollection<Person> Employees { get; set; }
    }
}
