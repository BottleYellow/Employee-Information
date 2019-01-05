using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Http;
using System.Text;
using EIS.Entities.Employee;
using EIS.WebApp.IServices;
using EIS.WebApp.Models;
using EIS.WebApp.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace EIS.WebApp.Controllers
{
    [DisplayName("Attendance Management")]
    public class AttendanceController : Controller
    {
        RedisAgent Cache = new RedisAgent();
        public readonly IEISService<Attendance> service;
        public AttendanceController(IEISService<Attendance> service)
        {
            this.service = service;
        }

        [HttpGet]
        [DisplayName("Attendance Reports")]
        public IActionResult AllAttendance()
        {
            EmployeeAttendance employeeAttendance = new EmployeeAttendance();
            return View("AllAttendance", employeeAttendance);
        }
        [HttpPost]
        public IActionResult AllAttendance(string date,string type)
        {
            HttpResponseMessage respattendance;
            string[] monthYear = new string[3];
            string[] week = new string[2];
            string attendance="";
            List<Person> attendances = new List<Person>();
            if(date.Contains('-'))
            {
                week = date.Split('-'); 
            }
            else 
            {
                monthYear = date.Split('/');
            }
            if (type == "year")
            {
                ViewBag.type = type;
                ViewBag.year = Convert.ToInt32(monthYear[0]);
                respattendance = service.GetResponse("api/Attendances/GetAllAttendanceYearly/" + monthYear[0]);
                attendance = respattendance.Content.ReadAsStringAsync().Result;
            }
            else if (type == "month")
            {
                ViewBag.type = type;
                ViewBag.month = Convert.ToInt32(monthYear[0]);
                ViewBag.year = Convert.ToInt32(monthYear[1]);
                respattendance = service.GetResponse("api/Attendances/GetAllAttendanceMonthly/" + monthYear[0]+ "/" + monthYear[1]);
                attendance = respattendance.Content.ReadAsStringAsync().Result;
            }
            else if (type == "week")
            {
                ViewBag.type = type;
                DateTime startDate = Convert.ToDateTime(week[0]);
                DateTime endDate = Convert.ToDateTime(week[1]);
                respattendance = service.GetResponse("api/Attendances/GetAllAttendanceWeekly/" + startDate + "/" + endDate);
                attendance = respattendance.Content.ReadAsStringAsync().Result;
            }
            attendances = JsonConvert.DeserializeObject<List<Person>>(attendance);
            return PartialView("GetAllAttendance", attendances);
        }

        [DisplayName("My Attendance History")]
        [HttpGet]
        public IActionResult EmployeeReports()
        {
            int id = Convert.ToInt32(HttpContext.Session.GetString("id"));
            HttpResponseMessage response = service.GetResponse("api/attendances/" + id + "");
            string stringData = response.Content.ReadAsStringAsync().Result;
            List<Attendance> data = JsonConvert.DeserializeObject<List<Attendance>>(stringData);
            return View(data);
        }
        [HttpPost]
        public IActionResult EmployeeReports(string date, string type)
        {
            int pId = Convert.ToInt32(Cache.GetStringValue("PersonId"));
            HttpResponseMessage respattendance;
            string[] monthYear = new string[3];
            string[] week = new string[2];
            string attendance = "";
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
                ViewBag.type = type;
                ViewBag.year = Convert.ToInt32(monthYear[0]);
                respattendance = service.GetResponse("api/Attendances/GetAttendanceById/" + pId +"/"+ monthYear[0]);
                attendance = respattendance.Content.ReadAsStringAsync().Result;
            }
            else if (type == "month")
            {
                ViewBag.type = type;
                ViewBag.month = Convert.ToInt32(monthYear[0]);
                ViewBag.year = Convert.ToInt32(monthYear[1]);
                respattendance = service.GetResponse("api/Attendances/GetAttendanceById/"+pId+"/" + monthYear[1] + "/" + monthYear[0]);
                attendance = respattendance.Content.ReadAsStringAsync().Result;
            }
            else if (type == "week")
            {
                ViewBag.type = type;
                DateTime startDate = Convert.ToDateTime(week[0]);
                DateTime endDate = Convert.ToDateTime(week[1]);
                ViewBag.startDate= startDate;
                respattendance = service.GetResponse("api/Attendances/GetWeeklyAttendanceById/"+pId+"/"+ startDate + "/" + endDate);
                attendance = respattendance.Content.ReadAsStringAsync().Result;
            }
            attendances = JsonConvert.DeserializeObject<List<Attendance>>(attendance);
            return PartialView("GetAttendanceById",attendances);
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
