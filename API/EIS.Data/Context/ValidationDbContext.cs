
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace EIS.Data.Context
{
   public class ValidationDbContext : IDesignTimeDbContextFactory<EIS.Data.Context.DbContext>
    {
        public EIS.Data.Context.DbContext CreateDbContext(string[] args)
        {
            //Get Connection string through app.settings
            IConfigurationRoot configuration = new ConfigurationBuilder()
           .SetBasePath(Directory.GetCurrentDirectory())
           .AddJsonFile("appsettings.json")
           .Build();
            string connectionString = configuration.GetConnectionString("DefaultConnection");

            var builder = new DbContextOptionsBuilder<EIS.Data.Context.DbContext>();
            builder.UseSqlServer(connectionString, optionsBuilder => optionsBuilder.MigrationsAssembly(typeof(EIS.Data.Context.DbContext).GetTypeInfo().Assembly.GetName().Name));
            return new EIS.Data.Context.DbContext(builder.Options);
        }
    }
}
