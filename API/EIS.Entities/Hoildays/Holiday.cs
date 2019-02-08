using EIS.Entities.Employee;
using EIS.Entities.Generic;
using System;
using System.Collections.Generic;
using System.Text;

namespace EIS.Entities.Hoildays
{
    public class Holiday:BaseEntity<int>
    {
        public int? LocationId { get; set; }
        public DateTime Date { get; set; }
        public string Vacation { get; set; }
        public virtual Locations Location { get; set; }
    }
}
