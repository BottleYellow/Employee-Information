using EIS.Entities.Generic;
using System;
using System.Collections.Generic;
using System.Text;

namespace EIS.Entities.Admin
{
    public class Login : BaseEntity<int>
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public int PersonId { get; set; }
        public string Role { get; set; }

    }
}
