 using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using EIS.Entities.Dashboard;
using EIS.Entities.Employee;
using EIS.Entities.Hoildays;
using EIS.Entities.Leave;
using EIS.WebApp.Filters;
using EIS.WebApp.IServices;
using EIS.WebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace EIS.WebApp.Controllers
{
    [SessionTimeOut]
    public class DashboardController : BaseController<Person>
    {
        public readonly IServiceWrapper _services;
        public DashboardController(IEISService<Person> service, IServiceWrapper services) : base(service)
        {
            _services = services;
        }
        public IActionResult AdminDashboard()
        {
            HttpResponseMessage response = _service.GetResponse(ApiUrl + "api/Dashboard/Admin");
            string stringData = response.Content.ReadAsStringAsync().Result;
            AdminDashboard dashboard = JsonConvert.DeserializeObject<AdminDashboard>(stringData);
            return View(dashboard);
        }

        [HttpPost]
        public IActionResult AdminDashboard(string status)
        {
            HttpResponseMessage response = _service.GetResponse(ApiUrl + "api/Dashboard/Admin" + status);
            string stringData = response.Content.ReadAsStringAsync().Result;
            AdminDashboard dashboard = JsonConvert.DeserializeObject<AdminDashboard>(stringData);
            return View(dashboard);
        }
        


        public IActionResult ManagerDashboard()
        {
            HttpResponseMessage response = _services.Employee.GetResponse(ApiUrl+"/api/Dashboard/Manager");
            string stringData = response.Content.ReadAsStringAsync().Result;
            ManagerDashboard dashBoard = JsonConvert.DeserializeObject<ManagerDashboard>(stringData);
            return View(dashBoard);
        }

        public IActionResult EmployeeDashboard()
        {
            //int PersonId = Convert.ToInt32(Cache.GetStringValue("PersonId"));
            int PersonId = Convert.ToInt32(GetSession().PersonId);
            HttpResponseMessage response = _services.Employee.GetResponse(ApiUrl+"/api/Dashboard/Employee/" + PersonId + "");
            string stringData = response.Content.ReadAsStringAsync().Result;
            EmployeeDashboard dashBoard = JsonConvert.DeserializeObject<EmployeeDashboard>(stringData);
            return View(dashBoard);
        }

        public IActionResult Calendar()
        {
            return View();
        }

        [HttpPost]
        public IActionResult GetFullCalendar(string intervalStart, string intervalEnd,string location)
        {
            List<CalendarData> data = new List<CalendarData>();
            if (!string.IsNullOrEmpty(intervalStart)&& !string.IsNullOrEmpty(intervalEnd))
            {
                DateTime startDate = Convert.ToDateTime(intervalStart);
                DateTime endDate = Convert.ToDateTime(intervalEnd);
                HttpResponseMessage response = _service.GetResponse(ApiUrl + "/api/Dashboard/CalendarData/" +location+"/"+startDate.ToString("MMM-dd-yyyy") + "/" + endDate.ToString("MMM-dd-yyyy"));

                string stringData = response.Content.ReadAsStringAsync().Result;

                data = JsonConvert.DeserializeObject<List<CalendarData>>(stringData);              
            }
            return Json(data);
        }
    }

}
