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
            ArrayList arrayData = new ArrayList();
           arrayData = LoadData<Person>(url);
            int recordsTotal = JsonConvert.DeserializeObject<int>(arrayData[0].ToString());
            IList<Person> data = JsonConvert.DeserializeObject<IList<Person>>(arrayData[1].ToString());
            return Json(new { recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
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
            ArrayList arrayData = new ArrayList();
            arrayData = LoadData<Attendance>(url);

            int recordsTotal = JsonConvert.DeserializeObject<int>(arrayData[0].ToString());
            IEnumerable<Attendance> data = JsonConvert.DeserializeObject<IEnumerable<Attendance>>(arrayData[1].ToString());
            return Json(new { recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
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
            HttpResponseMessage response = _service.GetResponse("api/Employee" );
            string stringData = response.Content.ReadAsStringAsync().Result;
            IList<Person> employeesdata = JsonConvert.DeserializeObject<IList<Person>>(stringData);
            var employees = from e in employeesdata.Where(x=>x.EmployeeCode!=Cache.GetStringValue("EmployeeCode"))
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
        public JsonResult EmployeeReportsById(string date, string type, int? id)
        {
            if (id == null)
            {
                id = Convert.ToInt32(Cache.GetStringValue("PersonId"));
            }
            string url = GetAttendanceByIdData(date, type, id);
            ArrayList arrayData = new ArrayList();
            arrayData = LoadData<Attendance>(url);

            int recordsTotal = JsonConvert.DeserializeObject<int>(arrayData[0].ToString());
            IList<Attendance> data = JsonConvert.DeserializeObject<IList<Attendance>>(arrayData[1].ToString());
            return Json(new { recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
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
            var attendance = new Attendance();
            int id = Convert.ToInt32(Cache.GetStringValue("PersonId"));
            if (ModelState.IsValid)
            {
                HttpClient client = _service.GetService();
                string stringData = JsonConvert.SerializeObject(attendance);
                var contentData = new StringContent(stringData, Encoding.UTF8, "application/json");
                HttpResponseMessage response = client.PostAsJsonAsync("api/attendances/" + id + "", attendance).Result;
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
                HttpResponseMessage response = client.PutAsJsonAsync("api/attendances/" + id + "", attendance).Result;
                string result = response.Content.ReadAsStringAsync().Result;
                Attendance attendances = JsonConvert.DeserializeObject<Attendance>(result);
                ViewBag.TotalHrs = attendances.TotalHours;
            }
            return RedirectToAction("Profile", "People", new { PersonId = Cache.GetStringValue("EmployeeCode") });
        }
        #endregion

        public IActionResult GetEmployeeAttendance()
        {
            HttpResponseMessage response = _service.GetResponse("api/Employee");
            string stringData = response.Content.ReadAsStringAsync().Result;
            IList<Person> employeesdata = JsonConvert.DeserializeObject<IList<Person>>(stringData);
            var employees = from e in employeesdata
                            select new Person
                            {
                                Id = e.Id,
                                FirstName = e.FirstName + " " + e.LastName
                            };
            ViewBag.Persons = employees;
            return View(employees);
        }


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
                url = "api/Attendances/GetYearlyAttendanceSummaryById/" + id + "/" + monthYear[0];
            }
            else if (type == "month")
            {
                url = "api/Attendances/GetMonthlyAttendanceSummaryById/" + id + "/" + monthYear[1] + "/" + monthYear[0];

            }
            else if (type == "week")
            {

                DateTime startDate = week[0] == null ? new DateTime(2018, 12, 30) : Convert.ToDateTime(week[0]);
                DateTime endDate = week[1] == null ? new DateTime(2019, 01, 05) : Convert.ToDateTime(week[1]);
                url = "api/Attendances/GetWeeklyAttendanceSummaryById/" + id + "/" + startDate.ToString("dd-MM-yyyy")+ "/" + endDate.ToString("dd-MM-yyyy");
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
                url = "api/Attendances/GetAllAttendanceYearly/" + monthYear[0];
            }
            else if (type == "week")
            {
                DateTime startDate = week[0] == null ? new DateTime(2018, 12, 30) : Convert.ToDateTime(week[0]);
                DateTime endDate = week[1] == null ? new DateTime(2019, 01, 05) : Convert.ToDateTime(week[1]);
                url = "api/Attendances/GetAllAttendanceWeekly/" + startDate.ToString("dd-MM-yyyy") + "/" + endDate.ToString("dd-MM-yyyy");
            }
            else
            {
                url = "api/Attendances/GetAllAttendanceMonthly/" + monthYear[0] + "/" + monthYear[1];
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
                url = "api/Attendances/GetYearlyAttendanceById/" + pId + "/" + monthYear[0];
            }
            else if (type == "month")
            {
                url = "api/Attendances/GetMonthlyAttendanceById/" + pId + "/" + monthYear[1] + "/" + monthYear[0];

            }
            else if (type == "week")
            {
                DateTime startDate = week[0] == null ? new DateTime(2018, 12, 30) : Convert.ToDateTime(week[0]);
                DateTime endDate = week[1] == null ? new DateTime(2019, 01, 05) : Convert.ToDateTime(week[1]);
                ViewBag.startDate = startDate;
                url = "api/Attendances/GetWeeklyAttendanceById/" + pId + "/" + startDate.ToString("dd-MM-yyyy") + "/" + endDate.ToString("dd-MM-yyyy");
            }
            return url;
        }
        #endregion


        [DisplayName("DateWiseAttendance")]
        [HttpGet]
        public IActionResult DateWiseAttendance()
        {
            int id = Convert.ToInt32(Cache.GetStringValue("PersonId"));
            HttpResponseMessage response = _service.GetResponse("api/Employee");
            string stringData = response.Content.ReadAsStringAsync().Result;
            IList<Person> employeesdata = JsonConvert.DeserializeObject<IList<Person>>(stringData);
            var employees = from e in employeesdata.Where(x=>x.EmployeeCode!=Cache.GetStringValue("EmployeeCode"))
                            select new Person
                            {
                                EmployeeCode = e.EmployeeCode,
                                FirstName = e.FirstName + " " + e.LastName
                            };
            ViewBag.Persons = employees;
            return View(employees);
        }
        
        [DisplayName("DateWiseAttendance")]
        [HttpPost]
        public IActionResult GetDateWiseAttendance(string fromdate,string todate,string id)
        {
            
            string url = url = "api/Attendances/GetDateWiseAttendance/" + id + "/" + Convert.ToDateTime(fromdate).ToString("dd-MM-yyyy") + "/" + Convert.ToDateTime(todate).ToString("dd-MM-yyyy");
            ArrayList arrayData = new ArrayList();
            arrayData = LoadData<Attendance>(url);

            int recordsTotal = JsonConvert.DeserializeObject<int>(arrayData[0].ToString());
            IEnumerable<Attendance> data = JsonConvert.DeserializeObject<IEnumerable<Attendance>>(arrayData[1].ToString());
            return Json(new { recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });

        }
    }
}