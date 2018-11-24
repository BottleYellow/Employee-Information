
using EIS.Data.Context;
using EIS.Entities.User;
using EIS.Repositories.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;


namespace EIS.WebAPI.Data
{
    public class SeedDatabase
    {
        public readonly IRepositoryWrapper _repository;
        public SeedDatabase(IRepositoryWrapper repository)
        {
            _repository = repository;
        }
        public static async Task Initialize(IServiceProvider serviceProvider)
        {

            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
            context.Database.Migrate();
            context.Database.EnsureCreated();

            //adding customs roles
            //string[] roleNames = { "Admin", "Manager", "Employee" };
            //foreach (var roleName in roleNames)
            //{
            //    // creating the roles and seeding them to the database
            //    var roleExist =await _repository.RoleManager.RoleExistsAsync(roleName);
            //    if (!roleExist)
            //    {
            //        await _repository.RoleManager.CreateRoleAsync(roleName);
            //    }
            //}
        }
    }
}
