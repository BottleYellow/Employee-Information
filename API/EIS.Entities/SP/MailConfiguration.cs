using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace EIS.Entities.SP
{
    public class MailConfiguration
    {
        [Key]
        public string UserId { get; set; }
        public string Password { get; set; }
        public string SMTPPort { get; set; }
        public string Host { get; set; }
    }
}
