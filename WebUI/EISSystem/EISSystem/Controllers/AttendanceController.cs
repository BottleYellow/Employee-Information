using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using EIS.Entities.Employee;
using EIS.WebAPI.RedisCache;
using EIS.WebApp.IServices;
using EIS.WebApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EIS.WebApp.Controllers
{
    public class AttendanceController : BaseController<Attendance>
    {
        public readonly IEISService<Attendance> service;
        RedisAgent Cache = new RedisAgent();
        public AttendanceController(IEISService<Attendance> service): base(service)
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

        public IActionResult GetAllAttendanceYearlyById(string date, string type)
        {
            int pid = Convert.ToInt32(Cache.GetStringValue("PersonId"));
            HttpResponseMessage respemp = service.GetResponse("api/employee/" + pid + "");
            string People = respemp.Content.ReadAsStringAsync().Result;
            Person people = JsonConvert.DeserializeObject<Person>(People);

            string[] monthyear = new string[2];
            monthyear = date.Split('/');

            if (type == "year")
            {
                HttpResponseMessage respattendance = service.GetResponse("api/Attendances/GetAllAttendanceYearly/" + pid + "/" + monthyear[0]);
                string Attendance = respattendance.Content.ReadAsStringAsync().Result;
                List<Attendance> attendances = JsonConvert.DeserializeObject<List<Attendance>>(Attendance);
                people.Attendance = attendances;

            }
            EmployeeAttendance employeeAttendance = new EmployeeAttendance();
            employeeAttendance.Persons = people;
            return PartialView(employeeAttendance);
        }


        [HttpGet]
        public IActionResult GetAllAttendance(string date,string type)
        {
            HttpResponseMessage respattendance;
            string[] monthyear = new string[3];
            string[] week = new string[2];
            if(date.Contains('-'))
            {
                week = date.Split('-'); 
            }
            else 
            {
                monthyear = date.Split('/');
            }

            respattendance = service.GetResponse("api/Employee");
            string People = respattendance.Content.ReadAsStringAsync().Result;
            IEnumerable<Person> people = JsonConvert.DeserializeObject<IEnumerable<Person>>(People);

            respattendance = service.GetResponse("api/Attendances");
            string Attendance = respattendance.Content.ReadAsStringAsync().Result;
            IEnumerable<Attendance> attendances = JsonConvert.DeserializeObject<IEnumerable<Attendance>>(Attendance);
            int pid = Convert.ToInt32(Cache.GetStringValue("PersonId"));
            Person person = new Person();
            if (type == "year")
            {
                ViewBag.val = "year";
                int year = Convert.ToInt32(monthyear[0]);
                ViewBag.yearval =year;
                people = from p in people
                         select (new Person
                         {
                             Id = p.Id,
                             FirstName = p.FirstName,
                             Attendance = (from a in attendances where a.PersonId == p.Id && a.DateIn.Year == year select a).ToList()
                         });
                person=people.Where(x=>x.Id==pid).FirstOrDefault();
            }
            else if (type == "month")
            {
                ViewBag.val = "month";
                int month = Convert.ToInt32(monthyear[0]);
                int year = Convert.ToInt32(monthyear[1]);
                people = from p in people
                         select (new Person
                         {
                             Id = p.Id,
                             FirstName = p.FirstName,
                             Attendance = (from a in attendances where a.PersonId == p.Id && a.DateIn.Year == year && a.DateIn.Month==month select a).ToList()
                         });
            }
            else
            {
                ViewBag.val = "week";
                DateTime startOfWeek = Convert.ToDateTime(week[0]);
                DateTime endOfWeek = Convert.ToDateTime(week[1]);
                people = from p in people
                         select (new Person
                         {
                             Id = p.Id,
                             FirstName = p.FirstName,
                             Attendance = (from a in attendances where a.PersonId == p.Id && (a.DateIn.Date >= startOfWeek && a.DateIn.Date <= endOfWeek) select a).ToList()
                         });
            }
                
            return PartialView("GetAllAttendanceMonthlyById",person);
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
