using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using EIS.Entities.Dashboard;
using EIS.Entities.Employee;
using EIS.Entities.Hoildays;
using EIS.Entities.Leave;
using EIS.WebApp.IServices;
using EIS.WebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace EIS.WebApp.Controllers
{
    public class DashboardController : BaseController<Person>
    {
        public readonly IServiceWrapper _services;
        public DashboardController(IEISService<Person> service, IServiceWrapper services):base(service)
        {
            _services = services;
        }
        public IActionResult AdminDashboard()
        {
            HttpResponseMessage response = _services.Employee.GetResponse("api/Dashboard/Admin");
            string stringData = response.Content.ReadAsStringAsync().Result;
            var dashBoard = JsonConvert.DeserializeObject<AdminDashboard>(stringData);
            return View(dashBoard);
        }

        public IActionResult ManagerDashboard()
        {
            HttpResponseMessage response = _services.Employee.GetResponse("api/Dashboard/Manager");
            string stringData = response.Content.ReadAsStringAsync().Result;
            var dashBoard = JsonConvert.DeserializeObject<ManagerDashboard>(stringData);
            return View(dashBoard);
        }

        public IActionResult EmployeeDashboard()
        {
            int PersonId = Convert.ToInt32(Cache.GetStringValue("PersonId"));
            HttpResponseMessage response = _services.Employee.GetResponse("api/Dashboard/Employee/"+PersonId+"");
            string stringData = response.Content.ReadAsStringAsync().Result;
            var dashBoard = JsonConvert.DeserializeObject<EmployeeDashboard>(stringData);
            return View(dashBoard);
        }

        public IActionResult Calendar()
        {
         
            return View();
        }

        [HttpPost]
        public IActionResult GetFullCalendar()
        {
            
            List<CalendarData> datas = new List<CalendarData>();
            HttpResponseMessage holidayResponse = _service.GetResponse("api/Holiday");
            string holidayData = holidayResponse.Content.ReadAsStringAsync().Result;
            List<Holiday> holidayList = JsonConvert.DeserializeObject<List<Holiday>>(holidayData);
            foreach (var d in holidayList)
            {
                CalendarData calendarData = new CalendarData();
                calendarData.Title = d.Vacation;
                calendarData.Description ="Holiday due to:-"+ d.Vacation;
                calendarData.StartDate = d.Date;
                calendarData.EndDate = d.Date;
                calendarData.Color = "Red";
                calendarData.AllDay = false;

                datas.Add(calendarData);
            }

            HttpResponseMessage leaveResponse = _service.GetResponse("api/LeaveRequest/GetLeaveRequests1");
            string leaveData = leaveResponse.Content.ReadAsStringAsync().Result;
            List<LeaveRequest> leaveList = JsonConvert.DeserializeObject<List<LeaveRequest>>(leaveData);
            foreach (var d in leaveList)
            {
                CalendarData calendarData = new CalendarData();
                string leave = "";
                if(d.LeaveType=="Casual Leave")
                {
                    leave = "CL";
                }
                calendarData.Title = d.EmployeeName+" ("+ leave + "-"+ d.Status+")";
                calendarData.Description = "Leave Status:-"+d.Status;
                calendarData.StartDate = d.FromDate;
                calendarData.EndDate = d.ToDate;
                if(d.Status=="Pending")
                { 
                calendarData.Color = "Orange";
                }
                else { 
                calendarData.Color = "Blue";
                }
                calendarData.AllDay = false;
                datas.Add(calendarData);
            }


            HttpResponseMessage response = _service.GetResponse("api/Attendances");
            string stringData = response.Content.ReadAsStringAsync().Result;
            List<Attendance> data = JsonConvert.DeserializeObject<List<Attendance>>(stringData);
            foreach (var d in data)
            {
                CalendarData calendarData = new CalendarData();
                calendarData.Title = d.Person.FirstName+d.Person.LastName+" ("+d.TimeIn.ToString(@"hh\:mm") +"-"+d.TimeOut.ToString(@"hh\:mm") + ")";
                calendarData.Description = "Working Hours:-"+d.TotalHours.ToString(@"hh\:mm");
                calendarData.StartDate = d.DateIn;
                calendarData.EndDate = d.DateOut;
                calendarData.Color = "Green";
                calendarData.AllDay = false;

                datas.Add(calendarData);
            }

            return Json(datas);
        }
    }

}
