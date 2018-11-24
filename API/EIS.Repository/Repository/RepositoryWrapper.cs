using EIS.Data.Context;
using EIS.Repositories.IRepository;
using Microsoft.Extensions.Options;

namespace EIS.Repositories.Repository
{
    public class RepositoryWrapper : IRepositoryWrapper
    {
        private readonly ApplicationDbContext _dbContext;

        private IEmployeeRepository _employee;

        private ICurrentAddressRepository _currentAddress;

        private IPermanentAddressRepository _permanentAddress;

        private IEmergencyAddressRepository _emergencyAddress;

        private IOtherAddressRepository _otherAddress;

        private IAttendanceRepository _attendance;

        private ILeaveRepository _leave;

        private IUserRepository _user;

        IRoleRepository _roleManager;

     public IEmployeeRepository Employee 
        {
            get {
                if (_employee == null)
                {
                    _employee = new EmployeeRepository(_dbContext);
                }
                return _employee;
            }
        }

        public ICurrentAddressRepository CurrentAddress
        {
            get
            {
                if(_currentAddress==null)
                {
                    _currentAddress = new CurrentAddressRepository(_dbContext);
                }
                return _currentAddress;
            }
        }
        public IPermanentAddressRepository PermanentAddress
        {
            get
            {
                if (_permanentAddress == null)
                {
                    _permanentAddress = new PermanentAddressRepository(_dbContext);
                }
                return _permanentAddress;
            }
        }
        public IEmergencyAddressRepository EmergencyAddress
        {
            get
            {
                if (_emergencyAddress == null)
                {
                    _emergencyAddress = new EmergencyAddressRepository(_dbContext);
                }
                return _emergencyAddress;
            }
        }
        public IOtherAddressRepository OtherAddress
        {
            get
            {
                if (_otherAddress == null)
                {
                    _otherAddress = new OtherAddressRepository(_dbContext);
                }
                return _otherAddress;
            }
        }

        public IAttendanceRepository Attendance
        {
            get
            {
                if(_attendance== null)
                {
                    _attendance = new AttendanceRepository(_dbContext);
                }
                return _attendance;
            }
        }
        public ILeaveRepository Leave
        {
            get
            {
                if (_leave == null)
                {
                    _leave = new LeaveRepository(_dbContext);
                }
                return _leave;
            }
        }
        public IUserRepository Users
        {
            get
            {
                if (_user == null)
                {
                    _user = new UserRepository(_dbContext);
                }
                return _user;
            }
        }
        public IRoleRepository RoleManager
        {
            get
            {
                if (_roleManager == null)
                {
                    _roleManager = new RoleRepository(_dbContext);
                }
                return _roleManager;
            }
        }
        public RepositoryWrapper(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

    }
}
