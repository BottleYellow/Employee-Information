using EIS.Data.Context;
using EIS.Repositories.IRepository;

namespace EIS.Repositories.Repository
{
    public class RepositoryWrapper : IRepositoryWrapper
    {
        
        private readonly DbContext _dbContext;

        private IEmployeeRepository _employee;

        private ICurrentAddressRepository _currentAddress;

        private IPermanentAddressRepository _permanentAddress;

        private IEmergencyAddressRepository _emergencyAddress;

        private IAttendanceRepository _attendance;

        private ILeaveRepository _leave;

        private ILoginRepository _login;

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
        public ILoginRepository Login
        {
            get
            {
                if (_login == null)
                {
                    _login = new LoginRepository(_dbContext);
                }
                return _login;
            }
        }
        public RepositoryWrapper(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

    }
}
