

namespace EIS.Repositories.IRepository
{
    public interface IRepositoryWrapper
    {
        IEmployeeRepository Employee { get; }
        ICurrentAddressRepository CurrentAddress { get; }
        IPermanentAddressRepository PermanentAddress { get; }
        IEmergencyAddressRepository EmergencyAddress { get; }
        IOtherAddressRepository OtherAddress { get; }
        IAttendanceRepository Attendance { get; }
        ILeaveRepository Leave { get; }
        ILoginRepository Login { get; }

    }
}
