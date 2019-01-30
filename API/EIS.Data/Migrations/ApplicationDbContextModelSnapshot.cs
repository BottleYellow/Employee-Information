﻿// <auto-generated />
using System;
using EIS.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EIS.Data.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.0-rtm-35687")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("EIS.Entities.Address.Current", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasColumnType("nvarchar(400)");

                    b.Property<string>("City")
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Country")
                        .HasColumnType("nvarchar(100)");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime");

                    b.Property<bool>("IsActive");

                    b.Property<int>("PersonId");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasColumnType("varchar(15)");

                    b.Property<string>("PinCode")
                        .IsRequired()
                        .HasColumnType("varchar(6)");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("rowversion");

                    b.Property<string>("State")
                        .HasColumnType("nvarchar(50)");

                    b.Property<int>("TenantId");

                    b.Property<DateTime>("UpdatedDate")
                        .HasColumnType("datetime");

                    b.HasKey("Id");

                    b.HasIndex("PersonId")
                        .IsUnique();

                    b.ToTable("tblCurrentAddress","LMS");
                });

            modelBuilder.Entity("EIS.Entities.Address.Emergency", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasColumnType("nvarchar(400)");

                    b.Property<string>("City")
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Country")
                        .HasColumnType("nvarchar(100)");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("nvarchar(50)");

                    b.Property<bool>("IsActive");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("MobileNumber")
                        .IsRequired()
                        .HasColumnType("varchar(10)");

                    b.Property<int>("PersonId");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("varchar(15)");

                    b.Property<string>("PinCode")
                        .IsRequired()
                        .HasColumnType("varchar(6)");

                    b.Property<string>("Relation")
                        .IsRequired()
                        .HasColumnType("varchar(30)");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("rowversion");

                    b.Property<string>("State")
                        .HasColumnType("nvarchar(50)");

                    b.Property<int>("TenantId");

                    b.Property<DateTime>("UpdatedDate")
                        .HasColumnType("datetime");

                    b.HasKey("Id");

                    b.HasIndex("PersonId");

                    b.ToTable("tblEmergencyAddress","LMS");
                });

            modelBuilder.Entity("EIS.Entities.Address.Other", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Address")
                        .HasColumnType("nvarchar(400)");

                    b.Property<string>("AddressType")
                        .IsRequired()
                        .HasColumnType("nvarchar(30)");

                    b.Property<string>("City")
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Country")
                        .HasColumnType("nvarchar(50)");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime");

                    b.Property<bool>("IsActive");

                    b.Property<int>("PersonId");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("varchar(15)");

                    b.Property<string>("PinCode")
                        .IsRequired()
                        .HasColumnType("varchar(6)");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("rowversion");

                    b.Property<string>("State")
                        .HasColumnType("nvarchar(50)");

                    b.Property<int>("TenantId");

                    b.Property<DateTime>("UpdatedDate")
                        .HasColumnType("datetime");

                    b.HasKey("Id");

                    b.HasIndex("PersonId");

                    b.ToTable("tblOtherAddress","LMS");
                });

            modelBuilder.Entity("EIS.Entities.Address.Permanent", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasColumnType("nvarchar(400)");

                    b.Property<string>("City")
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Country")
                        .HasColumnType("nvarchar(100)");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime");

                    b.Property<bool>("IsActive");

                    b.Property<int>("PersonId");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasColumnType("varchar(15)");

                    b.Property<string>("PinCode")
                        .IsRequired()
                        .HasColumnType("varchar(6)");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("rowversion");

                    b.Property<string>("State")
                        .HasColumnType("nvarchar(50)");

                    b.Property<int>("TenantId");

                    b.Property<DateTime>("UpdatedDate")
                        .HasColumnType("datetime");

                    b.HasKey("Id");

                    b.HasIndex("PersonId")
                        .IsUnique();

                    b.ToTable("tblPermanentAddress","LMS");
                });

            modelBuilder.Entity("EIS.Entities.Employee.Attendance", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime");

                    b.Property<DateTime>("DateIn")
                        .HasColumnType("date");

                    b.Property<DateTime>("DateOut")
                        .HasColumnType("date");

                    b.Property<bool>("IsActive");

                    b.Property<int>("PersonId");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("rowversion");

                    b.Property<int>("TenantId");

                    b.Property<TimeSpan>("TimeIn")
                        .HasColumnType("time");

                    b.Property<TimeSpan>("TimeOut")
                        .HasColumnType("time");

                    b.Property<TimeSpan>("TotalHours")
                        .HasColumnType("time");

                    b.Property<DateTime>("UpdatedDate")
                        .HasColumnType("datetime");

                    b.HasKey("Id");

                    b.HasIndex("PersonId");

                    b.ToTable("tblAttendance","LMS");
                });

            modelBuilder.Entity("EIS.Entities.Employee.Person", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("AadharCard")
                        .HasColumnType("varchar(12)");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("date");

                    b.Property<DateTime>("DateOfBirth")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("date")
                        .HasDefaultValueSql("getdate()");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<string>("EmailAddress")
                        .IsRequired()
                        .HasColumnType("nvarchar(150)");

                    b.Property<string>("EmployeeCode")
                        .IsRequired()
                        .HasColumnType("nvarchar(30)");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Gender")
                        .IsRequired()
                        .HasColumnType("varchar(15)");

                    b.Property<string>("Image")
                        .HasColumnType("varchar(max)");

                    b.Property<bool>("IsActive")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(true);

                    b.Property<DateTime>("JoinDate")
                        .HasColumnType("date");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("nvarchar(50)");

                    b.Property<DateTime>("LeavingDate")
                        .HasColumnType("date");

                    b.Property<string>("MiddleName")
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("MobileNumber")
                        .IsRequired()
                        .HasColumnType("varchar(10)");

                    b.Property<string>("PanCard")
                        .HasColumnType("varchar(10)");

                    b.Property<int>("ReportingPersonId");

                    b.Property<int>("RoleId");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("rowversion");

                    b.Property<double?>("Salary")
                        .HasColumnType("float");

                    b.Property<int>("StreamId")
                        .HasColumnType("int");

                    b.Property<int>("TenantId");

                    b.Property<DateTime>("UpdatedDate")
                        .HasColumnType("date");

                    b.HasKey("Id");

                    b.HasIndex("EmailAddress")
                        .IsUnique();

                    b.HasIndex("MobileNumber")
                        .IsUnique();

                    b.HasIndex("RoleId");

                    b.HasIndex("TenantId", "EmployeeCode")
                        .IsUnique();

                    b.ToTable("tblPerson","LMS");
                });

            modelBuilder.Entity("EIS.Entities.Employee.Role", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Access");

                    b.Property<DateTime>("CreatedDate");

                    b.Property<bool>("IsActive");

                    b.Property<string>("Name");

                    b.Property<int>("TenantId");

                    b.Property<DateTime>("UpdatedDate");

                    b.HasKey("Id");

                    b.HasIndex("TenantId", "Name")
                        .IsUnique()
                        .HasFilter("[Name] IS NOT NULL");

                    b.ToTable("tblRoles","LMS");
                });

            modelBuilder.Entity("EIS.Entities.Hoildays.Holiday", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("CreatedDate");

                    b.Property<DateTime>("Date")
                        .HasColumnType("date");

                    b.Property<bool>("IsActive");

                    b.Property<string>("Location")
                        .IsRequired()
                        .HasColumnType("nvarchar(50)");

                    b.Property<int>("TenantId");

                    b.Property<DateTime>("UpdatedDate");

                    b.Property<string>("Vacation")
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("Id");

                    b.ToTable("tblHolidays","LMS");
                });

            modelBuilder.Entity("EIS.Entities.Leave.EmployeeLeaves", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<double>("AllotedDays")
                        .HasColumnType("float");

                    b.Property<double>("Available")
                        .HasColumnType("float");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime");

                    b.Property<bool>("IsActive");

                    b.Property<string>("LeaveType");

                    b.Property<int>("PersonId");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("rowversion");

                    b.Property<int>("TenantId");

                    b.Property<int>("TypeId");

                    b.Property<int?>("TypeOfLeaveId");

                    b.Property<DateTime>("UpdatedDate")
                        .HasColumnType("datetime");

                    b.HasKey("Id");

                    b.HasIndex("PersonId")
                        .IsUnique();

                    b.HasIndex("TypeOfLeaveId");

                    b.ToTable("tblEmployeeLeaves","LMS");
                });

            modelBuilder.Entity("EIS.Entities.Leave.LeaveCredit", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<double>("AllotedDays")
                        .HasColumnType("float");

                    b.Property<double>("Available")
                        .HasColumnType("float");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime");

                    b.Property<bool>("IsActive");

                    b.Property<int>("LeaveId");

                    b.Property<string>("LeaveType")
                        .IsRequired()
                        .HasColumnType("varchar(50)");

                    b.Property<int>("PersonId");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("rowversion");

                    b.Property<int>("TenantId");

                    b.Property<DateTime>("UpdatedDate")
                        .HasColumnType("datetime");

                    b.Property<DateTime>("ValidFrom")
                        .HasColumnType("date");

                    b.Property<DateTime>("ValidTo")
                        .HasColumnType("date");

                    b.HasKey("Id");

                    b.HasIndex("LeaveId");

                    b.HasIndex("PersonId");

                    b.ToTable("tblLeaveCredit","LMS");
                });

            modelBuilder.Entity("EIS.Entities.Leave.LeaveRequest", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("AppliedDate")
                        .HasColumnType("datetime");

                    b.Property<double>("ApprovedDays")
                        .HasColumnType("float");

                    b.Property<double>("Available")
                        .HasColumnType("float");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime");

                    b.Property<string>("EmployeeName")
                        .IsRequired()
                        .HasColumnType("varchar(100)");

                    b.Property<DateTime>("FromDate")
                        .HasColumnType("date");

                    b.Property<bool>("IsActive");

                    b.Property<string>("LeaveType")
                        .IsRequired()
                        .HasColumnType("varchar(50)");

                    b.Property<int>("PersonId");

                    b.Property<string>("Reason")
                        .HasColumnType("varchar(200)");

                    b.Property<double>("RequestedDays")
                        .HasColumnType("float");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("rowversion");

                    b.Property<string>("Status")
                        .HasColumnType("varchar(50)");

                    b.Property<int>("TenantId");

                    b.Property<DateTime>("ToDate")
                        .HasColumnType("date");

                    b.Property<int>("TypeId");

                    b.Property<DateTime>("UpdatedDate")
                        .HasColumnType("datetime");

                    b.HasKey("Id");

                    b.HasIndex("PersonId");

                    b.HasIndex("TypeId");

                    b.ToTable("tblLeaveRequests","LMS");
                });

            modelBuilder.Entity("EIS.Entities.Leave.LeaveRules", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime");

                    b.Property<string>("Description")
                        .HasColumnType("varchar(200)");

                    b.Property<bool>("IsActive");

                    b.Property<string>("LeaveType")
                        .IsRequired()
                        .HasColumnType("varchar(50)");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("rowversion");

                    b.Property<int>("TenantId");

                    b.Property<DateTime>("UpdatedDate")
                        .HasColumnType("datetime");

                    b.Property<DateTime>("ValidFrom")
                        .HasColumnType("date");

                    b.Property<DateTime>("ValidTo")
                        .HasColumnType("date");

                    b.Property<double>("Validity")
                        .HasColumnType("float");

                    b.HasKey("Id");

                    b.ToTable("tblLeaveRules","LMS");
                });

            modelBuilder.Entity("EIS.Entities.Leave.PastLeaves", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("CreatedDate");

                    b.Property<string>("EmployeeName")
                        .HasColumnType("nvarchar(100)");

                    b.Property<DateTime>("FromDate")
                        .HasColumnType("date");

                    b.Property<bool>("IsActive");

                    b.Property<int>("PersonId");

                    b.Property<string>("Reason")
                        .HasColumnType("nvarchar(200)");

                    b.Property<double>("RequestedDays")
                        .HasColumnType("float");

                    b.Property<string>("Status")
                        .HasColumnType("nvarchar(50)");

                    b.Property<int>("TenantId");

                    b.Property<DateTime>("ToDate")
                        .HasColumnType("date");

                    b.Property<DateTime>("UpdatedDate");

                    b.HasKey("Id");

                    b.HasIndex("PersonId");

                    b.ToTable("tblPastLeaves","LMS");
                });

            modelBuilder.Entity("EIS.Entities.OtherEntities.Configuration", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Code");

                    b.Property<DateTime>("CreatedDate");

                    b.Property<bool>("IsActive");

                    b.Property<int>("TenantId");

                    b.Property<DateTime>("UpdatedDate");

                    b.Property<DateTime>("ValidFrom");

                    b.Property<DateTime>("ValidUpTo");

                    b.Property<string>("Value");

                    b.HasKey("Id");

                    b.ToTable("tblConfiguration","LMS");
                });

            modelBuilder.Entity("EIS.Entities.User.Users", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("AccessFailedCount");

                    b.Property<DateTime>("CreatedDate");

                    b.Property<bool>("EmailConfirmed");

                    b.Property<bool>("IsActive");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("PersonId")
                        .HasColumnType("int");

                    b.Property<bool>("PhoneConfirmed");

                    b.Property<int>("TenantId");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<DateTime>("UpdatedDate");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("PersonId")
                        .IsUnique();

                    b.HasIndex("TenantId", "UserName")
                        .IsUnique();

                    b.ToTable("tblUsers","LMS");
                });

            modelBuilder.Entity("EIS.Entities.Address.Current", b =>
                {
                    b.HasOne("EIS.Entities.Employee.Person", "Person")
                        .WithOne("CurrentAddress")
                        .HasForeignKey("EIS.Entities.Address.Current", "PersonId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("EIS.Entities.Address.Emergency", b =>
                {
                    b.HasOne("EIS.Entities.Employee.Person", "Person")
                        .WithMany("EmergencyAddress")
                        .HasForeignKey("PersonId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("EIS.Entities.Address.Other", b =>
                {
                    b.HasOne("EIS.Entities.Employee.Person", "Person")
                        .WithMany("OtherAddress")
                        .HasForeignKey("PersonId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("EIS.Entities.Address.Permanent", b =>
                {
                    b.HasOne("EIS.Entities.Employee.Person", "Person")
                        .WithOne("PermanentAddress")
                        .HasForeignKey("EIS.Entities.Address.Permanent", "PersonId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("EIS.Entities.Employee.Attendance", b =>
                {
                    b.HasOne("EIS.Entities.Employee.Person", "Person")
                        .WithMany("Attendance")
                        .HasForeignKey("PersonId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("EIS.Entities.Employee.Person", b =>
                {
                    b.HasOne("EIS.Entities.Employee.Role", "Role")
                        .WithMany("Persons")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("EIS.Entities.Leave.EmployeeLeaves", b =>
                {
                    b.HasOne("EIS.Entities.Employee.Person", "Person")
                        .WithOne("EmployeeLeaves")
                        .HasForeignKey("EIS.Entities.Leave.EmployeeLeaves", "PersonId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("EIS.Entities.Leave.LeaveRules", "TypeOfLeave")
                        .WithMany()
                        .HasForeignKey("TypeOfLeaveId");
                });

            modelBuilder.Entity("EIS.Entities.Leave.LeaveCredit", b =>
                {
                    b.HasOne("EIS.Entities.Leave.LeaveRules", "LeaveRule")
                        .WithMany("Credits")
                        .HasForeignKey("LeaveId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("EIS.Entities.Employee.Person", "Person")
                        .WithMany("LeaveCredits")
                        .HasForeignKey("PersonId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("EIS.Entities.Leave.LeaveRequest", b =>
                {
                    b.HasOne("EIS.Entities.Employee.Person", "Person")
                        .WithMany("LeaveRequests")
                        .HasForeignKey("PersonId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("EIS.Entities.Leave.LeaveRules", "TypeOfLeave")
                        .WithMany("Requests")
                        .HasForeignKey("TypeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("EIS.Entities.Leave.PastLeaves", b =>
                {
                    b.HasOne("EIS.Entities.Employee.Person", "Person")
                        .WithMany("PastLeaves")
                        .HasForeignKey("PersonId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("EIS.Entities.User.Users", b =>
                {
                    b.HasOne("EIS.Entities.Employee.Person", "Person")
                        .WithOne("User")
                        .HasForeignKey("EIS.Entities.User.Users", "PersonId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
