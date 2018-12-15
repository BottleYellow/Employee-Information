﻿using EIS.Entities.Address;
using EIS.Entities.Employee;
using EIS.Entities.User;

using EIS.Repositories.IRepository;
using EIS.Repositories.Repository;
using EIS.Validations.FluentValidations;
using EIS.WebApp.Filters;
using EIS.WebApp.IServices;
using EIS.WebApp.Services;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Newtonsoft.Json.Serialization;
using System;
using System.Net;
using System.Threading.Tasks;

namespace EIS.WebApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
          
            services.AddDbContext<EIS.Data.Context.ApplicationDbContext>(config =>
            {
                config.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            });

            
            services.AddMvc();
            services.AddMvc(options =>
            {
                options.Filters.Add(typeof(CustomActionFilter));
            });
            services.AddMvc().AddFluentValidation();

            #region[Validations]
            services.AddTransient<IValidator<Person>, PersonValidator>();
            services.AddTransient<IValidator<Attendance>, AttendanceValidator>();
            services.AddTransient<IValidator<Leaves>, LeavesValidator>();
            services.AddTransient<IValidator<Permanent>, PermanentAddressValidator>();
            services.AddTransient<IValidator<Current>, CurrentAddressValidator>();
            services.AddTransient<IValidator<Emergency>, EmergencyAddressValidator>();
            services.AddTransient<IValidator<Other>, OtherAddressValidator>();
            #endregion

            
            //for generic repository
            services.AddScoped<IRepositoryWrapper, RepositoryWrapper>();
            services.AddScoped<RedisAgent>();
            services.AddSession();
            services.AddTransient(typeof(IEISService<>), typeof(EISService<>));
            services.AddSingleton<IControllerService, ControllerService>();
            ////Authorization
            //services.AddAuthorization(options =>
            //{
            //    options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
            //});
            

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            //AppDependencyResolver.Init(app.ApplicationServices);
            //var Cache = (IDistributedCache)AppDependencyResolver.Current.GetService(typeof(IDistributedCache));
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            
            app.UseAuthentication();
            app.UseSession();
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            string controller = "Account";
            string action = "Login";
            string Template = "{controller=" + controller + "}/{action=" + action + "}";
            string id = new RedisAgent().GetStringValue("PersonId");

            if (id != null)
            {
                string role= new RedisAgent().GetStringValue("Role");
                if (role == "Admin")
                {
                    controller = "People";
                    action = "Index";
                    Template = "{controller=" + controller + "}/{action=" + action + "}";
                }
                else
                {
                    controller = "People";
                    action = "Profile";
                    Template = "{controller=" + controller + "}/{action=" + action + "}/{id=" + id + "}";
                }
            }
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: Template);
            });
        }
    }
}
