 using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using EIS.Entities.Dashboard;
using EIS.Entities.Employee;
using EIS.Entities.Hoildays;
using EIS.Entities.Leave;
using EIS.Entities.Models;
using EIS.Entities.SP;
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
            ViewBag.Locations = GetLocations();
            return View();
        }

        [HttpPost]
        public IActionResult AdminDashboard(string attendanceStatus, int location)
        {
            HttpResponseMessage response = _service.GetResponse(ApiUrl + "api/Dashboard/Admin/" + attendanceStatus + "/" + location);
            string stringData = response.Content.ReadAsStringAsync().Result;
            Admin_Dashboard dashboard = JsonConvert.DeserializeObject<Admin_Dashboard>(stringData);
            return Json(dashboard);
        }

        public IActionResult HRDashboard()
        {
            ViewBag.Locations = GetLocations();            
            if(TempData["BirhdayAlert"]!=null)
            {
                int day = DateTime.Now.Day;
                int month = DateTime.Now.Month;
                HttpResponseMessage response = _service.GetResponse(ApiUrl + "api/Dashboard/BirthdayData/" + day + "/" + month);
                string stringData = response.Content.ReadAsStringAsync().Result;
                List<Person> birthDayperson = JsonConvert.DeserializeObject<List<Person>>(stringData);
                string person = Convert.ToString(TempData["BirhdayAlert"]);
                if (birthDayperson.Count > 0)
                {
                    foreach (var p in birthDayperson)
                    {
                        person += p.FullName + ",";
                    }
                    person= person.Remove(person.Length - 1, 1) + ".";
                    TempData["BirhdayAlert"] = person;
                }
                else
                {
                    TempData["BirhdayAlert"] = null;
                }            
            }            
            return View();
        }

        [HttpPost]
        public IActionResult HRDashboard(string attendanceStatus, int location)
        {
            HttpResponseMessage response = _service.GetResponse(ApiUrl + "api/Dashboard/Admin/" + attendanceStatus + "/" + location);
            string stringData = response.Content.ReadAsStringAsync().Result;
            Admin_Dashboard dashboard = JsonConvert.DeserializeObject<Admin_Dashboard>(stringData);
            return Json(dashboard);
        }
        public IActionResult ManagerDashboard()
        {
            ViewBag.Locations = GetLocations();
            return View();
        }
        [HttpPost]
        public IActionResult ManagerDashboard(string attendanceStatus, int location)
        {
            HttpResponseMessage response = _service.GetResponse(ApiUrl + "api/Dashboard/Admin/" + attendanceStatus + "/" + location);
            string stringData = response.Content.ReadAsStringAsync().Result;
            Admin_Dashboard dashboard = JsonConvert.DeserializeObject<Admin_Dashboard>(stringData);
            return Json(dashboard);
        }
        public IActionResult EmployeeDashboard()
        {
            return View();
        }

        [HttpPost]
        public IActionResult EmployeeDashboard(int PersonId)
        {
            HttpResponseMessage response = _service.GetResponse(ApiUrl + "api/Dashboard/Employee/" + PersonId);
            string stringData = response.Content.ReadAsStringAsync().Result;
            Employee_Dashboard dashboard = JsonConvert.DeserializeObject<Employee_Dashboard>(stringData);
            return Json(dashboard);
        }
        [DisplayName("Admin Calendar")]
        public IActionResult Calendar()
        {
            ViewBag.Locations = GetLocations();
            return View();
        }

        [ActionName("Calendar")]
        [HttpPost]
        public IActionResult GetAdminCalendar(string intervalStart, string intervalEnd,int location)
        {
            List<CalendarData> data = new List<CalendarData>();
            if (!string.IsNullOrEmpty(intervalStart)&& !string.IsNullOrEmpty(intervalEnd))
            {
                DateTime startDate = Convert.ToDateTime(intervalStart);
                DateTime endDate = Convert.ToDateTime(intervalEnd);
                HttpResponseMessage response = _service.GetResponse(ApiUrl + "/api/Dashboard/AdminCalendarData/" +location+"/"+startDate.ToString("MMM-dd-yyyy") + "/" + endDate.ToString("MMM-dd-yyyy"));

                string stringData = response.Content.ReadAsStringAsync().Result;

                data = JsonConvert.DeserializeObject<List<CalendarData>>(stringData);              
            }
            return Json(data);
        }

        [DisplayName("Employee Calendar")]
        public IActionResult EmployeeCalendar()
        {
            return View();
        }
        
        [ActionName("EmployeeCalendar")]
        [HttpPost]
        public IActionResult GetEmployeeCalendar(string intervalStart, string intervalEnd,int personId)
        {
            List<CalendarData> data = new List<CalendarData>();
            if (!string.IsNullOrEmpty(intervalStart) && !string.IsNullOrEmpty(intervalEnd))
            {
                DateTime startDate = Convert.ToDateTime(intervalStart);
                DateTime endDate = Convert.ToDateTime(intervalEnd);
                HttpResponseMessage response = _service.GetResponse(ApiUrl + "/api/Dashboard/EmployeeCalendarData/"+personId+"/"+ startDate.ToString("MMM-dd-yyyy") + "/" + endDate.ToString("MMM-dd-yyyy"));

                string stringData = response.Content.ReadAsStringAsync().Result;

                data = JsonConvert.DeserializeObject<List<CalendarData>>(stringData);
            }
            return Json(data);
        }
    }

}
