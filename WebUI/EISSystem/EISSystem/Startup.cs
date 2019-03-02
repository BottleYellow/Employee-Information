using EIS.Entities.Address;
using EIS.Entities.Employee;
using EIS.Entities.Hoildays;
using EIS.Entities.Leave;
using EIS.Repositories.IRepository;
using EIS.Repositories.Repository;
using EIS.Validations.FluentValidations;
using EIS.WebApp.Filters;
using EIS.WebApp.IServices;
using EIS.WebApp.Services;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;
using System.Net;

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
                options.Filters.Add(typeof(ErrorHandlingFilter));
            });
            services.AddMvc()
            .AddFluentValidation(fvc =>
                fvc.RegisterValidatorsFromAssemblyContaining<Startup>());
            #region[Validations]
            services.AddTransient<IValidator<Person>, PersonValidator>();
            services.AddTransient<IValidator<Attendance>, AttendanceValidator>();
            services.AddTransient<IValidator<LeaveRequest>, LeaveRequestValidator>();
            services.AddTransient<IValidator<LeaveRules>, LeaveRulesValidator>();
            services.AddTransient<IValidator<LeaveCredit>, LeaveCreditValidator>();
            services.AddTransient<IValidator<Permanent>, PermanentAddressValidator>();
            services.AddTransient<IValidator<Current>, CurrentAddressValidator>();
            services.AddTransient<IValidator<Emergency>, EmergencyAddressValidator>();
            services.AddTransient<IValidator<Other>, OtherAddressValidator>();
            services.AddTransient<IValidator<Holiday>, HolidayValidator>();
            services.AddTransient<IValidator<Locations>, LocationValidator>();
            #endregion

            services.AddTransient<IRepositoryWrapper, RepositoryWrapper>();
            services.AddDistributedMemoryCache();

            services.AddSession();
            services.AddTransient<IServiceWrapper, ServiceWrapper>();
            services.AddTransient(typeof(IEISService<>), typeof(EISService<>));
            services.AddSingleton<IControllerService, ControllerService>();
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
                    {
                        options.AccessDeniedPath = new PathString("/login");
                        options.LoginPath = new PathString("/login");
                        options.SlidingExpiration = true;
                    });
            services.AddMvc().AddJsonOptions(options => {
                var resolver = options.SerializerSettings.ContractResolver;
                if (resolver != null)
                {
                    var res = (DefaultContractResolver)resolver;
                    res.NamingStrategy = null;
                }
            });
            services.AddMvc()
              .AddJsonOptions(
                    options => options.SerializerSettings.ReferenceLoopHandling
                        = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
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
            
            app.UseHttpContext();
            app.UseAuthentication();
            app.UseSession();
            app.UseHttpsRedirection();
            app.UseWebAppExceptionHandler();
            app.UseStaticFiles();
            string controller = "Account";
            string action = "Login";
            string Template = "{controller=" + controller + "}/{action=" + action + "}";
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: Template);
            });
        }
    }
}
