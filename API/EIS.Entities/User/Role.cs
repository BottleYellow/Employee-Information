using EIS.Entities.Generic;
using System;
using System.Collections.Generic;
using System.Text;

namespace EIS.Entities.User
{
    public class Role : BaseEntity<int>
    {
        public string Name { get; set; }
        public string Access { get; set; }
        public UserRoles UserRole { get; set; }
        
    }
}
