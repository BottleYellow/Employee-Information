using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EIS.Entities.Employee;
using EIS.Entities.Address;
namespace EIS.WebApp.Models
{
    public class CreateEmployee
    { 
        public Person people { get; set; }
        public Permanent permanent { get; set; }
        public Current current { get; set; }
        public Emergency emergencyAddress { get; set; }
    }
}
