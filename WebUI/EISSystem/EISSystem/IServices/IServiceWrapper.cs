using EIS.Entities.Address;
using EIS.Entities.Employee;
using EIS.Entities.Hoildays;
using EIS.Entities.Leave;
using EIS.Entities.User;
using EIS.WebApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EIS.WebApp.IServices
{
    public interface IServiceWrapper
    {
        IEISService<Person> Employee { get; }
        IEISService<Role> Roles { get; }
        IEISService<Current> CurrentAddress { get; }
        IEISService<Permanent> PermanentAddress { get; }
        IEISService<Emergency> EmergencyAddress { get; }
        IEISService<Other> OtherAddress { get; }
        IEISService<Attendance> Attendances { get; }
        IEISService<LeaveCredit> LeaveCredit { get; }
        IEISService<LeaveRules> LeaveRules { get; }
        IEISService<LeaveRequest> LeaveRequest { get; }
        IEISService<Users> Users { get; }
        IEISService<Holiday> Holidays { get; }
    }
}
