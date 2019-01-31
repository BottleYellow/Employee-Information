﻿using System;
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
        public IActionResult GetEmployeeDashboard()
        {
            int PersonId = Convert.ToInt32(Cache.GetStringValue("PersonId"));
            EmployeeDashboard dashboard = _repository.Dashboard.GetEmployeeDashboard(TenantId,PersonId);
            return Ok(dashboard);
        }

        [Route("CalendarData")]
        [HttpGet]
        public IActionResult GetCalendarData()
        {
            List<CalendarData> calendarDataList = _repository.Dashboard.GetCalendarDetails();
            return Ok(calendarDataList);
        }
    }
}