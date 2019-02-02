using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using EIS.Entities.Employee;
using EIS.WebApp.IServices;
using EIS.WebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace EIS.WebApp.Controllers
{
    [DisplayName("Attendance Management")]
    public class AttendanceController : BaseController<Attendance>
    {
        public AttendanceController(IEISService<Attendance> service) : base(service)
        {
        }

        #region[AllAttendance]
        [HttpGet]
        [DisplayName("Attendance Reports")]
        public IActionResult AllAttendance()
        {
            return View();
        }
        [HttpPost]
        public IActionResult AllAttendance(string date, string type)
        {
            string url = GetAllAttendanceData(date, type);
           return LoadData<Person>(url,null);

        }
        #endregion

        #region[Attendance History]
        [DisplayName("My Attendance History")]
        [HttpGet]
        public IActionResult EmployeeReports()
        {
            return View();
        }

        [DisplayName("My Attendance History")]
        [HttpPost]
        public IActionResult EmployeeReports(string date, string type)
        {
            int pId = Convert.ToInt32(Cache.GetStringValue("PersonId"));
            string url = GetAttendanceByIdData(date, type, pId);
            return LoadData<Attendance>(url,null);

        }    

        [HttpPost]
        public IActionResult GetAttendanceSummary(string date, string type)
        {
            int id = Convert.ToInt32(Cache.GetStringValue("PersonId"));
            string url = GetAttendanceSummaryData(date, type, id);
            HttpResponseMessage response = _service.GetResponse(url );
            string stringData = response.Content.ReadAsStringAsync().Result;
            AttendanceReport attendanceReport = new AttendanceReport();
            attendanceReport = JsonConvert.DeserializeObject<AttendanceReport>(stringData);
            return Json(attendanceReport);
        }
        #endregion

        #region[Employee Attendance History]
        [DisplayName("Employee Attendance History")]
        [HttpGet]
        public IActionResult AttendanceSummary()
        {
            HttpResponseMessage response = _service.GetResponse(ApiUrl+"/api/Employee" );
            string stringData = response.Content.ReadAsStringAsync().Result;
            IList<Person> employeesdata = JsonConvert.DeserializeObject<IList<Person>>(stringData);
            IEnumerable<Person> employees = from e in employeesdata.Where(x=>x.EmployeeCode!=Cache.GetStringValue("EmployeeCode"))
                            select new Person
                            {
                                Id = e.Id,
                                FirstName = e.FirstName + " " + e.LastName
                            };
            ViewBag.Persons = employees;
            return View(employees);
        }
        [ActionName("AttendanceSummary")]
        [HttpPost]
        public IActionResult EmployeeReportsById(string date, string type, int? id)
        {
            if (id == null)
            {
                id = Convert.ToInt32(Cache.GetStringValue("PersonId"));
            }
            //date = Convert.ToDateTime(date).ToShortDateString();
            string url = GetAttendanceByIdData(date, type, id);
            ArrayList arrayData = new ArrayList();
            return LoadData<Attendance>(url,null);

        }
        [ActionName("AttendanceSummary")]
        [HttpPut]
        public JsonResult AttendanceSummaryById(string date, string type, int? id)
        {
            string url = GetAttendanceSummaryData(date, type, id);
            HttpResponseMessage response = _service.GetResponse(url );
            string stringData = response.Content.ReadAsStringAsync().Result;
            AttendanceReport attendanceReport = new AttendanceReport();
            attendanceReport = JsonConvert.DeserializeObject<AttendanceReport>(stringData);
            return Json(attendanceReport);
        }
        #endregion

        #region[AttendanceInOut]
        [DisplayName("Create Attendance")]
        [HttpGet]
        public IActionResult AttendanceInOut()
        {
            Attendance attendance = new Attendance();
            int id = Convert.ToInt32(Cache.GetStringValue("PersonId"));
            if (ModelState.IsValid)
            {
                HttpClient client = _service.GetService();
                string stringData = JsonConvert.SerializeObject(attendance);
                var contentData = new StringContent(stringData, Encoding.UTF8, "application/json");
                HttpResponseMessage response = client.PostAsJsonAsync(ApiUrl+"/api/attendances/" + id + "", attendance).Result;
                ViewBag.statusCode = Convert.ToInt32(response.StatusCode);
            }
            return RedirectToAction("Profile","People",new { PersonId = Cache.GetStringValue("EmployeeCode") });
        }

        [HttpPost]
        public IActionResult AttendanceInOut(int id, Attendance attendance)
        {
            id = Convert.ToInt32(Cache.GetStringValue("PersonId"));
            if (ModelState.IsValid)
            {
                HttpClient client = _service.GetService();
                string stringData = JsonConvert.SerializeObject(attendance);
                var contentData = new StringContent(stringData, Encoding.UTF8, "application/json");
                HttpResponseMessage response = client.PutAsJsonAsync(ApiUrl+"/api/attendances/" + id + "", attendance).Result;
                string result = response.Content.ReadAsStringAsync().Result;
                Attendance attendances = JsonConvert.DeserializeObject<Attendance>(result);
                ViewBag.TotalHrs = attendances.TotalHours;
            }
            return RedirectToAction("Profile", "People", new { PersonId = Cache.GetStringValue("EmployeeCode") });
        }
        #endregion

        //public IActionResult GetEmployeeAttendance()
        //{
        //    HttpResponseMessage response = _service.GetResponse(ApiUrl+"/api/Employee");
        //    string stringData = response.Content.ReadAsStringAsync().Result;
        //    IList<Person> employeesdata = JsonConvert.DeserializeObject<IList<Person>>(stringData);
        //    IEnumerable<Person> employees = from e in employeesdata
        //                    select new Person
        //                    {
        //                        Id = e.Id,
        //                        FirstName = e.FirstName + " " + e.LastName
        //                    };
        //    ViewBag.Persons = employees;
        //    return View(employees);
        //}


        #region[Method]
        [NonAction]
        public string GetAttendanceSummaryData(string date, string type, int? id)
        {
            if (id == null)
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
                url = ApiUrl+"/api/Attendances/GetYearlyAttendanceSummaryById/" + id + "/" + monthYear[0];
            }
            else if (type == "month")
            {
                url = ApiUrl+"/api/Attendances/GetMonthlyAttendanceSummaryById/" + id + "/" + monthYear[1] + "/" + monthYear[0];

            }
            else if (type == "week")
            {

                DateTime startDate = Convert.ToDateTime(week[0]);
                DateTime endDate =  Convert.ToDateTime(week[1]);
                url = ApiUrl+"/api/Attendances/GetWeeklyAttendanceSummaryById/" + id + "/" + startDate.ToString("dd-MM-yyyy")+ "/" + endDate.ToString("dd-MM-yyyy");
            }
            return url;
        }
        [NonAction]
        public string GetAllAttendanceData(string date, string type)
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
                url = ApiUrl+"/api/Attendances/GetAllAttendanceYearly/" + monthYear[0];
            }
            else if (type == "week")
            {
                DateTime startDate = DateTime.ParseExact(week[0], "dd/MM/yyyy", CultureInfo.InvariantCulture);
                DateTime endDate = week[1] == null ? new DateTime(2019, 01, 05) : Convert.ToDateTime(week[1]);
                url = ApiUrl+"/api/Attendances/GetAllAttendanceWeekly/" + startDate.ToString("dd-MM-yyyy") + "/" + endDate.ToString("dd-MM-yyyy");
            }
            else
            {
                url = ApiUrl+"/api/Attendances/GetAllAttendanceMonthly/" + monthYear[0] + "/" + monthYear[1];
            }
            return url;
        }
        [NonAction]
        public string GetAttendanceByIdData(string date, string type, int? pId)
        {
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
                url = ApiUrl+"/api/Attendances/GetYearlyAttendanceById/" + pId + "/" + monthYear[0];
            }
            else if (type == "month")
            {
                url = ApiUrl+"/api/Attendances/GetMonthlyAttendanceById/" + pId + "/" + monthYear[1] + "/" + monthYear[0];

            }
            else if (type == "week")
            {
                DateTime startDate = week[0] == null ? new DateTime(2018, 12, 30) : Convert.ToDateTime(week[0]);
                DateTime endDate = week[1] == null ? new DateTime(2019, 01, 05) : Convert.ToDateTime(week[1]);
                ViewBag.startDate = startDate;
                url = ApiUrl+"/api/Attendances/GetWeeklyAttendanceById/" + pId + "/" + startDate.ToString("dd-MM-yyyy") + "/" + endDate.ToString("dd-MM-yyyy");
            }
            return url;
        }
        #endregion


        [DisplayName("Attendance Datewise")]
        [HttpGet]
        public IActionResult DateWiseAttendance()
        {
            int id = Convert.ToInt32(Cache.GetStringValue("PersonId"));
            HttpResponseMessage response = _service.GetResponse(ApiUrl+"/api/Employee");
            string stringData = response.Content.ReadAsStringAsync().Result;
            IList<Person> employeesdata = JsonConvert.DeserializeObject<IList<Person>>(stringData);
            IEnumerable<Person> employees = from e in employeesdata.Where(x=>x.EmployeeCode!=Cache.GetStringValue("EmployeeCode"))
                            select new Person
                            {
                                EmployeeCode = e.EmployeeCode,
                                FirstName = e.FirstName + " " + e.LastName
                            };
            ViewBag.Persons = employees;
            return View(employees);
        }
        
        [ActionName("DateWiseAttendance")]
        [HttpPost]
        public IActionResult GetDateWiseAttendance(string fromdate,string todate,string id)
        {
            
            string url = ApiUrl+"/api/Attendances/GetDateWiseAttendance/" + id + "/" + Convert.ToDateTime(fromdate).ToString("dd-MM-yyyy") + "/" + Convert.ToDateTime(todate).ToString("dd-MM-yyyy");
            ArrayList arrayData = new ArrayList();
            return LoadData<Attendance>(url,null);


        }
    }
}