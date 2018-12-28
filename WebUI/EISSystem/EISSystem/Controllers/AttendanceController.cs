using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using EIS.Entities.Employee;
using EIS.WebApp.IServices;
using EIS.WebApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EIS.WebApp.Controllers
{
    public class AttendanceController : Controller
    {
        public readonly IEISService<Attendance> service;
        public AttendanceController(IEISService<Attendance> service)
        {
            this.service = service;
        }
        // GET: /<controller>/
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
            string[] monthyear = new string[3];
            string[] week = new string[2];
            string attendance="";
            List<Person> person = new List<Person>();
            if(date.Contains('-'))
            {
                week = date.Split('-'); 
            }
            else 
            {
                monthyear = date.Split('/');
            }
            if (type == "year")
            { 
                respattendance = service.GetResponse("api/Attendances/GetAllAttendanceYearly/" + monthyear[0]);
                attendance = respattendance.Content.ReadAsStringAsync().Result;
            }
            var peoples = JsonConvert.DeserializeObject(attendance);
            person = JsonConvert.DeserializeObject<List<Person>>(peoples.ToString());
            return PartialView(person);
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
                
                //return RedirectToAction("index","People");
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
