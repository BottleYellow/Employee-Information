using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using EIS.Entities.Employee;
using EIS.WebApp.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EIS.WebApp.Controllers
{
    public class AttendanceController : Controller
    {
        public readonly IEISService service;
        public AttendanceController(IEISService service)
        {
            this.service = service;
        }
        // GET: /<controller>/
        public IActionResult Index()
        {
            int id = Convert.ToInt32(HttpContext.Session.GetString("id"));
            HttpResponseMessage response = service.GetResponse("api/attendances/" + id + "");
            string stringData = response.Content.ReadAsStringAsync().Result;
            List<Attendance> data = JsonConvert.DeserializeObject<List<Attendance>>(stringData);
            return View(data);
        }

        [HttpGet]
        public IActionResult GetAllAttendance()
        {
            HttpResponseMessage response = service.GetResponse("api/attendances");
            string stringData = response.Content.ReadAsStringAsync().Result;
            List<Attendance> data = JsonConvert.DeserializeObject<List<Attendance>>(stringData);
            Response.StatusCode = (int)response.StatusCode;
            return View("AllAttendance",data);
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
