using EIS.Entities.Address;
using EIS.Entities.Employee;
using EIS.Entities.Hoildays;
using EIS.Entities.Leave;
using EIS.Entities.Models;
using EIS.Entities.OtherEntities;
using EIS.Entities.SP;
using EIS.Entities.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;

namespace EIS.Data.Context
{
    public class ApplicationDbContext : BuilderExtension
    {
        private readonly string connectionString;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
       : base(options)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(connectionString, options =>
             {
                 options.UseRowNumberForPaging();
             });
        }

        #region[Tables]
        [NotMapped]
        public virtual DbSet<SP_AdminDashboard> _sp_AdminDashboard { get; set; }
        [NotMapped]
        public virtual DbSet<SP_AdminDashboardCount> _sp_AdminDashboardcount { get; set; }
        [NotMapped]
        public virtual DbSet<Sp_AdminDashboardLeave> _sp_AdminDashboardLeave { get; set; }
        [NotMapped]
        public virtual DbSet<SP_EmployeeDashboard> _sp_EmployeeDashboard { get; set; }
        [NotMapped]
        public virtual DbSet<SP_EmployeeDashboardCount> _sp_EmployeeDashboardcount { get; set; }
        [NotMapped]
        public virtual DbSet<SP_GetAttendanceCountReport> _sp_GetAttendanceCountReport { get; set; }
        [NotMapped]
        public virtual DbSet<AttendanceReport> _sp_GetEmployeeAttendanceCount { get; set; }
        [NotMapped]
        public virtual DbSet<EmployeeAttendanceData> _sp_GetEmployeeAttendanceData { get; set; }
        [NotMapped]
        public virtual DbSet<GetAdminHrManager> _sp_GetAdminHrManager { get; set; }
        [NotMapped]
        public virtual DbSet<MailConfiguration> _sp_MailConfigurations { get; set; }
        [NotMapped]
        public virtual DbSet<SP_GetEmployee> _sp_GetEmployee { get; set; }
        [NotMapped]
        public virtual DbSet<ActualLeaveCount> _sp_GetLeaveCount { get; set; }
        [NotMapped]
        public virtual DbSet<SP_GetDateWiseAttendance> _sp_GetDateWiseAttendances { get; set; }

        public DbSet<Users> Users { get; set; }
        public DbSet<Person> Person { get; set; }
        public DbSet<LeaveRules> LeaveRules { get; set; }
        public DbSet<LeaveRequest> LeaveRequests { get; set; }
        public DbSet<LeaveCredit> LeaveCredit { get; set; }
        public DbSet<EmployeeLeaves> EmployeeLeaves { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<Permanent> PermanentAddresses { get; set; }
        public DbSet<Current> CurrentAddresses { get; set; }
        public DbSet<Emergency> EmergencyAddresses { get; set; }
        public DbSet<Other> OtherAddresses { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Configuration> Configurations { get; set; }
        public DbSet<Holiday> Holidays { get; set; }
        public DbSet<PastLeaves> PastLeaves { get; set; }
        public DbSet<Locations> Locations { get; set; }
        #endregion
    }
}
