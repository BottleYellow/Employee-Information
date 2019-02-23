using System;
using System.Collections.Generic;
using System.Text;

namespace EIS.Entities.Models
{
   public class LeaveCreditViewModel
    {
        public string LocationName { get; set; }
        public string FullName { get; set; }
        public string LeaveType { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public int AllotedDays { get; set; }
        public int Available { get; set; }
        public bool ActiveStatus { get; set; }
        public int Id { get; set; }

    }
}
