using EIS.Entities.Generic;
using System;
using System.Collections.Generic;
using System.Text;

namespace EIS.Entities.User
{
    public class UserRoles : BaseEntity<int>
    {
        public int UserId { get; set; }
        public string RoleId { get; set; }
    }
}
