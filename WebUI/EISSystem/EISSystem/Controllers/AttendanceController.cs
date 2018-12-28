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
        public IActionResult GetAllAttendanceYearly(string date,string type)
        {
            HttpResponseMessage respemp = service.GetResponse("api/Employee");
            string People = respemp.Content.ReadAsStringAsync().Result;
            List<Person> people = JsonConvert.DeserializeObject<List<Person>>(People);

            string[] monthyear = new string[2];
            monthyear = date.Split('/');
            foreach(var p in people)
            {
                if (type=="year")
                {
                    HttpResponseMessage respattendance = service.GetResponse("api/Attendances/GetAllAttendanceYearly/" + p.Id + "/" + monthyear[0]);
                    string Attendance = respattendance.Content.ReadAsStringAsync().Result;
                    List<Attendance> attendances = JsonConvert.DeserializeObject<List<Attendance>>(Attendance);
                    p.Attendance = attendances;
                }
                else
                {
                    HttpResponseMessage respattendance = service.GetResponse("api/Attendances/GetAllAttendanceMonthly/" + p.Id + "/" + monthyear[0] + "/" + monthyear[1]);
                    string Attendance = respattendance.Content.ReadAsStringAsync().Result;
                    List<Attendance> attendances = JsonConvert.DeserializeObject<List<Attendance>>(Attendance);
                    p.Attendance = attendances;
                }
            }

            EmployeeAttendance employeeAttendance = new EmployeeAttendance();
            employeeAttendance.Persons = people.ToList();
            return PartialView(employeeAttendance);
        }

        //[HttpGet]
        //public IActionResult GetAllAttendance()
        //{
        //    HttpResponseMessage response = service.GetResponse("api/attendances");
        //    string stringData = response.Content.ReadAsStringAsync().Result;
        //    List<Attendance> data = JsonConvert.DeserializeObject<List<Attendance>>(stringData);
        //    Response.StatusCode = (int)response.StatusCode;
        //    return View("AllAttendance",data);
        //}

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
