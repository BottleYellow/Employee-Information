﻿using EIS.Entities.Employee;
using EIS.Entities.Generic;
using System;
using System.Collections.Generic;
using System.Text;

namespace EIS.Entities.Address
{
    public class Other : BaseEntity<int>
    {
        #region[OtherAddress]
        public int PersonId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string PinCode { get; set; }
        public string PhoneNumber { get; set; }
        #endregion
        public virtual Person Person { get; set; }
    }
}
