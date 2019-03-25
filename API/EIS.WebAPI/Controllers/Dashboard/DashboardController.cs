using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using EIS.Entities.Dashboard;
using EIS.Entities.Employee;
using EIS.Entities.Models;
using EIS.Entities.SP;
using EIS.Repositories.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace EIS.WebAPI.Controllers.Dashboard
{
    [Route("api/Dashboard")]
    [ApiController]
    public class DashboardController : BaseController
    {
        public DashboardController(IRepositoryWrapper repository) : base(repository)
        { }

        [Route("Admin/{attendanceStatus}/{location}/{date}")]
        [HttpGet]
        public IActionResult GetAdminDashboard(string attendanceStatus, int location, string date)
        {
            Admin_Dashboard dashboard = _repository.Dashboard.GetAdminDashboard(attendanceStatus, location, TenantId, date);
            return Ok(dashboard);
        }

        [Route("Manager")]
        [HttpGet]
        public IActionResult GetManagerDashboard()
        {
            ManagerDashboard dashboard = _repository.Dashboard.GetManagerDashboard(TenantId);
            return Ok(dashboard);
        }

        [Route("Employee/{PersonId}")]
        [HttpGet]
        public IActionResult GetEmployeeDashboard([FromRoute]int PersonId)
        {
            Person person = _repository.Employee.FindByCondition(x => x.Id == PersonId);
            if (person.IsOnProbation == true)
            {
                int probationPeriod = person.PropbationPeriodInMonth.GetValueOrDefault();
                DateTime joinDate = person.JoinDate;
                DateTime probationEndDate = joinDate.AddMonths(probationPeriod);
                if (probationEndDate.Date < DateTime.Now.Date)
                {
                    person.IsOnProbation = false;
                    person.PropbationPeriodInMonth = null;
                    _repository.Employee.UpdateAndSave(person);
                }
            }
            Employee_Dashboard dashboard = _repository.Dashboard.GetEmployeeDashboard(TenantId, PersonId);
            return Ok(dashboard);
        }

        [Route("AdminCalendarData/{location}/{startDate}/{endDate}")]
        [HttpGet]
        public IActionResult GetAdminCalendarData([FromRoute]int location, [FromRoute]string startDate, [FromRoute]string endDate)
        {
            List<CalendarData> calendarDataList = _repository.Dashboard.GetAdminCalendarDetails(location, Convert.ToDateTime(startDate), Convert.ToDateTime(endDate));
            return Ok(calendarDataList);
        }

        [Route("EmployeeCalendarData/{personId}/{startDate}/{endDate}")]
        [HttpGet]
        public IActionResult GetEmployeeCalendarData([FromRoute]int personId, [FromRoute]string startDate, [FromRoute]string endDate)
        {
            List<CalendarData> calendarDataList = _repository.Dashboard.GetEmployeeCalendarDetails(personId, Convert.ToDateTime(startDate), Convert.ToDateTime(endDate));
            return Ok(calendarDataList);
        }

        [Route("BirthdayData/{day}/{month}")]
        [HttpGet]
        public IActionResult GetAllEmployeeBirthday([FromRoute]int day, [FromRoute]int month)
        {
            List<Person> person = _repository.Employee.FindAllByCondition(x => x.DateOfBirth.Day == day && x.DateOfBirth.Month == month).ToList();
            return Ok(person);
        }

        [Route("EmployeeBirthday/{day}/{month}/{id}")]
        public IActionResult GetEmployeeBirthday([FromRoute]int day, [FromRoute]int month, [FromRoute]string id)
        {
            int personId = Convert.ToInt32(id);

            string birthdayEmployeeName = _repository.Employee.FindByCondition(x => x.Id == personId && x.DateOfBirth.Day == day && x.DateOfBirth.Month == month)?.FullName;
            return Ok(birthdayEmployeeName);
        }

        [Route("AttendanceRequest/{personId}/{dateSelected}/{message}")]
        public IActionResult AttendanceRequest([FromRoute]string personId,[FromRoute]string dateSelected,[FromRoute]string message)
        {            
            int pId = Convert.ToInt32(personId);
            DateTime date = Convert.ToDateTime(dateSelected);

            Attendance attendance=_repository.Attendances.FindByCondition(x => x.PersonId == pId && x.DateIn == date);
            if(attendance==null)
            {
                string employeeCode = _repository.Employee.FindByCondition(x => x.Id == pId).EmployeeCode;
                Attendance newAttendance = new Attendance();
                newAttendance.EmployeeCode = employeeCode;
                newAttendance.DateIn = date;
                newAttendance.TimeIn = new TimeSpan();
                newAttendance.IsActive = false;
                newAttendance.Message = message;
                newAttendance.HrStatus = false;
                newAttendance.CreatedDate = DateTime.Now;
                newAttendance.UpdatedDate = DateTime.Now;
                newAttendance.TenantId = TenantId;
                newAttendance.PersonId = pId;
                _repository.Attendances.CreateAndSave(newAttendance);               
            }
            else
            {
                attendance.Message = message;
                attendance.HrStatus = false;
                _repository.Attendances.UpdateAndSave(attendance);
            }

            string status = "success";
            return Ok(status);
        }
    }
}