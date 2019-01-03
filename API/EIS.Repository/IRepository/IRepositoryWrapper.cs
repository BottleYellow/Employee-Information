
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
        ILeaveRepository Leave { get; }
        IUserRepository Users { get; }
        
    }
}
