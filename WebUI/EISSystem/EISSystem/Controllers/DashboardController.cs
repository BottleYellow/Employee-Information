using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using EIS.Entities.Dashboard;
using EIS.Entities.Employee;
using EIS.WebApp.IServices;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace EIS.WebApp.Controllers
{
    public class DashboardController : BaseController<Person>
    {
        public readonly IServiceWrapper _services;
        public DashboardController(IEISService<Person> service, IServiceWrapper services):base(service)
        {
            _services = services;
        }
        public IActionResult AdminDashboard()
        {
            HttpResponseMessage response = _services.Employee.GetResponse("api/Dashboard/Admin");
            string stringData = response.Content.ReadAsStringAsync().Result;
            var dashBoard = JsonConvert.DeserializeObject<AdminDashboard>(stringData);
            return View(dashBoard);
        }

        public IActionResult ManagerDashboard()
        {
            HttpResponseMessage response = _services.Employee.GetResponse("api/Dashboard/Manager");
            string stringData = response.Content.ReadAsStringAsync().Result;
            var dashBoard = JsonConvert.DeserializeObject<ManagerDashboard>(stringData);
            return View(dashBoard);
        }

        public IActionResult EmployeeDashboard()
        {
            int PersonId = Convert.ToInt32(Cache.GetStringValue("PersonId"));
            HttpResponseMessage response = _services.Employee.GetResponse("api/Dashboard/Employee/"+PersonId+"");
            string stringData = response.Content.ReadAsStringAsync().Result;
            var dashBoard = JsonConvert.DeserializeObject<EmployeeDashboard>(stringData);
            return View(dashBoard);
        }
    }
}