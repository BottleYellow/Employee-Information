using EIS.Entities.Employee;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EIS.WebApp.Models
{
    public class EmployeeAttendance
    {
        public List<Person> Persons { get; set; }
        public List<Attendance> Attendances { get; set; }

    }
}
