using EIS.Entities.Employee;
using EIS.Entities.Generic;

namespace EIS.Entities.Address
{
    public class Current:BaseEntity<int>
    {
        public int PersonId { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string PinCode { get; set; }
        public string PhoneNumber { get; set; }
        public virtual Person Person { get; set; }
    }
}
