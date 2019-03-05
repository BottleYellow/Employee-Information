using EIS.Entities.Employee;
using EIS.Entities.Generic;
using System;
using System.Collections.Generic;
using System.Text;

namespace EIS.Entities.Hoildays
{
    public class WeeklyOffs 
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }
        public bool IsActive { get; set; }
        public virtual ICollection<Person> Employees { get; set; }
    }
}
