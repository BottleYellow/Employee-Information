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
        //public IActionResult AllAttendance()
        //{
        //    EmployeeAttendance employeeAttendance = new EmployeeAttendance();
        //    return View("AllAttendance", employeeAttendance);
        //}
        //[HttpGet]
        //public IActionResult GetAllAttendanceYearly(DateTime date, string type)
        //{
        //    HttpResponseMessage respemp = service.GetResponse("api/Employee");
        //    string People = respemp.Content.ReadAsStringAsync().Result;
        //    List<Person> people = JsonConvert.DeserializeObject<List<Person>>(People);

        //    foreach (var p in people)
        //    {
        //        HttpResponseMessage respattendance = service.GetResponse("api/Attendances/" + p.Id + "");
        //        string Attendance = respattendance.Content.ReadAsStringAsync().Result;
        //        List<Attendance> attendances = JsonConvert.DeserializeObject<List<Attendance>>(Attendance);
        //        ICollection<Attendance> attendance = new Collection<Attendance>();
        //        foreach (var a in attendances)
        //        {
        //            if (type == "year")
        //            {
        //                if (a.CreatedDate.Year == date.Year)
        //                {
        //                    attendance.Add(a);
        //                }
        //            }
        //            else if (type == "month")
        //            {
        //                if (a.CreatedDate.Month == date.Month && a.CreatedDate.Year == date.Year)
        //                {
        //                    attendance.Add(a);
        //                }
        //            }

        //            //if(val[0]==null)
        //            //{
        //            //    if (a.CreatedDate.Year == Convert.ToInt32(val[1]))
        //            //    {
        //            //        attendance.Add(a);
        //            //    }
        //            //}
        //            //else
        //            //{
        //            //    if (a.CreatedDate.Month== Convert.ToInt32(val[0]) && a.CreatedDate.Year == Convert.ToInt32(val[1]))
        //            //    {
        //            //        attendance.Add(a);
        //            //    }
        //            //}

        //        }
        //        p.Attendance = attendance;
        //    }

        //    EmployeeAttendance employeeAttendance = new EmployeeAttendance();
        //    employeeAttendance.Persons = people.ToList();
        //    return PartialView(employeeAttendance);
        //}

        [HttpGet]
        public IActionResult GetAllAttendance()
        {
            HttpResponseMessage response = service.GetResponse("api/attendances");
            string stringData = response.Content.ReadAsStringAsync().Result;
            List<Attendance> data = JsonConvert.DeserializeObject<List<Attendance>>(stringData);
            Response.StatusCode = (int)response.StatusCode;
            return View("AllAttendance", data);
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
        public IActionResult Update(int id, Attendance attendance)
        {
            id = Convert.ToInt32(HttpContext.Session.GetString("id"));
            if (ModelState.IsValid)
            {
                HttpClient client = service.GetService();
                string stringData = JsonConvert.SerializeObject(attendance);
                var contentData = new StringContent(stringData, Encoding.UTF8, "application/json");
                HttpResponseMessage response = client.PutAsJsonAsync("api/attendances/" + id + "", attendance).Result;
                ViewBag.Message = response.Content.ReadAsStringAsync().Result;
                TempData["success"] = "success";
                //return RedirectToAction("index","People");
            }
            return RedirectToAction("Index", "People");
        }
    }
}
