using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using EIS.Entities.Employee;
using EIS.WebApp.IServices;
using EIS.WebApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace EIS.WebApp.Controllers
{
    public class AttendanceController : Controller
    {
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
                attendance = respattendance.Content.ReadAsStringAsync().Result;
            }
            else if (type == "week")
            {
                ViewBag.type = type;
                respattendance = service.GetResponse("api/Attendances/GetAllAttendanceWeekly/" + Convert.ToDateTime(week[0]) + "/" + Convert.ToDateTime(week[1]));
                attendance = respattendance.Content.ReadAsStringAsync().Result;
            }
            attendances = JsonConvert.DeserializeObject<List<Person>>(attendance);
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
            int id = Convert.ToInt32(HttpContext.Session.GetString("id"));
            if (ModelState.IsValid)
            {
                HttpClient client = service.GetService();
                string stringData = JsonConvert.SerializeObject(attendance);
                var contentData = new StringContent(stringData, Encoding.UTF8, "application/json");
                HttpResponseMessage response = client.PostAsJsonAsync("api/attendances/" + id + "", attendance).Result;
                ViewBag.Message = response.Content.ReadAsStringAsync().Result;
                TempData["success"] = "success";
            }
            return RedirectToAction("Index", "People");
        }

        [HttpPost]
        public IActionResult Update(int id,Attendance attendance)
        {
            id= Convert.ToInt32(HttpContext.Session.GetString("id"));
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
