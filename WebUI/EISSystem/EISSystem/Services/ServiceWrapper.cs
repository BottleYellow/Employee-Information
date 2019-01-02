using EIS.Entities.Address;
using EIS.Entities.Employee;
using EIS.Entities.Leave;
using EIS.Entities.User;
using EIS.WebApp.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EIS.WebApp.Services
{
    public class ServiceWrapper : IServiceWrapper
    {
        private IEISService<Person> _employee;

        private IEISService<Role> _roles;

        private IEISService<Current> _currentAddress;

        private IEISService<Permanent> _permanentAddress;

        private IEISService<Emergency> _emergencyAddress;

        private IEISService<Other> _otherAddress;

        private IEISService<Attendance> _attendances;

        private IEISService<LeaveCredit> _leaveCredit;

        private IEISService<LeaveRules> _leaveRules;

        private IEISService<LeaveRequest> _leaveRequest;

        private IEISService<Users> _users;

        public IEISService<Person> Employee
        {
            get
            {
                if (_employee == null)
                {
                    _employee = new EISService<Person>();
                }
                return _employee;
            }
        }

        public IEISService<Current> CurrentAddress
        {
            get
            {
                if (_currentAddress == null)
                {
                    _currentAddress = new EISService<Current>();
                }
                return _currentAddress;
            }
        }
        public IEISService<Permanent> PermanentAddress
        {
            get
            {
                if (_permanentAddress == null)
                {
                    _permanentAddress = new EISService<Permanent>();
                }
                return _permanentAddress;
            }
        }

        public IEISService<Emergency> EmergencyAddress
        {
            get
            {
                if (_emergencyAddress == null)
                {
                    _emergencyAddress = new EISService<Emergency>();
                }
                return _emergencyAddress;
            }
        }

        public IEISService<Other> OtherAddress
        {
            get
            {
                if (_otherAddress == null)
                {
                    _otherAddress = new EISService<Other>();
                }
                return _otherAddress;
            }
        }

        public IEISService<Attendance> Attendances
        {
            get
            {
                if (_attendances == null)
                {
                    _attendances = new EISService<Attendance>();
                }
                return _attendances;
            }
        }

        public IEISService<LeaveCredit> LeaveCredit
        {
            get
            {
                if (_leaveCredit == null)
                {
                    _leaveCredit = new EISService<LeaveCredit>();
                }
                return _leaveCredit;
            }
        }

        public IEISService<LeaveRules> LeaveRules
        {
            get
            {
                if (_leaveRules == null)
                {
                    _leaveRules = new EISService<LeaveRules>();
                }
                return _leaveRules;
            }
        }

        public IEISService<LeaveRequest> LeaveRequest
        {
            get
            {
                if (_leaveRequest == null)
                {
                    _leaveRequest = new EISService<LeaveRequest>();
                }
                return _leaveRequest;
            }
        }

        public IEISService<Users> Users
        {
            get
            {
                if (_users == null)
                {
                    _users = new EISService<Users>();
                }
                return _users;
            }
        }

        public IEISService<Role> Roles
        {
            get
            {
                if (_roles == null)
                {
                    _roles = new EISService<Role>();
                }
                return _roles;
            }
        }
    }
}
