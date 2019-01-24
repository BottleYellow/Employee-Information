using EIS.Data.Context;
using EIS.Repositories.IRepository;

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

        private ILeaveRequestRepository _leaveRequest;

        private ILeaveRulesRepository _leaveRules;

        private ILeaveCreditRepository _leaveCredit;

        private IUserRepository _user;

        private IDashboardRepository _dashboard;

        private IHolidayRepository _holiday;


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

        public IAttendanceRepository Attendances
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
        public ILeaveRequestRepository LeaveRequest
        {
            get
            {
                if (_leaveRequest == null)
                {
                    _leaveRequest = new LeaveRequestRepository(_dbContext);
                }
                return _leaveRequest;
            }
        }
        public ILeaveRulesRepository LeaveRules
        {
            get
            {
                if (_leaveRules == null)
                {
                    _leaveRules = new LeaveRulesRepository(_dbContext);
                }
                return _leaveRules;
            }
        }
        public ILeaveCreditRepository LeaveCredit
        {
            get
            {
                if (_leaveCredit == null)
                {
                    _leaveCredit = new LeaveCreditRepository(_dbContext);
                }
                return _leaveCredit;
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

        public IDashboardRepository Dashboard
        {
            get
            {
                if (_dashboard == null)
                {
                    _dashboard = new DashboardRepository(_dbContext);
                }
                return _dashboard;
            }
        }
        public IHolidayRepository Holidays
        {
            get
            {
                if (_holiday == null)
                {
                    _holiday = new HolidayRepository(_dbContext);
                }
                return _holiday;
            }
        }

        public RepositoryWrapper(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

    }
}
