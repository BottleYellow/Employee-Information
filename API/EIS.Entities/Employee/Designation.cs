using EIS.Entities.Generic;
using System.Collections.Generic;

namespace EIS.Entities.Employee
{
    public class Designation : BaseEntity<int>
    {
        public string Name { get; set; }
        public string Access { get; set; }
        public virtual ICollection<Person> Persons { get; set; }
    }
}
