using EIS.Entities.Employee;
using System.Collections.Generic;

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
