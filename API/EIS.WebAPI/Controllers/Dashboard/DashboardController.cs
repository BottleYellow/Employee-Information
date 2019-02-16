using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using EIS.Entities.Dashboard;
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
        {        }

        [Route("Admin/{attendanceStatus}/{location}")]
        [HttpGet]
        public IActionResult GetAdminDashboard(string attendanceStatus, int location)
        {
            Admin_Dashboard dashboard = _repository.Dashboard.GetAdminDashboard(attendanceStatus, location, TenantId);
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
            Employee_Dashboard dashboard = _repository.Dashboard.GetEmployeeDashboard(TenantId,PersonId);
            return Ok(dashboard);
        }

        [Route("AdminCalendarData/{location}/{startDate}/{endDate}")]
        [HttpGet]
        public IActionResult GetAdminCalendarData([FromRoute]int location,[FromRoute]string startDate, [FromRoute]string endDate)
        {
            List<CalendarData> calendarDataList = _repository.Dashboard.GetAdminCalendarDetails(location,Convert.ToDateTime(startDate), Convert.ToDateTime(endDate));
            return Ok(calendarDataList);
        }

        [Route("EmployeeCalendarData/{personId}/{startDate}/{endDate}")]
        [HttpGet]
        public IActionResult GetEmployeeCalendarData([FromRoute]int personId, [FromRoute]string startDate, [FromRoute]string endDate)
        {
            List<CalendarData> calendarDataList = _repository.Dashboard.GetEmployeeCalendarDetails(personId, Convert.ToDateTime(startDate), Convert.ToDateTime(endDate));
            return Ok(calendarDataList);
        }
    }
}