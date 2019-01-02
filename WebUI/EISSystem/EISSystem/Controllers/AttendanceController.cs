using System;
using System.Collections.Generic;
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
    public class AttendanceController : Controller
    {
        RedisAgent Cache = new RedisAgent();
        public readonly IEISService<Attendance> service;
        public AttendanceController(IEISService<Attendance> service)
        {
            this.service = service;
        }
        public IActionResult Index()
        {
            return View("AllAttendance");
        }
        [HttpGet]
        public IActionResult AllAttendance()
        {
            EmployeeAttendance employeeAttendance = new EmployeeAttendance();
            return View("AllAttendance", employeeAttendance);
        }
        [HttpGet]
        public IActionResult GetAllAttendance(string date,string type)
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
<<<<<<< HEAD
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
            return PartialView(attendances);
        }

        public IActionResult GetAttendanceById(string date, string type)
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
=======
>>>>>>> eab0133b5e8f6e86eb09bb18611280e9b8dcee1c
                attendance = respattendance.Content.ReadAsStringAsync().Result;
            }
            else if (type == "week")
            {
                ViewBag.type = type;
<<<<<<< HEAD
                DateTime startDate = Convert.ToDateTime(week[0]);
                DateTime endDate = Convert.ToDateTime(week[1]);
                ViewBag.startDate= startDate;
                respattendance = service.GetResponse("api/Attendances/GetWeeklyAttendanceById/"+pId+"/"+ startDate + "/" + endDate);
                attendance = respattendance.Content.ReadAsStringAsync().Result;
            }
            attendances = JsonConvert.DeserializeObject<List<Attendance>>(attendance);
=======
                respattendance = service.GetResponse("api/Attendances/GetAllAttendanceWeekly/" + Convert.ToDateTime(week[0]) + "/" + Convert.ToDateTime(week[1]));
                attendance = respattendance.Content.ReadAsStringAsync().Result;
            }
            attendances = JsonConvert.DeserializeObject<List<Person>>(attendance);
>>>>>>> eab0133b5e8f6e86eb09bb18611280e9b8dcee1c
            return PartialView(attendances);
        }

        [HttpGet]
        public IActionResult Create()
        {
            int id = Convert.ToInt32(HttpContext.Session.GetString("id"));
            HttpResponseMessage response = service.GetResponse("api/attendances/" + id + "");
            string stringData = response.Content.ReadAsStringAsync().Result;
            List<Attendance> data = JsonConvert.DeserializeObject<List<Attendance>>(stringData);
            return View(data);
        }

        [HttpPost]
        public IActionResult Create(Attendance attendance)
        {
<<<<<<< HEAD
            int id = Convert.ToInt32(Cache.GetStringValue("PersonId"));
=======
            int id = Convert.ToInt32(HttpContext.Session.GetString("id"));
>>>>>>> eab0133b5e8f6e86eb09bb18611280e9b8dcee1c
            if (ModelState.IsValid)
            {
                HttpClient client = service.GetService();
                string stringData = JsonConvert.SerializeObject(attendance);
                var contentData = new StringContent(stringData, Encoding.UTF8, "application/json");
<<<<<<< HEAD
                HttpResponseMessage response = client.PostAsJsonAsync("api/attendances/" + id + "", attendance).Result;                
                ViewBag.statusCode = Convert.ToInt32(response.StatusCode);                
=======
                HttpResponseMessage response = client.PostAsJsonAsync("api/attendances/" + id + "", attendance).Result;
                ViewBag.Message = response.Content.ReadAsStringAsync().Result;
                TempData["success"] = "success";
>>>>>>> eab0133b5e8f6e86eb09bb18611280e9b8dcee1c
            }
            return RedirectToAction("Index", "People");
        }

        [HttpPost]
        public IActionResult Update(int id,Attendance attendance)
        {
            id = Convert.ToInt32(Cache.GetStringValue("PersonId"));
            if (ModelState.IsValid)
            {
                HttpClient client = service.GetService();
                string stringData = JsonConvert.SerializeObject(attendance);
                var contentData = new StringContent(stringData, Encoding.UTF8, "application/json");
                HttpResponseMessage response = client.PutAsJsonAsync("api/attendances/"+id+"",attendance).Result;
                ViewBag.Message = response.Content.ReadAsStringAsync().Result;
                TempData["success"] = "success";
                //return RedirectToAction("index","People");
            }
            return RedirectToAction("Index", "People");
        }
    }
}
