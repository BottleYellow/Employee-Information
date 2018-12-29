using EIS.Entities.Employee;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EIS.WebApp.Models
{
    public class EmployeeAttendance
    {
        public Person Persons { get; set; }
        public List<Attendance> Attendances { get; set; }

    }

    public class EmployeeAttendanceById
    {
        public Person Persons { get; set; }
        public List<Attendance> Attendances { get; set; }

    }
}
