using EIS.Entities.Generic;
using System;
using System.Collections.Generic;
using System.Text;

namespace EIS.Entities.Employee
{
    public class Designation : BaseEntity<int>
    {
        public string Name { get; set; }
        public string Access { get; set; }
        public virtual ICollection<Person> Persons { get; set; }
    }
}
