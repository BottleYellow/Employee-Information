using EIS.Entities.Generic;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace EIS.Entities.MessageRequest
{
    public class MessageRequest
    {
        public int? PersonId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? IsActive { get; set; }

        [DataType(DataType.Date)]
        public DateTime? RequestedDate { get; set; }
        public string Message { get; set; }
    }
}
