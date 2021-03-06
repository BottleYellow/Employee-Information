﻿using EIS.Entities.Address;
using EIS.Entities.Enums;
using EIS.Entities.Generic;
using EIS.Entities.Hoildays;
using EIS.Entities.Leave;
using EIS.Entities.User;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EIS.Entities.Employee
{
    public class Person : BaseEntity<int>
    {
        #region [Entities]
        public string EmployeeCode { get; set; }
        public int? LocationId { get; set; }
        public int? StreamId { get; set; }
        public string PanCard { get; set; }
        public string AadharCard { get; set; }
        public string Image { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }

        [DataType(DataType.Date)]
        public DateTime JoinDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? LeavingDate { get; set; }
        public string MobileNumber { get; set; }

        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }
        public string EmailAddress { get; set; }
        public float? Salary { get; set; }
        public string Description { get; set; }
        public int RoleId { get; set; }
        public int ReportingPersonId { get; set; }
        public TimeSpan? WorkingHours { get; set; }
        public int? WeeklyOffId { get; set; }
        public bool? IsOnProbation { get; set; }
        public int? PropbationPeriodInMonth { get; set; }
        public string PersonalEmail { get; set; }
        public string ContactNumber { get; set; }
        #endregion

        #region [Relations]
        public virtual Users User { get; set; }
        public virtual WeeklyOffs WeeklyOff { get; set; }
        public virtual Gender Gender { get; set; }
        public virtual Role Role { get; set; }
        public virtual ICollection<LeaveRequest> LeaveRequests { get; set; }
        public virtual EmployeeLeaves EmployeeLeaves { get; set; }
        public virtual ICollection<Attendance> Attendance { get; set; }
        public virtual ICollection<LeaveCredit> LeaveCredits { get; set; }
        public virtual Permanent PermanentAddress { get; set; }
        public virtual Locations Location { get; set; }
        public virtual Current CurrentAddress { get; set; }
        public virtual ICollection<Emergency> EmergencyAddress { get; set; }
        public virtual ICollection<Other> OtherAddress { get; set; }
        public virtual ICollection<PastLeaves> PastLeaves { get; set; }
        #endregion
        [NotMapped]
        public string FullName {
            get
            {
                if(string.IsNullOrEmpty(MiddleName))
                {
                    return FirstName + " " + LastName;
                }
                return FirstName+" " + MiddleName+" " + LastName;
            }
        }
    }
    public class TestModel
    {
        public Person[] people;
        public int count;
    }
}
