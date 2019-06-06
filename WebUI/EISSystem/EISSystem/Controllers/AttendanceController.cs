using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using EIS.Entities.Employee;
using EIS.Entities.Models;
using EIS.Entities.SP;
using EIS.WebApp.Filters;
using EIS.WebApp.IServices;
using EIS.WebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace EIS.WebApp.Controllers
{
    [SessionTimeOut]
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
            ViewBag.Locations = GetLocations();
            return View();
        }
        [HttpPost]
        public IActionResult AllAttendance(string date, string type, int location,bool status)
        {
            string url = GetAllAttendanceData(date, type, location,status);
            HttpResponseMessage response = _service.GetResponse(url);
            string stringData = response.Content.ReadAsStringAsync().Result;
            Attendance_Report attendanceData = JsonConvert.DeserializeObject<Attendance_Report>(stringData);
            return Json(attendanceData);
        }

       
        #endregion

        #region Brief Attendance Reports
        [HttpGet]
        [DisplayName("Brief Attendance Report")]
        public IActionResult BriefAttendanceReport()
        {
            ViewBag.Locations = GetLocations();
            return View();
        }
        [HttpPost]
        public IActionResult BriefAttendanceReport(string date, string type, int location,bool status)
        {
            string url = GetAllAttendanceNewData(date, type, location,status);
            HttpResponseMessage response = _service.GetResponse(url);
            string stringData = response.Content.ReadAsStringAsync().Result;
            Attendance_Report_New attendanceData = JsonConvert.DeserializeObject<Attendance_Report_New>(stringData);
            return Json(attendanceData);
        }

        [HttpGet]
        [DisplayName("Leaves In Detail")]
        public IActionResult LeavesInDetail(string Type,string Year,string Month, string EmployeeCode)
        {
            HttpResponseMessage response = _service.GetResponse(ApiUrl + "/api/Attendances/GetLeavesInDetail/" + Type + "/" + Year + "/" + Month + "/" + EmployeeCode);
            string stringData = response.Content.ReadAsStringAsync().Result;
            LeavesInDetail  data = JsonConvert.DeserializeObject<LeavesInDetail>(stringData);
            return PartialView("LeavesInDetail", data);
        }
     
        #endregion

        #region Attendance Summary
        [HttpGet]
        [DisplayName("Attendance Summary")]
        public IActionResult AttendanceSummary()
        {
            ViewBag.Locations = GetLocations();
            return View();
        }
        //[HttpPost]
        //public IActionResult AttendanceSummary(string date, string type, int location)
        //{
        //    bool status = true;
        //    string url = GetAllAttendanceData(date, type, location,status);
        //    HttpResponseMessage response = _service.GetResponse(url);
        //    string stringData = response.Content.ReadAsStringAsync().Result;
        //    Attendance_Report attendanceData = JsonConvert.DeserializeObject<Attendance_Report>(stringData);
        //    return Json(attendanceData);
        //}
        [HttpPost]
        public IActionResult AttendanceSummary(string date, string type, int location, bool status)
        {
            string url = GetAllAttendanceData(date, type, location, status);
            HttpResponseMessage response = _service.GetResponse(url);
            string stringData = response.Content.ReadAsStringAsync().Result;
            Attendance_Report attendanceData = JsonConvert.DeserializeObject<Attendance_Report>(stringData);
            return Json(attendanceData);
        }
        #endregion

        #region[Attendance History]
        [DisplayName("My Attendance History")]
        [HttpGet]
        public IActionResult EmployeeReports()
        {
            return View();
        }
        [HttpPost]
        public IActionResult EmployeeReports(string date, string type)
        {
            //int id = Convert.ToInt32(GetSession().PersonId);
            string EmployeeCode = GetSession().EmployeeCode;
            string url = GetAttendanceSummaryData(date, type, EmployeeCode);
            HttpResponseMessage response = _service.GetResponse(url);
            string stringData = response.Content.ReadAsStringAsync().Result;
            EmployeeAttendanceReport attendanceReport = new EmployeeAttendanceReport();
            attendanceReport = JsonConvert.DeserializeObject<EmployeeAttendanceReport>(stringData);
            return Json(attendanceReport);
        }
        #endregion

        #region[Employee Attendance History]
        [DisplayName("Employee Attendance History")]
        [HttpGet]
        public IActionResult EmployeeAttendanceHistory(int loc)
        {
            HttpResponseMessage response = _service.GetResponse(ApiUrl + "/api/Employee");
            string stringData = response.Content.ReadAsStringAsync().Result;
            IList<Person> employeesdata = JsonConvert.DeserializeObject<IList<Person>>(stringData);
            IEnumerable<Person> employees = from e in employeesdata.Where(x => x.EmployeeCode != GetSession().EmployeeCode)
                                            select new Person
                                            {
                                                Id = e.Id,
                                                FirstName = e.FirstName + " " + e.LastName
                                            };
            ViewBag.Persons = employees.OrderBy(x => x.FirstName);
            ViewBag.Locations = GetLocations();
            return View(employees);
        }
        [HttpPost]
        public JsonResult EmployeeAttendanceHistory(string date, string type, string id)
        {
            string url = GetAttendanceSummaryData(date, type, id);
            HttpResponseMessage response = _service.GetResponse(url);
            string stringData = response.Content.ReadAsStringAsync().Result;
            EmployeeAttendanceReport attendanceReport = JsonConvert.DeserializeObject<EmployeeAttendanceReport>(stringData);
            return Json(attendanceReport);
        }
        #endregion

        #region[AttendanceInOut]
        [DisplayName("Create Attendance")]
        [HttpGet]
        public IActionResult AttendanceInOut()
        {
            Attendance attendance = new Attendance();
            int id = Convert.ToInt32(GetSession().PersonId);
            if (ModelState.IsValid)
            {
                HttpClient client = _service.GetService();
                string stringData = JsonConvert.SerializeObject(attendance);
                var contentData = new StringContent(stringData, Encoding.UTF8, "application/json");
                HttpResponseMessage response = client.PostAsJsonAsync(ApiUrl + "api/attendances/" + id + "", attendance).Result;
                ViewBag.statusCode = Convert.ToInt32(response.StatusCode);
            }
            return RedirectToAction("Profile", "People", new { PersonId = GetSession().EmployeeCode });
        }

        [HttpPost]
        public IActionResult AttendanceInOut(int id, Attendance attendance)
        {
            id = Convert.ToInt32(GetSession().PersonId);
            //id = Convert.ToInt32(Cache.GetStringValue("PersonId"));
            if (ModelState.IsValid)
            {
                HttpClient client = _service.GetService();
                string stringData = JsonConvert.SerializeObject(attendance);
                var contentData = new StringContent(stringData, Encoding.UTF8, "application/json");
                HttpResponseMessage response = client.PutAsJsonAsync(ApiUrl + "/api/attendances/" + id + "", attendance).Result;
                string result = response.Content.ReadAsStringAsync().Result;
                Attendance attendances = JsonConvert.DeserializeObject<Attendance>(result);
                ViewBag.TotalHrs = attendances.TotalHours;
            }
            return RedirectToAction("Profile", "People", new { PersonId = GetSession().EmployeeCode });
        }
        #endregion

        #region Datewise Attendance
        [DisplayName("Attendance Datewise")]
        [HttpGet]
        public IActionResult DateWiseAttendance()
        {
            int id = Convert.ToInt32(GetSession().PersonId);
            //int id = Convert.ToInt32(Cache.GetStringValue("PersonId"));
            HttpResponseMessage response = _service.GetResponse(ApiUrl + "/api/Employee");
            string stringData = response.Content.ReadAsStringAsync().Result;
            IList<Person> employeesdata = JsonConvert.DeserializeObject<IList<Person>>(stringData);
            IEnumerable<Person> employees = from e in employeesdata.Where(x => x.EmployeeCode != GetSession().EmployeeCode)
                                            select new Person
                                            {
                                                Id = e.Id,
                                                EmployeeCode = e.EmployeeCode,
                                                FirstName = e.FirstName + " " + e.LastName
                                            };
            ViewBag.Persons = employees.OrderBy(x => x.FullName);
            ViewBag.Locations = GetLocations();
            return View(employees);
        }

        [ActionName("DateWiseAttendance")]
        [HttpPost]
        public IActionResult GetDateWiseAttendance(string fromdate, string todate, string id, int LocationId)
        {
            string url = "";
            if (!string.IsNullOrEmpty(fromdate) && !string.IsNullOrEmpty(todate))
            {
                url = ApiUrl + "/api/Attendances/GetDateWiseAttendance/" + id + "/" + LocationId + "/" + Convert.ToDateTime(fromdate).ToString("MMM-dd-yyyy") + "/" + Convert.ToDateTime(todate).ToString("MMM-dd-yyyy");
            }
            HttpResponseMessage response = _service.GetResponse(url);
            string stringData = response.Content.ReadAsStringAsync().Result;
            List<SP_GetDateWiseAttendance> attendance = JsonConvert.DeserializeObject<List<SP_GetDateWiseAttendance>>(stringData);
            return Json(attendance);
        }
        #endregion

        #region Attendance Modify and Deduct From Salary
        //[DisplayName("Attendance Modify")]
        //[HttpGet]
        //public IActionResult AttendanceUpdate()
        //{
        //    return View();
        //}

        //[ActionName("AttendanceUpdate")]
        //[HttpPost]
        //public IActionResult AttendanceUpdate(string personId, string message,string requestedDate, string timeIn, string timeOut)
        //{
        //    string url = "";
        //    if (!string.IsNullOrEmpty(requestedDate))
        //    {
        //        url = ApiUrl + "/api/Attendances/AttendanceUpdate/" + personId + "/" + message + "/" + Convert.ToDateTime(requestedDate).ToString("MMM-dd-yyyy") + "/" + timeIn + "/"+ timeOut;
        //    }
        //    HttpResponseMessage response = _service.GetResponse(url);
        //    string stringData = response.Content.ReadAsStringAsync().Result;
        //    string data = JsonConvert.DeserializeObject<string>(stringData);
        //    return Json(data);
        //}
        [DisplayName("Deduct From Salary")]
        [HttpPost]
        public IActionResult DeductFromSalary(string EmployeeCode,string Dates)
        {
            HttpResponseMessage response = _service.GetResponse(ApiUrl + "/api/Attendances/DeductFromSalary/" + EmployeeCode+"/"+Dates);
            string stringData = response.Content.ReadAsStringAsync().Result;
            string data = JsonConvert.DeserializeObject<string>(stringData);

            return Json(data);
        }
        #endregion

        #region[Methods]
        [NonAction]
        public string GetAttendanceSummaryData(string date, string type, string id)
        {

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
                url = ApiUrl + "/api/Attendances/GetYearlyAttendanceSummaryById/" + id + "/" + monthYear[0];
            }
            else if (type == "month")
            {
                url = ApiUrl + "/api/Attendances/GetMonthlyAttendanceSummaryById/" + id + "/" + monthYear[1] + "/" + monthYear[0];

            }
            else if (type == "week")
            {
                if (!string.IsNullOrEmpty(week[0]) && !string.IsNullOrEmpty(week[1]))
                {
                    DateTime startDate = Convert.ToDateTime(week[0]);
                    DateTime endDate = Convert.ToDateTime(week[1]);
                    url = ApiUrl + "/api/Attendances/GetWeeklyAttendanceSummaryById/" + id + "/" + startDate.ToString("MMM-dd-yyyy") + "/" + endDate.ToString("MMM-dd-yyyy");
                }
            }
            return url;
        }
        [NonAction]
        public string GetAllAttendanceData(string date, string type, int location,bool status)
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
                url = ApiUrl + "/api/Attendances/GetAllAttendanceEmpCount/Year/" + monthYear[0] + "/0/" + location+"/"+status;
            }
            else if (type == "week")
            {
                DateTime startDate = week[0] == null ? new DateTime(2019, 01, 30) : Convert.ToDateTime(week[0]);
                DateTime endDate = week[1] == null ? new DateTime(2019, 01, 05) : Convert.ToDateTime(week[1]);
                url = ApiUrl + "/api/Attendances/GetAllAttendanceEmpCount/Week/" + startDate.ToString("MMM-dd-yyyy") + "/" + endDate.ToString("MMM-dd-yyyy") + "/" + location+"/"+status;
            }
            else
            {
                url = ApiUrl + "/api/Attendances/GetAllAttendanceEmpCount/Month/" + monthYear[0] + "/" + monthYear[1] + "/" + location+"/"+status;
            }
            return url;
        }
        [NonAction]
        public string GetAllAttendanceNewData(string date, string type, int location,bool status)
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
                url = ApiUrl + "/api/Attendances/GetAllAttendanceNew/Year/" + monthYear[0] + "/0/" + location+"/"+ status;
            }
            else if (type == "week")
            {
                DateTime startDate = week[0] == null ? new DateTime(2019, 01, 30) : Convert.ToDateTime(week[0]);
                DateTime endDate = week[1] == null ? new DateTime(2019, 01, 05) : Convert.ToDateTime(week[1]);
                url = ApiUrl + "/api/Attendances/GetAllAttendanceNew/Week/" + startDate.ToString("MMM-dd-yyyy") + "/" + endDate.ToString("MMM-dd-yyyy") + "/" + location+"/"+ status;
            }
            else
            {
                url = ApiUrl + "/api/Attendances/GetAllAttendanceNew/Month/" + monthYear[0] + "/" + monthYear[1] + "/" + location+"/"+ status;
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
                url = ApiUrl + "/api/Attendances/GetYearlyAttendanceById/" + pId + "/" + monthYear[0];
            }
            else if (type == "month")
            {
                url = ApiUrl + "/api/Attendances/GetMonthlyAttendanceById/" + pId + "/" + monthYear[1] + "/" + monthYear[0];

            }
            else if (type == "week")
            {

                DateTime startDate = week[0] == null ? new DateTime(2018, 12, 30) : Convert.ToDateTime(week[0]);
                DateTime endDate = week[1] == null ? new DateTime(2019, 01, 05) : Convert.ToDateTime(week[1]);
                ViewBag.startDate = startDate;
                url = ApiUrl + "/api/Attendances/GetWeeklyAttendanceById/" + pId + "/" + startDate.ToString("MMM-dd-yyyy") + "/" + endDate.ToString("MMM-dd-yyyy");
            }
            return url;
        }
        #endregion
    }
}