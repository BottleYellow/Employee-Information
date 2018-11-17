using EIS.Entities.Address;
using EIS.Entities.Admin;
using EIS.Entities.Employee;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace EIS.Data.Context
{
    public class DbContext : BuilderExtension
    {
        private readonly string connectionString;
        public DbContext(DbContextOptions<DbContext> options)
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
            optionsBuilder.UseSqlServer(connectionString);
        }

        #region[dbset]
        public DbSet<Login> Logins { get; set; }
        public DbSet<Person> Person { get; set; }
        public DbSet<Leaves> Leaves { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<Permanent> PermanentAddresses { get; set; }
        public DbSet<Current> CurrentAddresses { get; set; }
        public DbSet<Emergency> EmergencyAddresses { get; set; }
        public DbSet<Other> OtherAddresses { get; set; }
        #endregion
    }
}
