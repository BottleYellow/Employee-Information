﻿ using System;
using System.Collections.Generic;
using System.ComponentModel;
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
            HttpResponseMessage response = _service.GetResponse(ApiUrl + "api/Dashboard/Admin/All/All");
            string stringData = response.Content.ReadAsStringAsync().Result;
            AdminDashboard dashboard = JsonConvert.DeserializeObject<AdminDashboard>(stringData);
            ViewBag.Locations = GetLocations();
            return View(dashboard);
        }

        [HttpPost]
        public IActionResult AdminDashboard(string attendanceStatus,string location)
        {
            HttpResponseMessage response = _service.GetResponse(ApiUrl + "api/Dashboard/Admin/"+attendanceStatus+"/"+location);
            string stringData = response.Content.ReadAsStringAsync().Result;
            AdminDashboard dashboard = JsonConvert.DeserializeObject<AdminDashboard>(stringData);
            return Json(dashboard);
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

        [DisplayName("Admin Calendar")]
        public IActionResult AdminCalendar()
        {
            ViewBag.Locations = GetLocations();
            return View();
        }

        [ActionName("AdminCalendar")]
        [HttpPost]
        public IActionResult GetAdminCalendar(string intervalStart, string intervalEnd,string location)
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

        [DisplayName("Employee Calendar")]
        public IActionResult EmployeeCalendar()
        {
            return View();
        }
        
        [ActionName("EmployeeCalendar")]
        [HttpPost]
        public IActionResult GetEmployeeCalendar(string intervalStart, string intervalEnd)
        {
            List<CalendarData> data = new List<CalendarData>();
            if (!string.IsNullOrEmpty(intervalStart) && !string.IsNullOrEmpty(intervalEnd))
            {
                DateTime startDate = Convert.ToDateTime(intervalStart);
                DateTime endDate = Convert.ToDateTime(intervalEnd);
                HttpResponseMessage response = _service.GetResponse(ApiUrl + "/api/Dashboard/CalendarData/"+ startDate.ToString("MMM-dd-yyyy") + "/" + endDate.ToString("MMM-dd-yyyy"));

                string stringData = response.Content.ReadAsStringAsync().Result;

                data = JsonConvert.DeserializeObject<List<CalendarData>>(stringData);
            }
            return Json(data);
        }
    }

}
