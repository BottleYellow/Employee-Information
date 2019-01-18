using EIS.Entities.Employee;
using EIS.Entities.Generic;

namespace EIS.Entities.Address
{
    public class Emergency:BaseEntity<int>
    {
        #region[EmergencyAddress]
        public int PersonId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string PinCode { get; set; }
        public string Relation { get; set; }
        public string MobileNumber { get; set; }
        public string PhoneNumber { get; set; }
        #endregion
        public virtual Person Person { get; set; }
    }
}
