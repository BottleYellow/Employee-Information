using EIS.Data.Context;
using EIS.Entities.Address;
using EIS.Entities.Employee;
using EIS.Entities.Leave;
using EIS.Entities.User;
using Microsoft.EntityFrameworkCore;

namespace EIS.Data
{
    public class BuilderExtension : DbContext
    {
        private readonly DbContextOptions<ApplicationDbContext> options;

        public BuilderExtension(DbContextOptions<ApplicationDbContext> _options)
        {
            options = _options;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            try
            {

                #region[Users]
                modelBuilder.Entity<Users>().Property(p => p.UserName).HasColumnType("nvarchar(256)").IsRequired();
                modelBuilder.Entity<Users>().Property(p => p.Password).HasColumnType("nvarchar(max)").IsRequired();
                modelBuilder.Entity<Users>().Property(p => p.PersonId).HasColumnType("int").IsRequired();
                #endregion

                #region[Person]
                //For Employee model validation

                modelBuilder.Entity<Person>().Property(p => p.IdCard).HasColumnType("varchar(15)").IsRequired();
                modelBuilder.Entity<Person>().Property(p => p.PanCard).HasColumnType("varchar(10)");
                modelBuilder.Entity<Person>().Property(p => p.AadharCard).HasColumnType("varchar(12)");
                modelBuilder.Entity<Person>().Property(p => p.Image).HasColumnType("varbinary(MAX)");
                modelBuilder.Entity<Person>().Property(p => p.FirstName).HasColumnType("nvarchar(50)").IsRequired();
                modelBuilder.Entity<Person>().Property(p => p.MiddleName).HasColumnType("nvarchar(50)");
                modelBuilder.Entity<Person>().Property(p => p.LastName).HasColumnType("nvarchar(50)").IsRequired();
                modelBuilder.Entity<Person>().Property(p => p.JoinDate).HasColumnType("date").IsRequired();
                modelBuilder.Entity<Person>().Property(p => p.LeavingDate).HasColumnType("date");
                modelBuilder.Entity<Person>().Property(p => p.Gender).HasColumnType("varchar(15)").IsRequired();
                modelBuilder.Entity<Person>().Property(p => p.MobileNumber).HasColumnType("varchar(15)").IsRequired();
                modelBuilder.Entity<Person>().Property(p => p.DateOfBirth).HasColumnType("date").IsRequired();
                modelBuilder.Entity<Person>().Property(p => p.EmailAddress).HasColumnType("nvarchar(150)").IsRequired();
                modelBuilder.Entity<Person>().Property(p => p.Salary).HasColumnType("float");
                modelBuilder.Entity<Person>().Property(p => p.IsActive).HasColumnType("bit").HasDefaultValue(1);
                modelBuilder.Entity<Person>().Property(p => p.Description).HasColumnType("text");
                modelBuilder.Entity<Person>().Property(p => p.CreatedDate).HasColumnType("date");
                modelBuilder.Entity<Person>().Property(p => p.UpdatedDate).HasColumnType("date");
                modelBuilder.Entity<Person>().Property(p => p.RowVersion).HasColumnType("rowversion").IsConcurrencyToken().ValueGeneratedOnAddOrUpdate();
                #endregion

                #region[LeaveCredit]
                //For LeaveCredit model validation

                
                modelBuilder.Entity<LeaveCredit>().Property(p => p.LeaveType).HasColumnType("varchar(50)").IsRequired();
                modelBuilder.Entity<LeaveCredit>().Property(p => p.AllotedDays).HasColumnType("float").IsRequired();
                modelBuilder.Entity<LeaveCredit>().Property(p => p.ValidTo).HasColumnType("date").IsRequired();
                modelBuilder.Entity<LeaveCredit>().Property(p => p.ValidFrom).HasColumnType("date").IsRequired();
                modelBuilder.Entity<LeaveCredit>().Property(p => p.Available).HasColumnType("float").IsRequired();
                modelBuilder.Entity<LeaveCredit>().Property(p => p.CreatedDate).HasColumnType("datetime");
                modelBuilder.Entity<LeaveCredit>().Property(p => p.UpdatedDate).HasColumnType("datetime");
                modelBuilder.Entity<LeaveCredit>().Property(p => p.RowVersion).HasColumnType("rowversion").IsRowVersion();

                #endregion

                #region[LeavesRequests]
                //For EmployeeLeave model validation
                modelBuilder.Entity<LeaveRequest>().Property(p => p.EmployeeName).HasColumnType("varchar(100)").IsRequired();
                modelBuilder.Entity<LeaveRequest>().Property(p => p.LeaveType).HasColumnType("varchar(50)").IsRequired();
                modelBuilder.Entity<LeaveRequest>().Property(p => p.FromDate).HasColumnType("date").IsRequired();
                modelBuilder.Entity<LeaveRequest>().Property(p => p.ToDate).HasColumnType("date").IsRequired();
                modelBuilder.Entity<LeaveRequest>().Property(p => p.RequestedDays).HasColumnType("float");
                modelBuilder.Entity<LeaveRequest>().Property(p => p.ApprovedDays).HasColumnType("float");
                modelBuilder.Entity<LeaveRequest>().Property(p => p.Available).HasColumnType("float");
                modelBuilder.Entity<LeaveRequest>().Property(p => p.Reason).HasColumnType("varchar(200)");
                modelBuilder.Entity<LeaveRequest>().Property(p => p.Status).HasColumnType("varchar(50)");
                
                modelBuilder.Entity<LeaveRequest>().Property(p => p.AppliedDate).HasColumnType("datetime");
                modelBuilder.Entity<LeaveRequest>().Property(p => p.CreatedDate).HasColumnType("datetime");
                modelBuilder.Entity<LeaveRequest>().Property(p => p.UpdatedDate).HasColumnType("datetime");
                modelBuilder.Entity<LeaveRequest>().Property(p => p.RowVersion).HasColumnType("rowversion").IsRowVersion();
                #endregion

                #region[LeaveRules]
                //For EmployeeLeave model validation

                modelBuilder.Entity<LeaveRules>().Property(p => p.LeaveType).HasColumnType("varchar(50)").IsRequired();
                modelBuilder.Entity<LeaveRules>().Property(p => p.Description).HasColumnType("varchar(200)");
                modelBuilder.Entity<LeaveRules>().Property(p => p.ValidFrom).HasColumnType("date").IsRequired();
                modelBuilder.Entity<LeaveRules>().Property(p => p.ValidTo).HasColumnType("date").IsRequired();
                modelBuilder.Entity<LeaveRules>().Property(p => p.Validity).HasColumnType("float").IsRequired();
                modelBuilder.Entity<LeaveRules>().Property(p => p.CreatedDate).HasColumnType("datetime");
                modelBuilder.Entity<LeaveRules>().Property(p => p.UpdatedDate).HasColumnType("datetime");
                modelBuilder.Entity<LeaveRules>().Property(p => p.RowVersion).HasColumnType("rowversion").IsRowVersion();
                #endregion

                #region[EmployeeLeaves]
                //For EmployeeLeave model validation

                modelBuilder.Entity<EmployeeLeaves>().Property(p => p.AllotedDays).HasColumnType("float").IsRequired();
                modelBuilder.Entity<EmployeeLeaves>().Property(p => p.Available).HasColumnType("float").IsRequired();
                modelBuilder.Entity<EmployeeLeaves>().Property(p => p.CreatedDate).HasColumnType("datetime");
                modelBuilder.Entity<EmployeeLeaves>().Property(p => p.UpdatedDate).HasColumnType("datetime");
                modelBuilder.Entity<EmployeeLeaves>().Property(p => p.RowVersion).HasColumnType("rowversion").IsRowVersion();
                #endregion

                #region[Attendance]
                //For Attendance model validation

                modelBuilder.Entity<Attendance>().Property(p => p.DateIn).HasColumnType("date");
                modelBuilder.Entity<Attendance>().Property(p => p.TimeIn).HasColumnType("time");
                modelBuilder.Entity<Attendance>().Property(p => p.DateOut).HasColumnType("date");
                modelBuilder.Entity<Attendance>().Property(p => p.TimeOut).HasColumnType("time");
                modelBuilder.Entity<Attendance>().Property(p => p.TotalHours).HasColumnType("time");
                modelBuilder.Entity<Attendance>().Property(p => p.CreatedDate).HasColumnType("datetime");
                modelBuilder.Entity<Attendance>().Property(p => p.UpdatedDate).HasColumnType("datetime");
                modelBuilder.Entity<Attendance>().Property(p => p.RowVersion).HasColumnType("rowversion").IsRowVersion();
                #endregion

                #region[Permanent]
                //For EmployeePermanentAddress 

                modelBuilder.Entity<Permanent>().Property(p => p.Address).HasColumnType("nvarchar(400)").IsRequired();
                modelBuilder.Entity<Permanent>().Property(p => p.City).HasColumnType("nvarchar(50)");
                modelBuilder.Entity<Permanent>().Property(p => p.State).HasColumnType("nvarchar(50)");
                modelBuilder.Entity<Permanent>().Property(p => p.Country).HasColumnType("nvarchar(100)");
                modelBuilder.Entity<Permanent>().Property(p => p.PinCode).HasColumnType("varchar(6)").IsRequired();
                modelBuilder.Entity<Permanent>().Property(p => p.PhoneNumber).HasColumnType("varchar(15)").IsRequired();
                modelBuilder.Entity<Permanent>().Property(p => p.CreatedDate).HasColumnType("datetime");
                modelBuilder.Entity<Permanent>().Property(p => p.UpdatedDate).HasColumnType("datetime");
                modelBuilder.Entity<Permanent>().Property(p => p.RowVersion).HasColumnType("rowversion").IsRowVersion();
                #endregion

                #region[Current]
                //For EmployeeCurrentAddress 

                modelBuilder.Entity<Current>().Property(p => p.Address).HasColumnType("nvarchar(400)").IsRequired();
                modelBuilder.Entity<Current>().Property(p => p.City).HasColumnType("nvarchar(50)");
                modelBuilder.Entity<Current>().Property(p => p.State).HasColumnType("nvarchar(50)");
                modelBuilder.Entity<Current>().Property(p => p.Country).HasColumnType("nvarchar(100)");
                modelBuilder.Entity<Current>().Property(p => p.PinCode).HasColumnType("varchar(6)").IsRequired();
                modelBuilder.Entity<Current>().Property(p => p.PhoneNumber).HasColumnType("varchar(15)").IsRequired();
                modelBuilder.Entity<Current>().Property(p => p.CreatedDate).HasColumnType("datetime");
                modelBuilder.Entity<Current>().Property(p => p.UpdatedDate).HasColumnType("datetime");
                modelBuilder.Entity<Current>().Property(p => p.RowVersion).HasColumnType("rowversion").IsRowVersion();
                #endregion

                #region[Emergency]
                //For EmployeeEmergencyAddress

                modelBuilder.Entity<Emergency>().Property(p => p.FirstName).HasColumnType("nvarchar(50)").IsRequired();
                modelBuilder.Entity<Emergency>().Property(p => p.LastName).HasColumnType("nvarchar(50)").IsRequired();
                modelBuilder.Entity<Emergency>().Property(p => p.Address).HasColumnType("nvarchar(400)").IsRequired();
                modelBuilder.Entity<Emergency>().Property(p => p.City).HasColumnType("nvarchar(50)");
                modelBuilder.Entity<Emergency>().Property(p => p.State).HasColumnType("nvarchar(50)");
                modelBuilder.Entity<Emergency>().Property(p => p.Country).HasColumnType("nvarchar(100)");
                modelBuilder.Entity<Emergency>().Property(p => p.PinCode).HasColumnType("varchar(6)").IsRequired();
                modelBuilder.Entity<Emergency>().Property(p => p.Relation).HasColumnType("varchar(30)").IsRequired();
                modelBuilder.Entity<Emergency>().Property(p => p.MobileNumber).HasColumnType("varchar(15)").IsRequired();
                modelBuilder.Entity<Emergency>().Property(p => p.PhoneNumber).HasColumnType("varchar(15)");
                modelBuilder.Entity<Emergency>().Property(p => p.CreatedDate).HasColumnType("datetime");
                modelBuilder.Entity<Emergency>().Property(p => p.UpdatedDate).HasColumnType("datetime");
                modelBuilder.Entity<Emergency>().Property(p => p.RowVersion).HasColumnType("rowversion").IsRowVersion();
                #endregion

                #region[Other]
                //For EmployeeOtherAddress 

                modelBuilder.Entity<Other>().Property(p => p.AddressType).HasColumnType("nvarchar(30)").IsRequired();
                modelBuilder.Entity<Other>().Property(p => p.Address).HasColumnType("nvarchar(400)");
                modelBuilder.Entity<Other>().Property(p => p.City).HasColumnType("nvarchar(50)");
                modelBuilder.Entity<Other>().Property(p => p.State).HasColumnType("nvarchar(50)");
                modelBuilder.Entity<Other>().Property(p => p.Country).HasColumnType("nvarchar(50)");
                modelBuilder.Entity<Other>().Property(p => p.PinCode).HasColumnType("varchar(6)").IsRequired();
                modelBuilder.Entity<Other>().Property(p => p.PhoneNumber).HasColumnType("varchar(15)");
                modelBuilder.Entity<Other>().Property(p => p.CreatedDate).HasColumnType("datetime");
                modelBuilder.Entity<Other>().Property(p => p.UpdatedDate).HasColumnType("datetime");
                modelBuilder.Entity<Other>().Property(p => p.RowVersion).HasColumnType("rowversion").IsRowVersion();
                #endregion

                #region Constraints
               
                modelBuilder.Entity<Person>()
                    .HasIndex(p => p.PanCard)
                    .IsUnique();
                modelBuilder.Entity<Person>()
                    .HasIndex(p => p.AadharCard)
                    .IsUnique();
                modelBuilder.Entity<Person>()
                    .HasIndex(p => p.MobileNumber)
                    .IsUnique();
                modelBuilder.Entity<Person>()
                    .HasIndex(p => p.EmailAddress)
                    .IsUnique();
                modelBuilder.Entity<Person>()
                   .HasIndex(p => new { p.TenantId, p.IdCard })
                   .IsUnique();
                modelBuilder.Entity<Role>()
                     .HasIndex(r => new { r.TenantId, r.Name })
                     .IsUnique();
                modelBuilder.Entity<Users>()
                    .HasIndex(p => new { p.TenantId, p.UserName })
                    .IsUnique();

                #endregion

                #region[Table Relationship]

                //one leave has many credits
                modelBuilder.Entity<LeaveCredit>()
                  .HasOne(s => s.LeaveRule)
                  .WithMany(g => g.Credits)
                  .HasForeignKey(s => s.LeaveId);

                //one Leave Request has one LeaveType
                modelBuilder.Entity<LeaveRequest>()
                   .HasOne(s => s.TypeOfLeave)
                   .WithMany(g => g.Requests)
                   .HasForeignKey(s => s.TypeId);

                //one Leave Request has one LeaveType
                modelBuilder.Entity<LeaveRequest>()
                   .HasOne(s => s.TypeOfLeave)
                   .WithMany(g => g.Requests)
                   .HasForeignKey(s => s.TypeId);

                //one employees has many leave requests
                modelBuilder.Entity<LeaveRequest>()
                  .HasOne(s => s.Person)
                  .WithMany(g => g.LeaveRequests)
                  .HasForeignKey(s => s.PersonId);

                //one employee has many Leave Credits
                modelBuilder.Entity<LeaveCredit>()
                  .HasOne(s => s.Person)
                  .WithMany(g => g.LeaveCredits)
                  .HasForeignKey(s => s.PersonId);

                //one Employee has one EmployeeLeaves
                modelBuilder.Entity<EmployeeLeaves>()
                   .HasOne(s => s.Person)
                   .WithOne(g => g.EmployeeLeaves)
                   .HasForeignKey<EmployeeLeaves>(s => s.PersonId);

                //one employee has many attendance
                modelBuilder.Entity<Attendance>()
                   .HasOne(s => s.Person)
                   .WithMany(g => g.Attendance)
                   .HasForeignKey(g => g.PersonId);

                //one Employee has one Role
                modelBuilder.Entity<Person>()
                   .HasOne(s => s.Role)
                   .WithMany(g => g.Persons)
                   .HasForeignKey(s => s.RoleId);

                //one employee has one permanent address
                modelBuilder.Entity<Permanent>()
                   .HasOne(s => s.Person)
                   .WithOne(g => g.PermanentAddress)
                   .HasForeignKey<Permanent>(s => s.PersonId);

                //one employee has one current address
                modelBuilder.Entity<Current>()
                   .HasOne(s => s.Person)
                   .WithOne(g => g.CurrentAddress)
                   .HasForeignKey<Current>(s => s.PersonId);

                //one employee has many emergency addresses
                modelBuilder.Entity<Emergency>()
                   .HasOne(s => s.Person)
                   .WithMany(g => g.EmergencyAddress)
                   .HasForeignKey(s => s.PersonId);

                //one employee has many other addresses
                modelBuilder.Entity<Other>()
                   .HasOne(s => s.Person)
                   .WithMany(g => g.OtherAddress)
                   .HasForeignKey(s => s.PersonId);

                modelBuilder.Entity<Users>()
                    .HasOne(s => s.Person)
                    .WithOne(g => g.User)
                    .HasForeignKey<Users>(s => s.PersonId);



                #endregion

                #region[Table Schema]
                modelBuilder.Entity<Users>().ToTable("Users", "Account");
                modelBuilder.Entity<Person>().ToTable("Person", "Employee");
                modelBuilder.Entity<Role>().ToTable("Roles", "Employee");
                modelBuilder.Entity<LeaveRules>().ToTable("LeaveRules", "Leave");
                modelBuilder.Entity<LeaveCredit>().ToTable("LeaveCredit", "Leave");
                modelBuilder.Entity<EmployeeLeaves>().ToTable("EmployeeLeaves", "Leave");
                modelBuilder.Entity<LeaveRequest>().ToTable("LeaveRequests", "Leave");
                modelBuilder.Entity<Attendance>().ToTable("Attendance", "Employee");
                modelBuilder.Entity<Permanent>().ToTable("PermanentAddress", "Address");
                modelBuilder.Entity<Current>().ToTable("CurrentAddress", "Address");
                modelBuilder.Entity<Emergency>().ToTable("EmergencyAddress", "Address");
                modelBuilder.Entity<Other>().ToTable("OtherAddress", "Address");
                #endregion
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }
    }
}
