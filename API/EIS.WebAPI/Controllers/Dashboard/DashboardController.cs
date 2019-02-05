using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using EIS.Entities.Dashboard;
using EIS.Entities.Models;
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

        [Route("Admin")]
        [HttpGet]
        public IActionResult GetAdminDashboard()
        {
            AdminDashboard dashboard = _repository.Dashboard.GetAdminDashboard(TenantId);
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
            EmployeeDashboard dashboard = _repository.Dashboard.GetEmployeeDashboard(TenantId,PersonId);
            return Ok(dashboard);
        }

        [Route("CalendarData/{startDate}/{endDate}")]
        [HttpGet]
        public IActionResult GetCalendarData([FromRoute]string startDate, [FromRoute]string endDate)
        {
            List<CalendarData> calendarDataList = _repository.Dashboard.GetCalendarDetails(Convert.ToDateTime(startDate), Convert.ToDateTime(endDate));
            return Ok(calendarDataList);
        }
    }
}