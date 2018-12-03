using EIS.Entities.Address;
using EIS.Entities.Employee;
using EIS.Entities.User;
using EIS.Repositories.IRepository;
using EIS.Repositories.Repository;
using EIS.Validations.FluentValidations;
using EIS.WebApp.IServices;
using EIS.WebApp.Services;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace EIS.WebApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            
            services.AddDbContext<EIS.Data.Context.ApplicationDbContext>(config =>
            {
                config.UseSqlServer(Configuration.GetConnectionString("connection"));
            });

            services.AddMvc();
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
            services.AddSession();
            services.AddTransient<IEISService, EISService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
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
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseSession();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Login}/{action=Login}");
            });
        }
    }
}
