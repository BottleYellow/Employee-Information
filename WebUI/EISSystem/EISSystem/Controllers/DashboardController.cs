 using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using EIS.Entities.Dashboard;
using EIS.Entities.Employee;
using EIS.Entities.Hoildays;
using EIS.Entities.Leave;
using EIS.WebApp.IServices;
using EIS.WebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace EIS.WebApp.Controllers
{
    public class DashboardController : BaseController<Person>
    {
        public readonly IServiceWrapper _services;
        public DashboardController(IEISService<Person> service, IServiceWrapper services) : base(service)
        {
            _services = services;
        }
        public IActionResult AdminDashboard()
        {
            ViewData["Message"] = $"{Request.Scheme}://{Request.Host}{Request.PathBase}";
            HttpResponseMessage response = _services.Employee.GetResponse(ApiUrl+"/Dashboard/Admin");
            string stringData = response.Content.ReadAsStringAsync().Result;
            AdminDashboard dashBoard = JsonConvert.DeserializeObject<AdminDashboard>(stringData);
            return View(dashBoard);
        }

        public IActionResult ManagerDashboard()
        {
            HttpResponseMessage response = _services.Employee.GetResponse(ApiUrl+"/Dashboard/Manager");
            string stringData = response.Content.ReadAsStringAsync().Result;
            ManagerDashboard dashBoard = JsonConvert.DeserializeObject<ManagerDashboard>(stringData);
            return View(dashBoard);
        }

        public IActionResult EmployeeDashboard()
        {
            int PersonId = Convert.ToInt32(Cache.GetStringValue("PersonId"));
            HttpResponseMessage response = _services.Employee.GetResponse(ApiUrl+"/Dashboard/Employee/" + PersonId + "");
            string stringData = response.Content.ReadAsStringAsync().Result;
            EmployeeDashboard dashBoard = JsonConvert.DeserializeObject<EmployeeDashboard>(stringData);
            return View(dashBoard);
        }

        public IActionResult Calendar()
        {
            return View();
        }

        [HttpPost]
        public IActionResult GetFullCalendar()
        {
            HttpResponseMessage response = _service.GetResponse(ApiUrl+"/Dashboard/CalendarData");
            string stringData = response.Content.ReadAsStringAsync().Result;
            List<CalendarData> data = JsonConvert.DeserializeObject<List<CalendarData>>(stringData);         
            return Json(data);
        }
    }

}
