using System;
using System.Collections.Generic;
using System.Text;

namespace EIS.Entities.Models
{
   public class LeavePolicyViewModel
    {
        public string LocationName { get; set; }
        public string LeaveType { get; set; }
        public int Validity { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public bool ActiveStatus { get; set; }
        public int Id { get; set; }
    }
}
