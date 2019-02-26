using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace EIS.Entities.SP
{
    public class ActualLeaveCount
    {
        [Key]
        public int LeaveCount { get; set; }
    }
}
