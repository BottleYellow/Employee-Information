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

        #region Admin Dashboard
        [DisplayName("Admin Dashboard")]
        public IActionResult AdminDashboard()
        {
            ViewBag.Locations = GetLocations();
            return View();
        }

        [HttpPost]
        public IActionResult AdminDashboard(string attendanceStatus, int location,string date)
        {            
            HttpResponseMessage response = _service.GetResponse(ApiUrl + "api/Dashboard/Admin/" + attendanceStatus + "/" + location+"/"+date);
            string stringData = response.Content.ReadAsStringAsync().Result;
            Admin_Dashboard dashboard = JsonConvert.DeserializeObject<Admin_Dashboard>(stringData);
            return Json(dashboard);
        }
        #endregion

        #region HR Dashboard
        [DisplayName("HR Dashboard")]
        public IActionResult HRDashboard()
        {
            ViewBag.Locations = GetLocations();            
            if(TempData["BirthdayAlert"]!=null)
            {
                int day = DateTime.Now.Day;
                int month = DateTime.Now.Month;
                HttpResponseMessage response = _service.GetResponse(ApiUrl + "api/Dashboard/BirthdayData/" + day + "/" + month);
                string stringData = response.Content.ReadAsStringAsync().Result;
                List<Person> birthDayperson = JsonConvert.DeserializeObject<List<Person>>(stringData);
                string person = Convert.ToString(TempData["BirthdayAlert"]);
                if (birthDayperson.Count > 0)
                {
                    foreach (var p in birthDayperson)
                    {
                        person += p.FullName + ",";
                    }
                    person= person.Remove(person.Length - 1, 1) + ".";
                    TempData["BirthdayAlert"] = person;
                }
                else
                {
                    TempData["BirthdayAlert"] = null;
                }
                string id = GetSession().PersonId;
                HttpResponseMessage httpResponse = _service.GetResponse(ApiUrl + "api/Dashboard/EmployeeBirthday/" + day + "/" + month+"/"+id);
                string stringEmployeeData = httpResponse.Content.ReadAsStringAsync().Result;
                string employeeBirthdayperson = JsonConvert.DeserializeObject<string>(stringEmployeeData);
                if(!string.IsNullOrEmpty(employeeBirthdayperson))
                {
                    TempData["EmployeeBirthdayAlert"] = "Birthday wishes from Aadyam Consultant";
                }
                else
                {
                    TempData["EmployeeBirthdayAlert"] = null;
                }
            }            
            return View();
        }

        [HttpPost]
        public IActionResult HRDashboard(string attendanceStatus, int location, string date)
        {
            HttpResponseMessage response = _service.GetResponse(ApiUrl + "api/Dashboard/Admin/" + attendanceStatus + "/" + location + "/" + date);
            string stringData = response.Content.ReadAsStringAsync().Result;
            Admin_Dashboard dashboard = JsonConvert.DeserializeObject<Admin_Dashboard>(stringData);
            return Json(dashboard);
        }
        #endregion

        #region Manager Dashboard
        [DisplayName("Manager Dashboard")]
        public IActionResult ManagerDashboard()
        {
            ViewBag.Locations = GetLocations();
            if (TempData["BirthdayAlert"] != null)
            {
                int day = DateTime.Now.Day;
                int month = DateTime.Now.Month;
                string id = GetSession().PersonId;
                HttpResponseMessage response = _service.GetResponse(ApiUrl + "api/Dashboard/EmployeeBirthday/" + day + "/" + month+"/"+id);
                string stringData = response.Content.ReadAsStringAsync().Result;
                string birthDayperson = JsonConvert.DeserializeObject<string>(stringData);
                if (!string.IsNullOrEmpty(birthDayperson))
                {
                    TempData["BirthdayAlert"] = "Happy Birthday "+birthDayperson;
                }
                else
                {
                    TempData["BirthdayAlert"] = null;
                }
            }
            return View();
        }
        [HttpPost]
        public IActionResult ManagerDashboard(string attendanceStatus, int location, string date)
        {
            HttpResponseMessage response = _service.GetResponse(ApiUrl + "api/Dashboard/Admin/" + attendanceStatus + "/" + location + "/" + date);
            string stringData = response.Content.ReadAsStringAsync().Result;
            Admin_Dashboard dashboard = JsonConvert.DeserializeObject<Admin_Dashboard>(stringData);
            return Json(dashboard);
        }
        #endregion

        #region Employee Dashboard
        [DisplayName("Employee Dashboard")]
        public IActionResult EmployeeDashboard()
        {
            if (TempData["BirthdayAlert"] != null)
            {
                int day = DateTime.Now.Day;
                int month = DateTime.Now.Month;
                string id = GetSession().PersonId;
                HttpResponseMessage response = _service.GetResponse(ApiUrl + "api/Dashboard/EmployeeBirthday/" + day + "/" + month+"/"+id);
                string stringData = response.Content.ReadAsStringAsync().Result;
                string birthDayperson = JsonConvert.DeserializeObject<string>(stringData);
                if (!string.IsNullOrEmpty(birthDayperson))
                {
                    TempData["BirthdayAlert"] = "Birthday wishes from Aadyam Consultant";
                }
                else
                {
                    TempData["BirthdayAlert"] = null;
                }
            }
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
        #endregion

        #region Admin Calendar
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
        #endregion

        #region Employee Calendar
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
        #endregion

        #region Attendance Request
        [HttpPost]
        public IActionResult AttendanceRequest(string personId,string message,string dateSelected)
        {
            DateTime date = Convert.ToDateTime(dateSelected);
            HttpResponseMessage response = _service.GetResponse(ApiUrl + "/api/Dashboard/AttendanceRequest/" + personId + "/" + date.ToString("MMM-dd-yyyy") + "/" +message);
            string stringData = response.Content.ReadAsStringAsync().Result;
            string data = JsonConvert.DeserializeObject<string>(stringData);
            return Json(data);
        }
        #endregion
    }

}
