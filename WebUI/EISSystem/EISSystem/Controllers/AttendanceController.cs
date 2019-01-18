using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using EIS.Entities.Employee;
using EIS.Entities.Leave;
using EIS.WebApp.IServices;
using EIS.WebApp.Models;
using EIS.WebApp.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace EIS.WebApp.Controllers
{
    [DisplayName("Attendance Management")]
    public class AttendanceController : BaseController<Attendance>
    {
        RedisAgent Cache = new RedisAgent();
        public readonly IEISService<Attendance> service;
        public AttendanceController(IEISService<Attendance> service): base(service)
        {
            this.service = service;
        }
        [HttpGet]
        [DisplayName("Attendance Reports")]
        public IActionResult AllAttendance()
        {
            //EmployeeAttendance employeeAttendance = new EmployeeAttendance();
            //return View("AllAttendance", employeeAttendance);

            return View();
        }
        [HttpPost]
        public IActionResult AllAttendance(string date,string type)
        {
            string url = "";
            string[] monthYear = new string[3];
            string[] week = new string[2];
            ViewBag.type = type;
            if (date.Contains('-'))
            {
                week = date.Split('-'); 
            }
            else 
            {
                monthYear = date.Split('/');
            }
            if (type == "year")
            {                
                url = "api/Attendances/GetAllAttendanceYearly/" + monthYear[0];
            }
            else if (type == "week")
            {
                DateTime startDate = Convert.ToDateTime(week[0]);
                DateTime endDate = Convert.ToDateTime(week[1]);
                url = "api/Attendances/GetAllAttendanceWeekly/" + startDate + "/" + endDate;
            }
            else 
            {
                url = "api/Attendances/GetAllAttendanceMonthly/" + monthYear[0] + "/" + monthYear[1];
            }

            ArrayList arrayData = new ArrayList();
            arrayData = LoadData<Person>(url);

            int recordsTotal = JsonConvert.DeserializeObject<int>(arrayData[0].ToString());
            IList<Person> data = JsonConvert.DeserializeObject<IList<Person>>(arrayData[1].ToString());
         
            return Json(new { recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data});
        }

        [DisplayName("My Attendance History")]
        [HttpGet]
        public IActionResult EmployeeReports()
        {
            //int id = Convert.ToInt32(Cache.GetStringValue("PersonId"));
            //ViewBag.id = id;
            //HttpResponseMessage response = service.GetResponse("api/attendances/" + id + "");
            //string stringData = response.Content.ReadAsStringAsync().Result;
            //Attendance data = JsonConvert.DeserializeObject<Attendance>(stringData);
            //if (data == null)
            //{
            //    data = new Attendance();
            //}
            //return View(data);
            return View();
        }

        [DisplayName("My Attendance History")]
        [HttpPost]
        public IActionResult EmployeeReports(string date, string type)
        {
            int pId = Convert.ToInt32(Cache.GetStringValue("PersonId"));
            string url = "";
            string[] monthYear = new string[3];
            string[] week = new string[2];
            List<Attendance> attendances = new List<Attendance>();
            if (date.Contains('-'))
            {
                week = date.Split('-');
            }
            else
            {
                monthYear = date.Split('/');
            }
            if (type == "year")
            {
                url = "api/Attendances/GetYearlyAttendanceById/" + pId + "/" + monthYear[0];
            }
            else if (type == "month")
            {
                url = "api/Attendances/GetMOnthlyAttendanceById/" + pId + "/" + monthYear[1] + "/" + monthYear[0];

            }
            else if (type == "week")
            {
                DateTime startDate = Convert.ToDateTime(week[0]);
                DateTime endDate = Convert.ToDateTime(week[1]);
                ViewBag.startDate = startDate;
                url = "api/Attendances/GetWeeklyAttendanceById/" + pId + "/" + startDate + "/" + endDate;
            }
            ArrayList arrayData = new ArrayList();
            arrayData = LoadData<Attendance>(url);

            int recordsTotal = JsonConvert.DeserializeObject<int>(arrayData[0].ToString());
            IList<Attendance> data = JsonConvert.DeserializeObject<IList<Attendance>>(arrayData[1].ToString());
            return Json(new { recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
        }

        [DisplayName("My Attendance History")]
        [HttpPost]
        public IActionResult AttendanceSummary(string date, string type)
        {
               int id = Convert.ToInt32(Cache.GetStringValue("PersonId"));

         
            string url = "";
            string[] monthYear = new string[3];
            string[] week = new string[2];
            if (date.Contains('-'))
            {
                week = date.Split('-');
            }
            else
            {
                monthYear = date.Split('/');
            }
            if (type == "year")
            {
                url = "api/Attendances/GetYearlyAttendanceSummaryById/" + id + "/" + monthYear[0];
            }
            else if (type == "month")
            {
                url = "api/Attendances/GetMonthlyAttendanceSummaryById/" + id + "/" + monthYear[1] + "/" + monthYear[0];

            }
            else if (type == "week")
            {
                DateTime startDate = Convert.ToDateTime(week[0]);
                DateTime endDate = Convert.ToDateTime(week[1]);
                ViewBag.startDate = startDate;
                url = "api/Attendances/GetWeeklyAttendanceSummaryById/" + id + "/" + startDate + "/" + endDate;
            }      
            HttpResponseMessage response = service.GetResponse(url);
            string stringData = response.Content.ReadAsStringAsync().Result;
            AttendanceReport attendanceReport = new AttendanceReport();
             attendanceReport   = JsonConvert.DeserializeObject<AttendanceReport>(stringData);       
            return Json(attendanceReport);
        }

        [DisplayName("Employee Attendance History")]
        public IActionResult AttendanceSummaryById()
        {
            HttpResponseMessage response = service.GetResponse("api/Employee");
            string stringData = response.Content.ReadAsStringAsync().Result;
            IList<Person> employeesdata = JsonConvert.DeserializeObject<IList<Person>>(stringData);
            var employees = from e in employeesdata select new Person
            {
                Id = e.Id,
                FirstName = e.FirstName + " " + e.LastName
            };
            ViewBag.Persons = employees;
            return View(employees);
        }

        [DisplayName("Employee Attendance History")]
        [HttpPost]
        public IActionResult EmployeeReportsById(string date, string type,int? id)
        {
            if (id == null)
            {
                id = Convert.ToInt32(Cache.GetStringValue("PersonId"));
            }
            string url = "";
            string[] monthYear = new string[3];
            string[] week = new string[2];
            List<Attendance> attendances = new List<Attendance>();
            if (date.Contains('-'))
            {
                week = date.Split('-');
            }
            else
            {
                monthYear = date.Split('/');
            }
            if (type == "year")
            {
                url = "api/Attendances/GetYearlyAttendanceById/" + id + "/" + monthYear[0];
            }
            else if (type == "month")
            {
                url = "api/Attendances/GetMonthlyAttendanceById/" + id + "/" + monthYear[1] + "/" + monthYear[0];

            }
            else if (type == "week")
            {
                DateTime startDate = Convert.ToDateTime(week[0]);
                DateTime endDate = Convert.ToDateTime(week[1]);
                ViewBag.startDate = startDate;
                url = "api/Attendances/GetWeeklyAttendanceById/" + id + "/" + startDate + "/" + endDate;
            }
            ArrayList arrayData = new ArrayList();
            arrayData = LoadData<Attendance>(url);

            int recordsTotal = JsonConvert.DeserializeObject<int>(arrayData[0].ToString());
            IList<Attendance> data = JsonConvert.DeserializeObject<IList<Attendance>>(arrayData[1].ToString());
            return Json(new { recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
        }

        [DisplayName("Employee Attendance History")]
        [HttpPost]
        public IActionResult AttendanceSummaryById(string date, string type,int? id)
        {
            if (id==null)
            { 
            id = Convert.ToInt32(Cache.GetStringValue("PersonId"));
            }

            string url = "";
            string[] monthYear = new string[3];
            string[] week = new string[2];
            if (date.Contains('-'))
            {
                week = date.Split('-');
            }
            else
            {
                monthYear = date.Split('/');
            }
            if (type == "year")
            {
                url = "api/Attendances/GetYearlyAttendanceSummaryById/" + id + "/" + monthYear[0];
            }
            else if (type == "month")
            {
                url = "api/Attendances/GetMonthlyAttendanceSummaryById/" + id + "/" + monthYear[1] + "/" + monthYear[0];

            }
            else if (type == "week")
            {
                DateTime startDate = Convert.ToDateTime(week[0]);
                DateTime endDate = Convert.ToDateTime(week[1]);
                ViewBag.startDate = startDate;
                url = "api/Attendances/GetWeeklyAttendanceSummaryById/" + id + "/" + startDate + "/" + endDate;
            }
            HttpResponseMessage response = service.GetResponse(url);
            string stringData = response.Content.ReadAsStringAsync().Result;
            AttendanceReport attendanceReport = new AttendanceReport();
            attendanceReport = JsonConvert.DeserializeObject<AttendanceReport>(stringData);
            return Json(attendanceReport);
        }

        [DisplayName("Create Attendance")]
        [HttpGet]
        public IActionResult AttendanceInOut()
        {
            var attendance = new Attendance();
            int id = Convert.ToInt32(Cache.GetStringValue("PersonId"));
            if (ModelState.IsValid)
            {
                HttpClient client = service.GetService();
                string stringData = JsonConvert.SerializeObject(attendance);
                var contentData = new StringContent(stringData, Encoding.UTF8, "application/json");
                HttpResponseMessage response = client.PostAsJsonAsync("api/attendances/" + id + "", attendance).Result;                
                ViewBag.statusCode = Convert.ToInt32(response.StatusCode);                
            }
            return View("AllAttendance");
        }
        
        [HttpPost]
        public IActionResult AttendanceInOut(int id,Attendance attendance)
        {
            id = Convert.ToInt32(Cache.GetStringValue("PersonId"));
            if (ModelState.IsValid)
            {
                HttpClient client = service.GetService();
                string stringData = JsonConvert.SerializeObject(attendance);
                var contentData = new StringContent(stringData, Encoding.UTF8, "application/json");
                HttpResponseMessage response = client.PutAsJsonAsync("api/attendances/"+id+"",attendance).Result;
                ViewBag.Message = response.Content.ReadAsStringAsync().Result;
            }
            return View("AllAttendance");
        }
    }
}
