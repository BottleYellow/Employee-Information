using EIS.Entities.Address;
using EIS.Entities.Enums;
using EIS.Entities.Generic;
using EIS.Entities.Leave;
using EIS.Entities.User;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace EIS.Entities.Employee
{
    public class Person : BaseEntity<int>
    {
        #region [Entities]
        public string IdCard { get; set; }
        public string PanCard { get; set; }
        public string AadharCard { get; set; }
        public byte[] Image { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }

        [DataType(DataType.Date)]
        public DateTime JoinDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime LeavingDate { get; set; }
        public string MobileNumber { get; set; }

        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }
        public string EmailAddress { get; set; }
        public float Salary { get; set; }
        public string Description { get; set; }
        public int DesignationId { get; set; }
        public int ReportingPersonId { get; set; }
        #endregion

        #region [Relations]
        public virtual Users User { get; set; }
        
        public virtual Gender Gender { get; set; }
        public virtual Designation Designation { get; set; }
        public virtual ICollection<LeaveRequest> LeaveRequests { get; set; }
        public virtual EmployeeLeaves EmployeeLeaves { get; set; }
        public virtual ICollection<Attendance> Attendance { get; set; }
        public virtual ICollection<LeaveCredit> LeaveCredits { get; set; }
        public virtual Permanent PermanentAddress { get; set; }
        public virtual Current CurrentAddress { get; set; }
        public virtual ICollection<Emergency> EmergencyAddress { get; set; }
        public virtual ICollection<Other> OtherAddress { get; set; }
        #endregion
    }
}
