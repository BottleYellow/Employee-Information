
namespace EIS.Repositories.IRepository
{
    public interface IRepositoryWrapper
    {
        IEmployeeRepository Employee { get; }
        ICurrentAddressRepository CurrentAddress { get; }
        IPermanentAddressRepository PermanentAddress { get; }
        IEmergencyAddressRepository EmergencyAddress { get; }
        IOtherAddressRepository OtherAddress { get; }
        IAttendanceRepository Attendances { get; }
        ILeaveRequestRepository LeaveRequest { get; }
        ILeaveRulesRepository LeaveRules { get; }
        ILeaveCreditRepository LeaveCredit { get; }
        IUserRepository Users { get; }
        IDashboardRepository Dashboard{ get; }
        IHolidayRepository Holidays { get; }
        ILocationRepository Locations { get; }
        IPastLeavesRepository PastLeaves { get; }

    }
}
