﻿using EIS.Entities.Address;
using EIS.Entities.Employee;
using EIS.Entities.Leave;
using EIS.Entities.OtherEntities;
using EIS.Entities.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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
            optionsBuilder.UseSqlServer(connectionString,options =>
            {
                options.UseRowNumberForPaging();
            });
        }

        #region[Tables]
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

        #endregion
    }
}
