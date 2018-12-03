﻿using EIS.Data.Context;
using EIS.Entities.Address;
using EIS.Entities.Employee;
using EIS.Entities.User;
using Microsoft.EntityFrameworkCore;

namespace EIS.Data
{
    public class BuilderExtension : DbContext
    {
        private readonly DbContextOptions<ApplicationDbContext> options;

        public BuilderExtension(DbContextOptions<ApplicationDbContext> options)
        {
            this.options = options;
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
                modelBuilder.Entity<Person>().Property(p => p.Designation).HasColumnType("nvarchar(50)").IsRequired();
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

                #region[Leaves]
                //For EmployeeLeave model validation

                modelBuilder.Entity<Leaves>().Property(p => p.LeaveTypes).HasColumnType("varchar(30)");
                modelBuilder.Entity<Leaves>().Property(p => p.LeavesAlloted).HasColumnType("float").IsRequired();
                modelBuilder.Entity<Leaves>().Property(p => p.LeavesAvailed).HasColumnType("float").IsRequired();
                modelBuilder.Entity<Leaves>().Property(p => p.CreatedDate).HasColumnType("datetime");
                modelBuilder.Entity<Leaves>().Property(p => p.UpdatedDate).HasColumnType("datetime");
                modelBuilder.Entity<Leaves>().Property(p => p.RowVersion).HasColumnType("rowversion").IsRowVersion(); ;
                #endregion

                #region[Attendance]
                //For Attendance model validation

                modelBuilder.Entity<Attendance>().Property(p => p.DateIn).HasColumnType("date").IsRequired();
                modelBuilder.Entity<Attendance>().Property(p => p.TimeIn).HasColumnType("time").IsRequired();
                modelBuilder.Entity<Attendance>().Property(p => p.DateOut).HasColumnType("date").IsRequired();
                modelBuilder.Entity<Attendance>().Property(p => p.TimeOut).HasColumnType("time").IsRequired();
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

                #region[Roles]
                modelBuilder.Entity<Role>().Property(p => p.Id).HasColumnType("int").IsRequired();
                modelBuilder.Entity<Role>().Property(p => p.Name).HasColumnType("nvarchar(256)").IsRequired();
                modelBuilder.Entity<Role>().Property(p => p.Access).HasColumnType("nvarchar(max)").IsRequired();
                #endregion

                #region[User Roles]
                modelBuilder.Entity<UserRoles>().Property(p => p.UserId).HasColumnType("int").IsRequired();
                modelBuilder.Entity<UserRoles>().Property(p => p.RoleId).HasColumnType("int").IsRequired();
                #endregion

                #region[Table Relationship]
                //one employees has many leaves
                modelBuilder.Entity<Leaves>()
                  .HasOne(s => s.Person)
                  .WithMany(g => g.Leaves)
                  .HasForeignKey(s => s.PersonId);

                //one employee has many attendance
                modelBuilder.Entity<Attendance>()
                   .HasOne(s => s.Person)
                   .WithMany(g => g.Attendance)
                   .HasForeignKey(g => g.PersonId);

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

                modelBuilder.Entity<UserRoles>()
                    .HasOne(u => u.Role)
                    .WithOne(r => r.UserRole)
                    .HasForeignKey<UserRoles>(u => u.RoleId);

                modelBuilder.Entity<UserRoles>()
                    .HasOne(u => u.User)
                    .WithOne(u1 => u1.Role)
                    .HasForeignKey<UserRoles>(u => u.UserId);



                #endregion

                #region[Table Schema]
                modelBuilder.Entity<Users>().ToTable("Users", "Account");
                modelBuilder.Entity<UserRoles>().ToTable("UserRoles", "Account");
                modelBuilder.Entity<Role>().ToTable("Role", "Account");
                modelBuilder.Entity<AccessToken>().ToTable("Tokens", "Account");
                modelBuilder.Entity<Person>().ToTable("Person", "Employee");
                modelBuilder.Entity<Leaves>().ToTable("Leaves", "Employee");
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
