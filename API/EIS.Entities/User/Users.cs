using EIS.Entities.Employee;
using EIS.Entities.Generic;

namespace EIS.Entities.User
{
    public class Users : BaseEntity<int>
    {
        public string UserName { get; set; }
        public bool EmailConfirmed { get; set; }
        public string Password { get; set; }
        public bool PhoneConfirmed { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public int AccessFailedCount { get; set; }
        public int PersonId { get; set; }
        public virtual Person Person { get; set; }
    }
}
