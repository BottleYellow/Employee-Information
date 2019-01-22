using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
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
            var dashboard = _repository.Dashboard.GetAdminDashboard(TenantId);
            return Ok(dashboard);
        }

        [Route("Manager")]
        [HttpGet]
        public IActionResult GetManagerDashboard()
        {
            var dashboard = _repository.Dashboard.GetManagerDashboard(TenantId);
            return Ok(dashboard);
        }

        [Route("Employee/{PersonId}")]
        [HttpGet]
        public IActionResult GetEmployeeDashboard(int PersonId)
        {
            var dashboard = _repository.Dashboard.GetEmployeeDashboard(TenantId,PersonId);
            return Ok(dashboard);
        }
    }
}